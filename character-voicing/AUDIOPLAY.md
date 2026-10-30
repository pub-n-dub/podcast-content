## An audioplay

## Principle

The storyboard carries the argument; the audioplay gives it dramatic pressure,
space, and rhythm. Character voices are original performances, not imitations of
real people. Historical figures may be represented through attributed document
readings or clearly labeled reconstructions.

Every recorded line must be assigned one of these statuses in the session log:

- `NARRATION` — present-day host's argument.
- `DOCUMENT READING` — verified quotation, with source identifier.
- `RECONSTRUCTION` — invented but historically plausible dialogue or action.
- `SOUND ONLY` — a nonverbal scene or transition.

## Cast and voice direction

| Voice | Dramatic role | Performance direction | Permitted material |
| --- | --- | --- | --- |
| Host | Investigator and guide | Close, natural, precise; curiosity held in check by doubt. | Narration; transitions; stated inferences. |
| Archivist | Fictional evidentiary counterweight | Clear, measured, dry warmth; slightly more distant mic position. | Source framing; challenges; reads citations. |
| Document Reader: Berkeley | Gives the historical record a human presence | Original neutral period-inflected reading; no mimicry. | Verified Berkeley text only, attributed on screen. |
| Document Reader: Koff | Makes hands-on Squee work tangible | Practical, unshowy original reading; no mimicry. | Verified Koff text only, attributed on screen. |
| Workshop Voice A | Mechanical/fabrication perspective | Restrained, work-focused; distinguish through pace, not effects. | Reconstruction only. |
| Workshop Voice B | Electrical/integration perspective | Thoughtful, terse; natural voice. | Reconstruction only. |
| Tati Robot | Recurring nonverbal presence | Electromechanical motif, not a speech synthesizer. | Sound only. |

Do not give Porter, Jensen, Vall, Dennett, Berkeley, or Koff invented dialogue
under their own names. Where a reconstruction needs human speech, identify it as
an unnamed workshop voice in both the storyboard and session log.

## Versioned voice bible

The cast is intentionally a set of *functions in the investigation*, rather
than a gallery of accents. Distinction must be audible even in a dry mono
recording, before any mix treatment. Maintain these profiles as
`voice-profile: 1.0`; increment the version whenever a direction below changes.

### Host — the questioner

The Host carries the listener through an uncertain argument. They are close to
the microphone, attentive and exact, but never perform certainty that the
evidence has not earned.

| Dimension | Baseline | Under pressure | Do not drift into |
| --- | --- | --- | --- |
| Pace | 140–150 words per minute; sentences are allowed to breathe. | Slow by 10–15 WPM at a source gap; do not rush to resolve it. | Trailer-style urgency or lecturer cadence. |
| Tone | Natural, intimate, lightly skeptical. | Quieter and more deliberate—not darker or more theatrical. | A detective caricature or omniscient authority. |
| Articulation | Precise on names, dates, and claims; conversational elsewhere. | Give the final evidentiary noun a clean landing. | Over-enunciation throughout. |
| Pauses | 250–400 ms before a stated inference; 700–1,000 ms after an unresolved question. | Let silence carry an empty maker field. | Filling silence with a reflective sigh or a music cue. |

**Emotional tells:** curiosity makes the Host lean into the first concrete
detail; doubt makes the next sentence shorter. When corrected by the Archivist,
the Host receives the correction plainly and changes course—no defensive joke.

### Archivist — the evidentiary counterweight

The Archivist makes provenance, limits, and citations intelligible. This is not
an antagonist: the dry warmth should make a correction feel like useful craft.
Record with a slightly more distant mic position and a stable, seated posture.

| Dimension | Baseline | Under pressure | Do not drift into |
| --- | --- | --- | --- |
| Pace | 125–135 WPM; even and unhurried. | Slow slightly while separating what is known from inferred. | Bureaucratic monotone or academic scolding. |
| Tone | Clear, low-key, dry warmth. | More economical, not more forceful. | Mystery-narrator gravitas. |
| Articulation | Exact on archive terms and citation identifiers. | Put light stress on the limiting word: “provenance,” not “authorship.” | Sounding pleased to catch an error. |
| Pauses | 200–300 ms around a qualification; leave 400 ms after a correction. | Do not overlap the Host’s last word unless the board explicitly calls for it. | Comic timing or fussy hesitation. |

**Emotional tells:** confidence appears as calm specificity, never volume.
Compassion appears only as patience with ambiguity. The Host/Archivist contrast
is proximity and rhythm: the Host discovers; the Archivist calibrates.

### Document Reader: Berkeley — attributed record, human scale

This voice serves a verified Berkeley text and must be logged as `DOCUMENT
READING` with its source identifier. It is an original performance, not an
imitation or a claim about Berkeley's literal sound.

| Dimension | Direction |
| --- | --- |
| Pace | 115–125 WPM, with a measured line break between document clauses. |
| Tone | Neutral, composed, lightly period-inflected through formality of phrasing—not accent or mimicry. |
| Articulation | Let technical terms and proper names remain plain; retain quoted punctuation only when it changes sense. |
| Pauses | 300 ms at sentence ends; 500 ms before a consequential passage, not after every comma. |
| Mix identity | Subtle room only after the dry performance is approved; never archival crackle used as proof. |

**Do not drift into:** an elderly-professor caricature, a biographical
performance, invented connective speech, or any text that is not verified and
attributed.

### Document Reader: Koff — practical record, hands-on scale

This reader also serves verified text only. Koff's material should feel
practical and material without becoming a character voice or a technical
enthusiast's impression.

| Dimension | Direction |
| --- | --- |
| Pace | 130–140 WPM—slightly more active than Berkeley, while remaining plain. |
| Tone | Unshowy, workmanlike, alert to concrete actions and materials. |
| Articulation | Clean verbs and object names; give procedural sequences a steady forward motion. |
| Pauses | 200–300 ms at sentence ends; 350 ms before a quoted action whose consequence will be heard in Foley. |
| Mix identity | The same restrained documentary room as Berkeley; separation comes from pace and text, not an effect. |

**Do not drift into:** a regional or occupational stereotype, an impersonation,
invented commentary, or “performing” tool noises in the voice.

### Workshop Voice A — fabrication reconstruction

Workshop A speaks only in clearly logged `RECONSTRUCTION` passages. Their job
is to make mechanical work legible: sequence, resistance, weight, and the
small decision made at a bench.

| Dimension | Direction |
| --- | --- |
| Pace | 120–130 WPM; short practical clauses. |
| Tone | Restrained and task-focused; attention stays on the work, not the self. |
| Articulation | Firm consonants on physical actions: fit, turn, brace, test. |
| Pauses | 150–250 ms between actions; permit a 400 ms pause before a failed fit or test. |
| Relationship to sound | Dialogue stops before the featured `WRK-*` asset so the action can be heard. |

**Do not drift into:** a named historical person, nostalgic “shop talk,” comic
gruffness, or a stylized old-time accent. A's rhythm is practical and slightly
slower than B's.

### Workshop Voice B — integration reconstruction

Workshop B also exists only for `RECONSTRUCTION`. They bring the electrical and
systems perspective: concise observations, checks, and consequences.

| Dimension | Direction |
| --- | --- |
| Pace | 135–145 WPM; terse, with no wasted lead-in. |
| Tone | Thoughtful, natural, contained. |
| Articulation | Crisp on states and decisions: live, loose, connected, again. |
| Pauses | 100–200 ms between checks; 350 ms before a result that changes the next action. |
| Relationship to sound | Leave the relay click, switch throw, or motor test uncovered unless the board marks it as under-dialogue. |

**Do not drift into:** a “radio engineer” stereotype, technobabble, swagger,
or overlap that turns the workshop scene into an argument. B is quicker and
more diagnostic than A, not necessarily higher-pitched.

### Tati Robot — nonverbal recurring presence

Tati is never rendered from a speech voice or text-to-speech engine. Its
recognisable performance is the ordered motif `TATI-01` click → `TATI-02` low
strain → `TATI-03`/`TATI-04` movement → `TATI-05` stalled motion → `TATI-06`
silence tail. Vary duration, spacing, and foreground level by scene, but retain
the order whenever the motif is complete. The final silence is part of the
character, not unused track space.

## Performance-plan fields

For every independently renderable spoken segment, store the profile version
and cues explicitly. Punctuation may support phrasing, but may not be the only
timing instruction.

```yaml
id: board-02-host-003
voice: Host
voice_profile: host-1.0
line_status: NARRATION
text: "The file can tell us where it was kept. It cannot tell us who made it."
performance:
  pace_wpm: 142
  tone: "precise; doubt held in check"
  pause_between_sentences_ms: 350
  pause_after_ms: 800
  emphasis: ["cannot"]
mix_intent: foreground
assets_after: [ARC-07]
```

## Recording method

Record each spoken part dry, mono, 48 kHz/24-bit WAV. Preserve:

1. original take;
2. selected edit;
3. processed mix stem; and
4. final mix reference.

Character separation should come first from writing, pacing, mic distance, and
room treatment. Use processing sparingly: gentle EQ and dynamics for clarity;
subtle room for Archivist/document readings; no synthetic impersonation or
voice clone. Test each treatment on headphones and a phone speaker.

## Sound-byte library

Build short, clean, reusable sounds rather than one long atmospheric recording.
Each asset gets an ID, source/recording note, duration, rights status, and
recommended loudness.

| ID range | Sound bytes | Use |
| --- | --- | --- |
| `ARC-01`–`ARC-08` | paper sleeve, photograph set down, page turn, pencil, desk lamp, filing drawer, room tone, chair movement | Archive and evidence-card transitions. |
| `DOC-01`–`DOC-05` | typewriter key cluster, paper feed, stamp, binder close, microfilm/projector-style mechanical texture | Published-line and document moments; use lightly. |
| `WRK-01`–`WRK-12` | screw tray, screwdriver, wire handling, chassis tap, relay click, switch throw, soldering ambience, motor test, fan/ventilation, tool set-down | Explicitly reconstructed workshop scenes. |
| `TATI-01`–`TATI-06` | contact click, low motor strain, spring movement, head/jaw-like mechanism, stalled motion, silence tail | The robot motif: click → strain → motion → stop. |
| `TRN-01`–`TRN-04` | low tonal bridge, tape-stop-like restraint, short room decay, single piano/harmonium note if licensed or recorded | Section changes only; never cue a false revelation. |

Record original Foley where possible. Do not lift historical recordings, film
audio, commercial effects, or music without a documented licence. A sound made
for a reconstruction must not be presented as recorded evidence of the original
robot.

## Board-to-audio handoff

For each storyboard board, add these fields:

| Field | Requirement |
| --- | --- |
| `Voice` | Host, Archivist, Document Reader, Workshop A/B, or none. |
| `Line status` | Narration, Document Reading, Reconstruction, or Sound Only. |
| `Audio assets` | Exact IDs, not a vague instruction such as “workshop sounds.” |
| `Mix intent` | Foreground, under narration, transition, or silence. |
| `Evidence check` | Required for every documentary reading and on-screen quote. |

Silence is an intentional asset. It should follow an unresolved source gap and
precede the final `BUILDER: UNKNOWN` return.

## First recording package

Prepare a 2½-minute proof-of-format for storyboard boards 01–06:

1. Host cold open and evidence contract.
2. Archivist correction: provenance is not authorship.
3. `ARC-01`, `ARC-02`, and `ARC-07` archive Foley.
4. `TATI-01` through `TATI-06` as the robot motif.
5. A 10–15 second stretch of deliberate silence after the empty maker field.

Approve this proof only when voices remain distinct without heavy processing,
the evidence labels are intelligible, and the Tati motif feels particular rather
than generic.
