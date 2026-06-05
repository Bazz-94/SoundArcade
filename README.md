# Sound Arcade

Sound Arcade is an accessible, audio-first arcade game collection built in C# using Raylib-cs. It is designed to be fully playable by blind and low-vision players through spatial audio, text-to-speech, and keyboard controls alone, while remaining enjoyable for sighted players.

Rather than converting visual games to audio, Sound Arcade focuses on mini-games designed from the ground up for audio-only play. Visuals are supplementary and must never be required to understand or play a game.

## Current Game: RiverRun

RiverRun is an audio-first endless runner where the player survives as long as possible by switching lanes, jumping, and collecting points while obstacles rush toward them in a three-lane world.

The player automatically runs forward through left, center, and right lanes. Moment-to-moment play is about using keyboard input to dodge obstacles, jump over hazards, and collect score items while the difficulty steadily increases.

Audio is the primary interface:

- Spatial audio communicates lane position and distance for obstacles and collectibles.
- TTS announces score, pause state, game over, and significant events.
- Lane changes, jumps, pickups, warnings, and collisions each use distinct sound effects.

Controls:

- Left / Right arrows or A / D: change lanes
- Space: jump
- Escape or P: pause

The run ends when the player collides with an obstacle. Score increases by surviving longer and collecting pickups.

## Accessibility Rules

Every Sound Arcade game must pass these checks:

- Audio-only playable: all gameplay information is understandable through sound.
- Keyboard-only controllable: every action is available through keyboard input.
- State is always audible: score, position, pause, game over, and critical state changes are announced through audio or TTS.
- No vision-gated mechanics: visuals may help sighted players, but cannot provide exclusive gameplay information.

## Technical Approach

Sound Arcade uses Clean Architecture with lightweight DDD. A Platform Abstraction Layer decouples game and application logic from Raylib, allowing future platform implementations to be swapped in without rewriting game logic.

Each mini-game is a self-contained module registered by the Desktop host. The arcade shell treats games through a shared `IGame` contract.

## Solution Layout

- `Domain`: shared game contracts and domain primitives.
- `Domain.RiverRun`: RiverRun-specific domain module.
- `Application`: game orchestration, registry, game loop, scenes, and sessions.
- `Abstractions`: platform abstraction interfaces such as `IAudio`, `ITts`, `IInput`, `IRenderer`, and `IWindow`.
- `Infrastructure`: Raylib and platform-backed implementations.
- `Desktop`: executable host and composition root.
- `Tests`: minimal test project for CI and architecture checks.

## Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Build

```bash
dotnet build SoundArcade.slnx
```

### Run

```bash
dotnet run --project Desktop/Desktop.csproj
```

### Test

```bash
dotnet test SoundArcade.slnx
```

## Technology Stack

- .NET 8.0 / C# 12
- Raylib-cs 8.0.0
- System.Speech for the current Windows TTS stub

## Delivery Strategy

The project ships one mini-game at a time. The arcade shell and core architecture are built first, then mini-games are added incrementally with their own audio design and implementation brief.
