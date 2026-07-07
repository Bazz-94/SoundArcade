# Sound Arcade Project Brief

## Project Vision
Sound Arcade is a multi-mini-game arcade collection built **for blind players** while remaining engaging for sighted players. All games are fully playable via audio feedback and keyboard controls alone. Visuals are optional and supplementary — never load-bearing for gameplay.

## Solution Structure
```
Application - orchestrates game flow, manages mini-game lifecycle, handles input and audio via PAL interfaces.
Domain - Contains the arcade shell and shared game logic (e.g., scoring, player state).
Domain.{Game} - Each mini-game has its own project for game-specific logic and state.
Abstractions - Defines interfaces for platform-specific implementations.
Infrastructure - Implements platform-specific functionality (e.g., Raylib).
```

## Dependency Rules
```
Application  -> Domain.{Game} -> Domain
Infrastructure -> Abstractions
Application -> Abstractions
```

## Platform Abstraction Layer (PAL)
The PAL is the contract between game logic and platform. Game logic **never** calls Raylib directly.

### Required interfaces in `SoundArcade.Abstractions`
| Interface | Responsibility |
|-----------|---------------|
| `IRenderer` | Draw simple 3D shapes (boxes, spheres, lines) |
| `IAudio` | 3D positional sound playback, distance attenuation |
| `IInput` | Keyboard event polling |
| `ITts` | Text-to-speech for menus and game events |
| `IWindow` | Window lifecycle, resolution, delta time |

### PAL Signature Rule
All interface method signatures must use **platform primitive types only** (e.g., `int`, `string`, `Vector3`). No Domain models may cross the PAL boundary.

## Accessibility Rules (Non-negotiable)
- Every game state change must emit an audio event or TTS announcement.
- No game mechanic may rely solely on visual information.
- Menus navigate with arrow keys and Enter; items announced via `ITts` on focus.
- Spatial position must be communicable via audio alone.

## Input Model
- Navigation: arrow keys + Enter + Escape.
- All input flows through `IInput` — never polled from Raylib directly in game code.

## Audio Model
- **3D positional audio**: all objects have a world position; engine handles panning/attenuation.
- **TTS**: Synthesized voice for menus, scores, and significant events.

## Graphic Model
- **3D Graphics**: Limited to primitive 3D shapes (boxes, spheres, lines, points).
- **Camera**: Simple orthographic camera with fixed viewport.
- **Audio First**: Visuals are additive only; removing them must not break gameplay.

## Collision Model
- Calculations performed in 3D world space (spheres, boxes).

## 2D vs 3D
- **Positioning**: All objects have a 3D world position (x, y, z). For 2D games, z defaults to 0.

## Mini-Game Architecture
Each mini-game is a self-contained module.

### Registration
Mini-games are registered in `SoundArcade.Desktop` at wiring time. The Application treats them uniformly via an interface.

### Per-game projects
Complex games live in `SoundArcade.Domain.{Game}`. Shared logic lives in `SoundArcade.Domain`.

## Decisions Deferred
- Android platform target implementation details (MAUI vs MonoGame vs others)