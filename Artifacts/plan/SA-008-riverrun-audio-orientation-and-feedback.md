```yaml id="task_artifact"
task:
  id: SA-008
  title: RiverRun audio orientation and feedback
  description: Add RiverRun-specific audio behavior using left and right river ambience for lane orientation and map core gameplay and input events to simple generated sounds that remain readable alongside speech.

  scope:
    includes:
      - Drive continuous left and right river ambience during RiverRun so players infer lane position from the stereo field
      - Map RiverRun gameplay events to semantic audio IDs for coin pickup, obstacle hit or life loss, and lane or menu input feedback
      - Tune RiverRun audio levels so ambient guidance stays present without masking TTS or critical feedback
      - Add focused tests around RiverRun audio event selection or coordinator behavior where practical
    excludes:
      - New TTS narration content or score-announcement redesign
      - Dynamic weather, reactive water simulation, or multi-layer environmental soundscapes
      - Audio changes for other games beyond shared menu or input cues already consumed by RiverRun

  system_area: Domain.RiverRun / Application / Tests

  type: Gameplay

  dependencies:
    - task_id: SA-005
    - task_id: SA-007

  inputs:
    - RiverRun lane model and player position updates
    - Shared audio semantic IDs and ambient playback support from the audio layer
    - Existing RiverRun gameplay events for pickups, collisions, pause, and movement

  outputs:
    - RiverRun audio coordinator or equivalent application wiring for lane ambience and feedback cues
    - RiverRun event-to-sound mapping for pickup, hit, and input feedback
    - Tests or verification hooks that confirm the correct audio calls are issued for key gameplay situations

  acceptance_criteria:
    - During a run, the player hears stable river ambience on both sides and uses it to distinguish left, center, and right lane positioning
    - Collecting a coin, hitting an obstacle or losing a life, and core input actions each trigger a distinct generated cue
    - River ambience remains softer than critical feedback and does not mask TTS announcements in a manual accessibility pass
    - RiverRun-specific audio behavior is driven from application or coordinator code, not from direct Raylib calls in the domain

  implementation_notes:
    - Use the existing lane coordinates to anchor ambience consistently to the river banks rather than to transient objects
    - Prefer semantic event mapping over scattering literal sound IDs across RiverRun code
    - Keep cue lengths short so repeated actions do not muddy the mix
    - Favor narrow tests around audio dispatch and lane-to-ambience mapping instead of broad end-to-end audio assertions

  complexity: Medium

  status: Not Started

  related_tasks:
    - task_id: SA-002
    - task_id: SA-007
```