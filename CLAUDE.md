# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All paths are relative to the repo root. The solution is `Source/SoundArcade.slnx` (.NET 8, xUnit, central package versions in `Source/Directory.Packages.props`).

```bash
# Build
dotnet build Source/SoundArcade.slnx

# Run all tests
dotnet test Source/SoundArcade.slnx

# Run a single test class or test
dotnet test Source/Tests/Tests.csproj --filter "FullyQualifiedName~RiverRunSessionTests"
dotnet test Source/Tests/Tests.csproj --filter "FullyQualifiedName~RiverRunSessionTests.MethodName"

# Run the game (Application is the executable host / composition root)
dotnet run --project Source/Application/Application.csproj
```

Always build and run tests after every code change.

## Project Vision

Sound Arcade is an **audio-first** arcade collection for blind and sighted players: fully playable via keyboard and spatial audio/TTS alone; visuals never load-bearing. Full accessibility rules live in `artifacts/standards.md`.

## Architecture

Clean Architecture with lightweight DDD. Dependencies point inward toward Domain; game logic never references Raylib directly.

```
Application    → Domain.{Game} → Domain → Abstractions
Application    → Abstractions
Infrastructure → Abstractions
```

Every layer may depend on `Abstractions` (Domain references it directly, so `Domain.{Game}` gets it transitively); only Infrastructure implements it. Game logic talks to the platform exclusively through PAL interfaces.

- **`Source/Domain`** — shared contracts and primitives: `IGame`, `GameIdentity`, scene/menu models, `SceneManager`.
- **`Source/Domain.RiverRun`** — the RiverRun mini-game module. Each mini-game gets its own `Domain.{Game}` project.
- **`Source/Abstractions`** — the Platform Abstraction Layer (PAL): `IAudio`, `ITts`, `IInput`, `IRenderer`, `IWindow`, `ISettingsStore`.
- **`Source/Infrastructure`** — Raylib-cs and Windows implementations of the PAL (`RaylibAudio`, `RaylibRenderer`, `TextToSpeech`, ...).
- **`Source/Application`** — executable host and composition root (`Program.cs`), `ArcadeShell` game loop, scene factory, menu scenes, DI registration.
- **`Source/Tests`** — xUnit tests. Domain logic must be unit tested; infrastructure may be untested.

### Key patterns

- **Mini-game registration**: each game implements `IGame` and registers via a DI extension (e.g. `services.AddRiverRun()` in `ServiceCollectionExtensions`); `GameRegistry` catalogs them and the shell drives them through scenes.
- **Domain event pattern**: gameplay sessions (e.g. `RiverRunSession`) do not call `IAudio`/`ITts` directly. Domain methods return `IReadOnlyList<RunEvent>` (`PlaySoundEvent`, `StopSoundEvent`, `TextToSpeechEvent`), which the game coordinator (`Domain.RiverRun/Game/Game.cs`) translates into PAL calls. Keep new audio/TTS feedback in this event style — the session stays pure and unit-testable by asserting on returned events.
- **Scenes**: `SceneManager` + `IScene` drive both menus (Application) and gameplay (`RiverRunScene`). Gameplay scenes register their sounds (generated via `SoundProfile` or loaded from `Assets/`) with `IAudio` at construction.
- **Tuning vs constants**: per-game tuning lives in a settings record with defaulted constructor parameters (`RiverRunSettings`); fixed values (lane X positions, sound ids, spoken strings, volumes, pitches) live in a `RunConstants`-style static class. Tests construct settings with named arguments and a seeded `Random` for determinism.

## Standards

`artifacts/standards.md` is the single source of truth for project rules, design principles, and C# coding standards. Read it before writing or reviewing any code.

Additional conventions visible in the codebase but not in standards.md: `ImplicitUsings` is disabled (add explicit `using` directives, placed inside the namespace block), nullable reference types are enabled, and files use 2-space indentation.

## Agent Workflow & Artifacts

The `artifacts/` directory is shared memory for the project:

- `artifacts/stories/sa-XX-title.md` — stories: non-technical, independently testable units of work with acceptance criteria and dependencies. Check existing stories before creating new ones.
- `artifacts/implementation-plans/{storyid}.md` — task-level plans for implementing a story, with per-task status.

Workflow skills: `/start-planning-session` gathers context and breaks a feature into stories (`/write-stories`); `/implement` plans a story (`/create-implementation-plan`) and implements it task by task with user review.
