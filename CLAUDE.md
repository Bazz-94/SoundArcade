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

Sound Arcade is an **audio-first** arcade collection for blind and sighted players. Every game must be fully playable via keyboard and spatial audio/TTS alone; visuals are supplementary and never load-bearing. Any new mechanic must pass the Accessibility Test: audio-only playable, keyboard-only controllable, state always audible, no vision-gated mechanics.

## Architecture

Clean Architecture with lightweight DDD. Dependencies point inward toward Domain; game logic never references Raylib directly.

```
Application    → Domain.{Game} → Domain
Application    → Abstractions
Infrastructure → Abstractions
```

- **`Source/Domain`** — shared contracts and primitives: `IGame`, `GameIdentity`, scene/menu models, `SceneManager`.
- **`Source/Domain.RiverRun`** — the RiverRun mini-game module. Each mini-game gets its own `Domain.{Game}` project.
- **`Source/Abstractions`** — the Platform Abstraction Layer (PAL): `IAudio`, `ITts`, `IInput`, `IRenderer`, `IWindow`, `ISettingsStore`.
- **`Source/Infrastructure`** — Raylib-cs and Windows implementations of the PAL (`RaylibAudio`, `RaylibRenderer`, `TextToSpeech`, ...).
- **`Source/Application`** — executable host and composition root (`Program.cs`), `ArcadeShell` game loop, scene factory, menu scenes, DI registration.
- **`Source/Tests`** — xUnit tests. Domain logic must be unit tested; infrastructure may be untested.

### Key patterns

- **Mini-game registration**: each game implements `IGame` and registers via a DI extension (e.g. `services.AddRiverRun()` in `ServiceCollectionExtensions`); `GameRegistry` catalogs them and the shell drives them through scenes.
- **Domain event pattern**: gameplay sessions (e.g. `RiverRunSession`) do not call `IAudio`/`ITts` directly. Domain methods return `IReadOnlyList<RunEvent>` (`PlaySoundEvent`, `StopSoundEvent`, `TextToSpeechEvent`), which the scene layer translates into PAL calls. Keep new audio/TTS feedback in this event style.
- **Scenes**: `SceneManager` + `IScene` drive both menus (Application) and gameplay (`RiverRunScene`).

## C# Standards

- No `var` — always explicit types.
- Use `this.` for instance members; always use block bodies for methods.
- Stateful types: private setters plus explicit state-transition methods (e.g. `TakeDamage(int)`), never public setters.
- Define constants/enums instead of hardcoding values (see `RunConstants`); no magic strings.
- XML doc comments on all classes, methods, and properties — concise.
- Avoid redundant names (`GameLoop`, not `RiverRunGameLoop`) and single-use local variables.
- Prefer `foreach` over `for`.

## Agent Workflow & Artifacts

The `artifacts/` directory is shared memory for the project:

- `artifacts/standards.md` — project vision, architecture rules, and PAL contract.
- `artifacts/stories/SA-XXX-*.md` — Story Artifacts: medium-sized, independently testable units of work with acceptance criteria. Check existing stories before creating new ones.
