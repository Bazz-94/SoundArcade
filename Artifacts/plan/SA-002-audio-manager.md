# SA-002: Audio Manager and TTS integration

## Description
Implement an `IAudio`-based Audio Manager in Infrastructure that provides 3D positional playback, global volume control, SFX, music playback, and integrates with `ITts` for announcements. Ensure low-latency playback and provide hooks for audio ducking when TTS is active.

## Scope
Includes:
- Raylib or platform-backed implementation of `IAudio` and `ITts` stubs.
- Audio Manager service exposed to Application via Abstractions.
- Simple music playback with play/pause/stop and volume control.
- Ducking logic when TTS speaks.

Excludes:
- Complex DSP or audio graph editing.
- Platform-specific optimizations beyond Raylib.

## Metadata
- System area: Infrastructure / Abstractions
- Type: Audio
- Dependencies: SA-000
- Complexity: Medium
- Status: Not Started

## Inputs
- `IAudio` and `ITts` interfaces from Abstractions.
- Audio assets referenced by string IDs.

## Outputs
- Concrete `IAudio` and `ITts` implementations registered in DI.
- Audio Manager API usable by Application and Domain.

## Acceptance Criteria
- SFX and music play when requested via `IAudio` calls.
- TTS announcements can interrupt and duck music appropriately.
- Audio Manager adheres to PAL signatures and does not leak Domain types.

## Implementation Notes
- Keep simple mapping from string IDs to loaded audio buffers.
- Expose simple 3D playback method: `PlayAt(string id, Vector3 position, float volume)`.
- Wrap TTS synth calls to provide async callbacks when speaking starts/ends.

## Related Tasks
- SA-001
- SA-004
