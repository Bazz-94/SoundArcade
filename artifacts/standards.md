# Standards

## Project

Sound Arcade is an audio-first arcade collection: every game must be fully playable by blind and sighted players via keyboard and spatial audio/TTS alone. Visuals are supplementary and never load-bearing.

- **Accessibility (non-negotiable)**: every state change emits an audio event or TTS announcement; no mechanic relies solely on visual information; menus navigate with arrow keys + Enter, announced via `ITts` on focus; spatial position must be communicable via audio alone.
- **Input**: navigation is arrow keys, Enter, Escape; all input flows through `IInput`, never polled directly from Raylib in game code.
- **Audio**: all objects have a 3D world position; the engine handles panning/attenuation for positional audio; TTS covers menus, scores, and significant events.
- **Graphics**: limited to primitive 3D shapes (boxes, spheres, lines, points) with a simple orthographic camera; visuals are additive only — removing them must never break gameplay.
- **Collision**: computed in 3D world space (spheres, boxes); 2D games simply default z to 0.
- **Mini-games**: each is a self-contained module, registered with the Application host at wiring time and driven uniformly through a shared interface. Complex games get their own `Domain.{Game}` project; shared logic stays in `Domain`.
- **Deferred**: Android platform target implementation (MAUI vs MonoGame vs others) is undecided.

## Design Principles

- **Clean Architecture**: dependencies point inward only, toward Domain (`Application → Domain.{Game} → Domain`, `Application/Infrastructure → Abstractions`). Business logic, orchestration, and infrastructure stay clearly separated.
- **Platform Abstraction Layer (PAL)**: the contract between game logic and platform, defined in `Abstractions`. Game logic never calls Raylib directly, only `IRenderer`, `IAudio`, `IInput`, `ITts`, `IWindow`. All PAL method signatures use platform primitive types only (`int`, `string`, `Vector3`, etc.) — no Domain models cross the boundary.
- **DDD encapsulation**: domain entities manage their own state through explicit methods, never public setters. Business rules and invariants live in the Domain layer; use cases live in the Application layer.

## C# Standards

- No `var` — always explicit types.
- Use `this.` for instance members; always use block bodies for methods.
- Stateful types expose private setters plus explicit state-transition methods (e.g. a `Player` with a private-set `Health` mutated only via `TakeDamage`/`Heal`, never by setting the field directly).
- Avoid single-use local variables — inline the expression instead.
- Define constants or enums for meaningful values instead of hardcoding them; never hardcode string values.
- Avoid redundant words in names (`GameLoop`, not `RiverRunGameLoop`, when context is already clear).
- Prefer `foreach` over `for`.
- Provide concise XML doc comments on all classes, methods, and properties.
- Unit test all Domain logic; Application and Infrastructure code may be untested or covered by integration tests only.
