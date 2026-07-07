# Sound Arcade

Sound Arcade is an accessible, audio-first arcade game collection built in C# using Raylib-cs. It is designed to be fully playable by blind and low-vision players through spatial audio, text-to-speech, and keyboard controls alone, while remaining enjoyable for sighted players.

Rather than converting visual games to audio, Sound Arcade focuses on mini-games designed from the ground up for audio-only play. Visuals are supplementary and must never be required to understand or play a game.

## Accessibility Rules

Every Sound Arcade game must pass these checks:

- Audio-only playable: all gameplay information is understandable through sound.
- Keyboard-only controllable: every action is available through keyboard input.
- State is always audible: score, position, pause, game over, and critical state changes are announced through audio or TTS.
- No vision-gated mechanics: visuals may help sighted players, but cannot provide exclusive gameplay information.

## Technical Approach

Sound Arcade uses Clean Architecture with lightweight DDD. A Platform Abstraction Layer decouples game and application logic from Raylib, allowing future platform implementations to be swapped in without rewriting game logic.

Each mini-game is a self-contained module registered by the Application host. The arcade shell treats games through a shared `IGame` contract.

## Solution Layout

- `Domain`: shared game contracts and domain primitives.
- `Domain.{mini-game}`: mini game specific domain module.
- `Abstractions`: platform abstraction interfaces such as `IAudio`, `ITts`, `IInput`, `IRenderer`, and `IWindow`.
- `Infrastructure`: Raylib and platform-backed implementations.
- `Application`: executable host and composition root, game orchestration, registry, game loop, scenes, and sessions.
- `Tests`: xUnit test project; Domain logic must be unit tested.

## Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Build

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

## Technology Stack

- .NET 8.0 / C# 12
- Raylib-cs 8.0.0
- System.Speech for the current Windows TTS stub

## Delivery Strategy

The project ships one mini-game at a time. The arcade shell and core architecture are built first, then mini-games are added incrementally with their own audio design and implementation brief.
