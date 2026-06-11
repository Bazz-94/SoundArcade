# SA-003: Input mapping and Player Controller integration

## Description
Implement `IInput` handling in Infrastructure and expose a mapping system allowing keyboard remapping for core actions (`MoveLeft`, `MoveRight`, `Action`, `Pause`). Provide PlayerController in Application that consumes `IInput` and issues domain commands. Ensure all input flows through Abstractions and supports keyboard-only controls per accessibility rules.

## Scope
Includes:
- `IInput` implementation that polls keyboard and exposes events.
- Configuration storage for key mappings with save/load.
- PlayerController that reads mapped actions and invokes game commands.

Excludes:
- Gamepad/controller full support, optional later.
- Advanced rebinding UI beyond a simple settings screen.

## Metadata
- System area: Infrastructure / Application
- Type: Gameplay
- Dependencies: SA-000
- Complexity: Medium
- Status: Completed

## Inputs
- `IInput` interface from Abstractions.
- Key mapping settings.

## Outputs
- Key mapping persisted to settings.
- PlayerController consuming mapped inputs.

## Acceptance Criteria
- Player input triggers game actions correctly.
- Mappings persist between runs.
- All input goes through `IInput` interface.

## Implementation Notes
- Provide sane defaults for accessibility, arrow keys, Enter, Escape.
- Design mappings as Action -> KeyCode so UI can present them easily.

## Related Tasks
- SA-001
- SA-004
