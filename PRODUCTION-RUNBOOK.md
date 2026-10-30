# Audioplay production runbook

This is the locked work order for the Episode 15 proof package. The authored
script, performance plan, render manifest, masters, mix, and release manifest
are separate artifacts. Never replace a master with an encoded web deliverable.

## 1. Approve the six voice assignments

Render the same neutral passage with each profile and review it on headphones
and a phone speaker. The approved mapping is unique by role:

| Role | AeonVoice resource | Dramatic test |
| --- | --- | --- |
| Host | `Leena` | Close curiosity with doubt held in check. |
| Archivist | `Daria` | Measured correction with dry warmth. |
| Document Reader: Berkeley | `Bdl` | Neutral, attributed documentary reading. |
| Document Reader: Koff | `Evgeniy-eng` | Plain, practical documentary reading. |
| Workshop Voice A | `Ksp` | Deliberate fabrication actions. |
| Workshop Voice B | `Helen` | Terse integration checks. |

Tati is not a seventh voice: it remains the nonverbal `TATI-01`–`TATI-06`
motif. If an audition fails, change only the `engineVoice` mapping in the
render manifest and increment the relevant voice-profile version.

Render the ready-to-audition package with:

```bash
dotnet run --project character-voicing/cadencer -- \
  --segments character-voicing/voice-audition-plan.json \
  --output-dir releases/voice-audition-1
```

## 2. Compile script to render manifest

Keep prose in `audioplay-script.md`, performance choices in
`episode-15-proof-plan.yaml`, and the renderable text in
`episode-15-render-plan.json`. Before each render, verify every script segment
ID exists exactly once in the JSON manifest and that the JSON has the current
script revision.

## 3. Apply per-segment pace in AeonVoice

Cadencer must pass `paceWpm` as a relative native speech rate instead of merely
recording it as direction. Each voice declares a `nominalWpm`; Cadencer computes
`relativeRate = paceWpm / nominalWpm`, accepts only the engine-safe range
`0.5`–`2.0`, and includes the computed setting in the release manifest.

The AeonVoice .NET binding now exposes this through `SynthesisOptions` and the
three-argument `SynthesizeToPcm16` overload. The two-argument overload remains
at a relative rate of `1.0` for compatibility.

## 4. Render dialogue masters

```bash
dotnet run --project character-voicing/cadencer -- \
  --segments character-voicing/episode-15-render-plan.json \
  --output-dir releases/episode-15-proof
```

Cadencer validates required roles, unique engine voices, runtime resources,
segments, silence, and timing values; it writes one mono 48 kHz/24-bit WAV per
speech segment plus `release-manifest.json`.

## 5. Record and mix sound

Record the specified original `ARC-*` and `TATI-*` Foley, with source, rights,
duration, and hash logged. Assemble dialogue, Foley, ambience, and the
intentional final silence in a separate mix session. Do not cover the complete
Tati motif with dialogue, and introduce no music during A06.

## 6. Release checks

Validate source labels, source cards for factual claims, assets, broken paths,
dialogue audibility, ten-second `BUILDER: UNKNOWN` hold, loudness, transcript,
and release-manifest hashes. Archive original takes, selected masters, mix
stems, final mix, web encode, and the completed manifest together.
