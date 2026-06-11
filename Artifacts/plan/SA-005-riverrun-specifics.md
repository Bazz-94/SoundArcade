# SA-005: RiverRun: implement three-lane endless runner specifics

## Description
Implement RiverRun-specific mechanics and data: three-lane world model, lane switching, obstacle and pickup types with SFX/TTS mapping, single-life GameOver, scoring by survival and pickups, and tuning parameters for difficulty ramping. Ensure all audio, positional SFX, and TTS announcements meet accessibility rules.

## Scope
Includes:
- Three-lane world model and lane positions mapped to `Vector3` coordinates.
- Player controller support for lane change, Left/Right, and Pause, Escape/P.
- Spawner that creates lane-based obstacles and pickups moving toward the player.
- Collision detection for obstacles and pickups, single-life collision -> GameOver.
- Assign SFX string IDs for lane change, pickup, collision; TTS announcements for score milestones, pause, and game over.

Excludes:
- Complex graphics or animation.
- Multiple levels or level selection.

## Metadata
- System area: Domain.Game1 / Application
- Type: Gameplay
- Dependencies: SA-001, SA-002, SA-003
- Complexity: Medium
- Status: Not Started

## Inputs
- `IAudio` and `ITts`, positional audio and TTS.
- PlayerController and GameLoop from Application.

## Outputs
- RiverRun game module with lane switching, pickups, obstacles, and scoring.
- Audio mapping document linking events to SFX/TTS IDs.
- Playtest tuning values for spawn rates and speeds.

## Acceptance Criteria
- Player can run, switch lanes, collect pickups, and trigger GameOver on collision.
- All significant events have SFX or TTS feedback and are comprehensible without visuals.
- Difficulty ramp parameters are exposed and tunable.

## Implementation Notes
- Represent lanes as discrete x-offsets; forward axis is Y increasing toward incoming obstacles.
- Pickups should provide short, distinct chimes spatialized to lane position.
- Provide a minimum viable audio mix: SFX, music optional, TTS announcements.

## Related Tasks
- SA-004
