using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AeonVoice;

var options = ParseOptions(args);
if (options.ShowHelp) { PrintUsage(); return 0; }

try
{
    PreloadAeonVoiceNativeLibraries();
    if (options.SegmentsPath is not null)
    {
        var plan = ReadPlan(options.SegmentsPath);
        ValidatePlan(plan);
        if (options.ValidateOnly)
        {
            Console.WriteLine($"Validated {plan.Segments.Count} independently renderable segments.");
            return 0;
        }
        if (options.OutputDirectory is null) throw new ArgumentException("--output-dir is required when rendering --segments.");
        RenderPlan(plan, options.OutputDirectory);
        return 0;
    }
    RenderLegacyInput(options);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Cadencer failed.");
    Console.Error.WriteLine(ex.Message);
    return 1;
}

static CommandOptions ParseOptions(string[] args)
{
    var result = new CommandOptions();
    for (var i = 0; i < args.Length; i++)
    {
        string Value(string flag) => i + 1 < args.Length ? args[++i] : throw new ArgumentException($"{flag} requires a value.");
        switch (args[i])
        {
            case "--segments": result.SegmentsPath = Value("--segments"); break;
            case "--output-dir": result.OutputDirectory = Value("--output-dir"); break;
            case "--validate": result.ValidateOnly = true; break;
            case "--input": result.InputPath = Value("--input"); break;
            case "--output": result.OutputPath = Value("--output"); break;
            case "--voice": result.LegacyVoice = Value("--voice"); break;
            case "--cadence": result.LegacyCadence = Value("--cadence"); break;
            case "--help": case "-h": result.ShowHelp = true; break;
            default: throw new ArgumentException($"Unknown argument: {args[i]}");
        }
    }
    if (result.SegmentsPath is null && result.InputPath is null && !result.ShowHelp)
        throw new ArgumentException("Provide --segments <plan.json> or legacy --input <text-file>.");
    if (result.SegmentsPath is not null && result.InputPath is not null)
        throw new ArgumentException("Use either --segments or --input, not both.");
    return result;
}

static SegmentPlan ReadPlan(string path)
{
    if (!File.Exists(path)) throw new FileNotFoundException("Segment manifest not found.", path);
    var plan = JsonSerializer.Deserialize<SegmentPlan>(File.ReadAllText(path), CreateJsonOptions())
        ?? throw new InvalidDataException("Segment manifest is empty or invalid JSON.");
    plan.SourcePath = Path.GetFullPath(path);
    return plan;
}

static void ValidatePlan(SegmentPlan plan)
{
    if (plan.SchemaVersion != 1) throw new InvalidDataException("schemaVersion must be 1.");
    if (string.IsNullOrWhiteSpace(plan.ScriptRevision)) throw new InvalidDataException("scriptRevision is required.");
    if (plan.Voices.Count == 0 || plan.Segments.Count == 0) throw new InvalidDataException("Plan requires voices and segments.");
    foreach (var role in plan.RequiredVoices)
        if (!plan.Voices.ContainsKey(role)) throw new InvalidDataException($"Required voice '{role}' is not declared.");
    var engineVoices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var (role, voice) in plan.Voices)
    {
        if (string.IsNullOrWhiteSpace(voice.EngineVoice)) throw new InvalidDataException($"Voice '{role}' has no engineVoice mapping.");
        if (!engineVoices.Add(voice.EngineVoice)) throw new InvalidDataException($"Engine voice '{voice.EngineVoice}' is assigned to more than one role.");
        var resourceDirectory = Path.Combine(AppContext.BaseDirectory, "aeonvoice", "data", "voices", voice.EngineVoice.ToLowerInvariant());
        if (!Directory.Exists(resourceDirectory)) throw new DirectoryNotFoundException($"Voice '{role}' requires missing runtime resource '{resourceDirectory}'.");
    }
    var ids = new HashSet<string>(StringComparer.Ordinal);
    foreach (var segment in plan.Segments)
    {
        if (string.IsNullOrWhiteSpace(segment.Id) || !Regex.IsMatch(segment.Id, "^[A-Za-z0-9][A-Za-z0-9._-]*$"))
            throw new InvalidDataException($"Invalid segment id '{segment.Id}'.");
        if (!ids.Add(segment.Id)) throw new InvalidDataException($"Duplicate segment id '{segment.Id}'.");
        if (string.IsNullOrWhiteSpace(segment.Text)) throw new InvalidDataException($"Segment '{segment.Id}' has no text.");
        if (!plan.Voices.TryGetValue(segment.Voice, out var voice) || string.IsNullOrWhiteSpace(voice.EngineVoice))
            throw new InvalidDataException($"Segment '{segment.Id}' has no engineVoice mapping for '{segment.Voice}'.");
        if (segment.PaceWpm is <= 0 or > 300) throw new InvalidDataException($"Invalid paceWpm for '{segment.Id}'.");
        var relativeRate = segment.PaceWpm / (double)voice.NominalWpm;
        if (voice.NominalWpm is <= 0 or > 300 || relativeRate is < 0.5 or > 2.0)
            throw new InvalidDataException($"Segment '{segment.Id}' resolves to an invalid relative speech rate.");
        var pitch = segment.RelativePitch ?? voice.RelativePitch;
        var volume = segment.RelativeVolume ?? voice.RelativeVolume;
        if (pitch is < 0.5 or > 2.0 || volume is < 0.5 or > 2.0)
            throw new InvalidDataException($"Segment '{segment.Id}' has invalid pitch or volume controls.");
        if (segment.PauseAfterMs is < 0 or > 60_000) throw new InvalidDataException($"Invalid pauseAfterMs for '{segment.Id}'.");
    }
}

static void RenderPlan(SegmentPlan plan, string outputDirectory)
{
    var outputRoot = Path.GetFullPath(outputDirectory);
    var masters = Path.Combine(outputRoot, "masters");
    Directory.CreateDirectory(masters);
    using var engine = new AeonVoiceEngine();
    var rendered = new List<RenderedSegment>();
    foreach (var segment in plan.Segments)
    {
        var voice = plan.Voices[segment.Voice];
        var relativeRate = segment.PaceWpm / (double)voice.NominalWpm;
        var relativePitch = segment.RelativePitch ?? voice.RelativePitch;
        var relativeVolume = segment.RelativeVolume ?? voice.RelativeVolume;
        var spokenText = PrepareForNarration(ApplyPronunciations(segment.Text, plan.Pronunciations));
        var audio = engine.SynthesizeToPcm16(spokenText, voice.EngineVoice, new SynthesisOptions(relativeRate, relativePitch, relativeVolume));
        ValidateAudio(segment.Id, audio);
        var master = ResampleForMaster(audio);
        var relativePath = $"masters/{segment.Id}.wav";
        var masterPath = Path.Combine(outputRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        WritePcm24Wave(masterPath, master.SampleRate, master.Samples);
        rendered.Add(new(segment.Id, segment.Voice, voice.EngineVoice, relativePath,
            audio.SampleRate, master.SampleRate, master.Samples.Length, Math.Round(master.Samples.Length * 1000d / master.SampleRate),
            segment.PaceWpm, voice.NominalWpm, relativeRate, relativePitch, relativeVolume,
            segment.PauseAfterMs, Sha256(segment.Text), Sha256(spokenText), Sha256File(masterPath)));
        Console.WriteLine($"Rendered {segment.Id} -> {relativePath}");
    }
    var release = new ReleaseManifest("cadencer-release-1", DateTimeOffset.UtcNow, Path.GetFileName(plan.SourcePath),
        Sha256File(plan.SourcePath), plan.ScriptRevision,
        typeof(AeonVoiceEngine).Assembly.GetName().Version?.ToString() ?? "unknown",
        "PCM WAV, mono, 24-bit; no inserted pauses or mix processing", rendered);
    var manifestPath = Path.Combine(outputRoot, "release-manifest.json");
    File.WriteAllText(manifestPath, JsonSerializer.Serialize(release, CreateJsonOptions()));
    Console.WriteLine($"Wrote release manifest: {manifestPath}");
}

static void RenderLegacyInput(CommandOptions options)
{
    if (!File.Exists(options.InputPath)) throw new FileNotFoundException("Input file not found.", options.InputPath);
    var text = File.ReadAllText(options.InputPath).Trim();
    if (string.IsNullOrWhiteSpace(text)) throw new InvalidDataException("Input file is empty.");
    var outputPath = options.OutputPath ?? Path.ChangeExtension(options.InputPath, ".wav");
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
    using var engine = new AeonVoiceEngine();
    var audio = options.LegacyCadence.Equals("raw", StringComparison.OrdinalIgnoreCase)
        ? engine.SynthesizeToPcm16(PrepareForNarration(text), options.LegacyVoice)
        : SynthesizeLegacyCadence(engine, text, options.LegacyVoice);
    ValidateAudio("legacy-input", audio);
    var master = ResampleForMaster(audio);
    WritePcm24Wave(outputPath, master.SampleRate, master.Samples);
    Console.WriteLine($"Rendered 24-bit WAV master: {outputPath}");
    Console.WriteLine("Compatibility mode: use --segments for independently renderable production audio.");
}

static SynthesisResult SynthesizeLegacyCadence(AeonVoiceEngine engine, string text, string voice)
{
    var samples = new List<short>();
    var rate = 0;
    foreach (var sentence in Regex.Split(text, @"(?<=[.!?])\s+").Where(x => !string.IsNullOrWhiteSpace(x)))
    {
        var partial = engine.SynthesizeToPcm16(PrepareForNarration(sentence.Trim()), voice);
        rate = rate == 0 ? partial.SampleRate : rate;
        if (partial.SampleRate != rate) throw new InvalidOperationException("Engine returned mixed sample rates.");
        samples.AddRange(partial.Samples);
        samples.AddRange(new short[(int)Math.Round(rate * .43)]);
    }
    return new SynthesisResult(rate, samples.ToArray());
}

static void ValidateAudio(string id, SynthesisResult audio)
{
    if (audio.SampleRate <= 0 || audio.Samples.Length == 0) throw new InvalidDataException($"'{id}' produced invalid audio.");
    if (audio.Samples.All(sample => Math.Abs((int)sample) < 16)) throw new InvalidDataException($"'{id}' rendered as silence.");
}

static MasterAudio ResampleForMaster(SynthesisResult source)
{
    const int masterSampleRate = 48_000;
    if (source.SampleRate == masterSampleRate) return new MasterAudio(masterSampleRate, source.Samples);
    var outputLength = checked((int)Math.Round(source.Samples.Length * (double)masterSampleRate / source.SampleRate));
    var output = new short[outputLength];
    for (var index = 0; index < output.Length; index++)
    {
        var position = index * (double)source.SampleRate / masterSampleRate;
        var left = (int)position;
        var right = Math.Min(left + 1, source.Samples.Length - 1);
        var fraction = position - left;
        output[index] = (short)Math.Round(source.Samples[left] + (source.Samples[right] - source.Samples[left]) * fraction);
    }
    return new MasterAudio(masterSampleRate, output);
}

static void WritePcm24Wave(string path, int sampleRate, IReadOnlyList<short> samples)
{
    using var writer = new BinaryWriter(File.Create(path), Encoding.UTF8);
    const short channels = 1, bits = 24;
    var bytes = checked(samples.Count * 3);
    writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + bytes); writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
    writer.Write(16); writer.Write((short)1); writer.Write(channels); writer.Write(sampleRate);
    writer.Write(sampleRate * channels * bits / 8); writer.Write((short)(channels * bits / 8)); writer.Write(bits);
    writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(bytes);
    foreach (var sample in samples)
    {
        var value = ((int)sample) << 8;
        writer.Write((byte)value); writer.Write((byte)(value >> 8)); writer.Write((byte)(value >> 16));
    }
}

static void PreloadAeonVoiceNativeLibraries()
{
    foreach (var name in new[] { "libAeonVoice_core.so.10", "libAeonVoice_audio.so.2", "libAeonVoice.so" })
    {
        var path = Path.Combine(AppContext.BaseDirectory, name);
        if (File.Exists(path)) NativeLibrary.Load(path);
    }
}

static string PrepareForNarration(string text) => Regex.Replace(text, @"\bSupertoy\b", "Super toy", RegexOptions.IgnoreCase);
static string ApplyPronunciations(string text, IReadOnlyDictionary<string, string> pronunciations)
{
    foreach (var (written, spoken) in pronunciations.OrderByDescending(pair => pair.Key.Length))
        text = Regex.Replace(text, $@"\b{Regex.Escape(written)}\b", spoken, RegexOptions.IgnoreCase);
    return text;
}
static string Sha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
static string Sha256File(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

static void PrintUsage()
{
    Console.WriteLine("Cadencer: dotnet run --project character-voicing/cadencer -- --segments <plan.json> --output-dir <release-dir>");
    Console.WriteLine("Validate only: --segments <plan.json> --validate");
    Console.WriteLine("Renders one 48 kHz/24-bit WAV master per segment and a release manifest. Mixing and web encoding remain downstream.");
}

static JsonSerializerOptions CreateJsonOptions() => new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

sealed class CommandOptions { public string? SegmentsPath { get; set; } public string? OutputDirectory { get; set; } public bool ValidateOnly { get; set; } public string? InputPath { get; set; } public string? OutputPath { get; set; } public string LegacyVoice { get; set; } = "Leena"; public string LegacyCadence { get; set; } = "adaptive"; public bool ShowHelp { get; set; } }
sealed class SegmentPlan { public int SchemaVersion { get; set; } public string ScriptRevision { get; set; } = ""; public List<string> RequiredVoices { get; set; } = []; public Dictionary<string, string> Pronunciations { get; set; } = new(StringComparer.OrdinalIgnoreCase); public Dictionary<string, VoicePlan> Voices { get; set; } = new(StringComparer.Ordinal); public List<SegmentSpec> Segments { get; set; } = []; [JsonIgnore] public string SourcePath { get; set; } = ""; }
sealed class VoicePlan { public string EngineVoice { get; set; } = ""; public int NominalWpm { get; set; } public double RelativePitch { get; set; } = 1; public double RelativeVolume { get; set; } = 1; public string? RegistryId { get; set; } public string? Profile { get; set; } }
sealed class SegmentSpec { public string Id { get; set; } = ""; public string Text { get; set; } = ""; public string Voice { get; set; } = ""; public string? Status { get; set; } public int PaceWpm { get; set; } public double? RelativePitch { get; set; } public double? RelativeVolume { get; set; } public int PauseAfterMs { get; set; } }
sealed record MasterAudio(int SampleRate, short[] Samples);
sealed record RenderedSegment(string Id, string Voice, string EngineVoice, string MasterPath, int EngineSampleRate, int SampleRate, int Samples, double DurationMs, int PaceWpm, int NominalWpm, double RelativeRate, double RelativePitch, double RelativeVolume, int PlannedPauseAfterMs, string TextSha256, string SpokenTextSha256, string MasterSha256);
sealed record ReleaseManifest(string Schema, DateTimeOffset RenderedAtUtc, string SourceManifest, string SourceManifestSha256, string ScriptRevision, string EngineVersion, string MasterFormat, List<RenderedSegment> Segments);
