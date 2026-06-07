```yaml id="task_artifact"
task:
  id: SA-007
  title: Generated SFX and ambient audio support
  description: Extend the shared audio layer so SoundArcade can play simple generated sound effects and low-volume continuous ambient cues through the existing audio abstraction, with a Raylib-backed implementation suitable for RiverRun and future games.

  scope:
    includes:
      - Add the minimum IAudio surface needed for simple generated sounds and controllable ambient playback without exposing Raylib types.
      - Implement Raylib-backed generated sounds for short positive, negative, and input-style cues behind stable sound identifiers.
      - Implement support for soft looping ambient cues that can be started, updated, and stopped by the application layer.
      - Register and wire the new audio capability in the desktop composition root.
    excludes:
      - RiverRun-specific lane logic, cue timing, or event mapping.
      - Authored audio asset pipelines, external sound design tooling, or advanced DSP.
      - Global audio settings UI or per-category mixer controls.

  system_area: Abstractions / Infrastructure / Desktop

  type: Audio

  dependencies:
    - task_id: SA-002

  inputs:
    - Existing IAudio abstraction and RaylibAudio implementation.
    - Stable semantic sound identifiers for pickup, hit, and input feedback.
    - RiverRun requirement for continuous left/right river ambience.

  outputs:
    - Updated audio abstraction that supports generated cues and controllable ambient playback.
    - Raylib infrastructure implementation for synthesized basic SFX and looping ambient audio.
    - DI registration and a small set of reusable semantic sound IDs for downstream games.

  acceptance_criteria:
    - Application code can trigger simple generated SFX by semantic identifier without calling Raylib directly.
    - Application code can start and stop continuous ambient cues and adjust their spatial position or mix through the abstraction boundary.
    - Generated pickup, hit, and input cues are audibly distinct in a short manual smoke test.
    - The new abstraction keeps domain-facing signatures limited to primitive types and Vector3.

  implementation_notes:
    - Keep synthesis simple and deterministic; short tone or noise-based cues are sufficient.
    - Treat waveform generation as an infrastructure detail and expose only semantic IDs to consumers.
    - Ambient support should be lightweight and optimized for a small number of always-on cues.
    - Preserve current TTS ducking behavior when ambience or generated SFX are active.

  complexity: Medium

  status: Not Started

  related_tasks:
    - task_id: SA-005
    - task_id: SA-006
```