# SA-004: HUD, main menu, and settings persistence

## Description
Implement basic UI components using `IRenderer` and `ITts`: HUD displaying score and lives, visually optional, main menu navigable by keyboard, pause menu, and settings screen to change audio volume and key mappings. Persist settings to disk. Ensure menus follow accessibility rules and announce focused items via `ITts`.

## Scope
Includes:
- Main menu, pause menu, and settings screens.
- HUD overlay for score and lives, rendered via `IRenderer` but not required for gameplay.
- Settings persistence for audio volume and key mappings.
- `ITts` announcements for menu focus and selections.

Excludes:
- Complex graphical UI frameworks.
- Localization beyond English.

## Metadata
- System area: Application / Infrastructure
- Type: UI
- Dependencies: SA-000, SA-003, SA-002
- Complexity: Medium
- Status: Completed

## Inputs
- `ITts` and `IRenderer` from Abstractions.
- Settings storage service.

## Outputs
- Navigable main menu and settings UI.
- HUD overlays and TTS announcements for menu navigation.
- Persisted settings file in user profile.

## Acceptance Criteria
- Menus navigable with arrow keys and Enter; focused items announced.
- Settings persist and are applied immediately, for example volume changes affect Audio Manager.
- HUD displays correct score and lives when running.

## Implementation Notes
- Follow Accessibility Rules from `architect.agent.md` strictly.
- Keep UI keyboard-first; any mouse input is optional.

## Related Tasks
- SA-001
- SA-002
- SA-003
