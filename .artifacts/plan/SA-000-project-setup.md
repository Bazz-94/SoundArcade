# SA-000: Project setup & PAL enforcement

## Description
Create and verify solution layout, CI skeleton, coding standards, and ensure the Platform Abstraction Layer (Abstractions) is present and matches the architect decisions (`IAudio`, `ITts`, `IInput`, `IRenderer`, `IWindow`). Add example wiring in Desktop to register one mini-game.

## Scope
Includes:
- Create .NET 8 solution layout matching `architect.agent.md`.
- Verify/define Abstractions interfaces, signatures only, for `IAudio`, `ITts`, `IInput`, `IRenderer`, `IWindow`.
- Add sample registration in `SoundArcade.Desktop` to register a placeholder mini-game.

Excludes:
- Implementations of Abstractions, Raylib beyond trivial stubs.
- Game-specific domain logic.

## Metadata
- System area: Infrastructure / Solution
- Type: Gameplay
- Dependencies: None
- Complexity: Low
- Status: Done

## Inputs
- Architect decisions (`architect.agent.md`)
- Repository root

## Outputs
- Buildable solution with correct project structure.
- Abstractions interface stubs under `SoundArcade.Abstractions/`.
- Desktop wiring example registering one `IGame`.

## Acceptance Criteria
- Solution builds locally targeting .NET 8 on developer machine.
- Abstractions contain method signatures that use platform primitives only.
- Desktop shows example registration without referencing Domain models.

## Implementation Notes
- Follow dependency rules from `architect.agent.md`.
- Keep Abstractions free of Model types; use primitives and simple structs (`Vector3`, `Color`).
- Create minimal unit test project to satisfy CI.

## Related Tasks
- SA-001
- SA-002
