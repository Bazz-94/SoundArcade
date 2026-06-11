# SA-001: Core gameplay loop: single progressive level & scoring

## Description
Implement the core gameplay loop for the single-level progressive game: spawn player, spawn progressive obstacles/enemies that increase difficulty over time, handle input through `IInput`, scoring, lives, and game over. The goal is high-score play on a single level that gets harder rather than multiple levels. Ensure all game events emit audio/TTS via `ITts` and `IAudio` per architect rules.

## Scope
Includes:
- Player entity and controller that consumes `IInput` commands.
- Spawner service for obstacles/enemies with difficulty ramp over time.
- Score system and lives management.
- Game state machine: Playing, Paused, GameOver.
- Emit TTS and audio events on significant state changes.

Excludes:
- Visual polish or complex rendering.
- Level selection or multiple discrete levels.

## Metadata
- System area: Application / Domain.Game1
- Type: Gameplay
- Dependencies: SA-000
- Complexity: High
- Status: Not Started

## Inputs
- `IInput` interface from Abstractions.
- `ITts` and `IAudio` interfaces.
- GameLoop entry point from Application.

## Outputs
- Playable single-level loop with difficulty ramp and scoring.
- Events emitted for audio announcements.
- Unit tests for scoring and state transitions.

## Acceptance Criteria
- Player can start a run, earn score, lose lives, and reach GameOver.
- Difficulty increases over time, using spawn rate or speed.
- All state transitions produce TTS or audio feedback.
- Game uses only Abstractions, no Raylib, and adheres to dependency rules.

## Implementation Notes
- Use time-based difficulty curve, linear or exponential, configurable via settings.
- Keep world positions 3D `Vector3`; `z` can be zero.
- Design spawner to be data-driven to allow tuning without code changes.

## Related Tasks
- SA-002
- SA-003
