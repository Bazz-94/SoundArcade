# SA-006: Raylib Infrastructure: Window and Renderer implementation

## Description
Implement the concrete Raylib-backed versions of `IWindow` and `IRenderer` within the Infrastructure project. This provides the foundational "host" for the application, handling the game window lifecycle, timing (delta time), and the high-contrast primitive rendering required for sighted-player support.

## Scope
Includes:
- `RaylibWindow` implementation: Initialize/Close window, manage frame rate, and expose `DeltaTime`.
- `RaylibRenderer` implementation: Support for 3D primitives (DrawCube, DrawSphere, DrawLine) as defined in `IRenderer`.
- Integration of the Raylib camera (orthographic) to provide the fixed viewport.

Excludes:
- Audio/TTS implementation (covered in SA-002).
- Input implementation (covered in SA-003).
- Complex shaders or textures.

## Metadata
- System area: Infrastructure
- Type: Graphics
- Dependencies: SA-000
- Complexity: Medium
- Status: Not Started

## Inputs
- `IWindow` and `IRenderer` interfaces from `SoundArcade.Abstractions`.
- Raylib-cs NuGet package.

## Outputs
- `RaylibWindow.cs` in `SoundArcade.Infrastructure`.
- `RaylibRenderer.cs` in `SoundArcade.Infrastructure`.
- Wiring in the Desktop project to register these types in the DI container.

## Acceptance Criteria
- Running the Desktop project successfully opens a Raylib window.
- The `IRenderer` can draw basic 3D shapes to the screen using world-space coordinates.
- Window lifecycle (opening, closing, resizing) works correctly without crashing.
- Time-scaling/DeltaTime is consistent with Raylib's internal clock.

## Implementation Notes
- Ensure `RaylibWindow` correctly handles the `ShouldClose` check for the main loop.
- Use a high-contrast default color palette for primitives as per the Graphics Model.
- Keep the rendering logic strictly tied to the primitive types allowed in the PAL (Box, Sphere, Line, Point).
- Follow the PAL signature rule: use `System.Numerics.Vector3` and primitive types only.

## Related Tasks
- SA-001
- SA-004