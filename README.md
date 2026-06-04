# 🎮 Sound Arcade

Sound Arcade is a **completely audio-first game collection** designed for blind and low-vision players, while remaining genuinely fun for sighted players. All games are playable entirely through keyboard input and audio feedback.

Rather than converting visual games to audio, Sound Arcade **invents new game mechanics built from the ground up for audio-only play**.

## 🎯 Quick Start

### Prerequisites
- **.NET 8.0 SDK** or later
- Visual Studio 2022 or VS Code

### Building & Running

```bash
# Build
dotnet build

# Run
dotnet run --project SoundArcade/SoundArcade.csproj
```

## 🏗️ Architecture

Sound Arcade follows **domain-driven development** with three layers:

- **Audio-First**: All essential information conveyed through sound  
- **Keyboard-Only**: Arrow keys, WASD, Space, Enter, Escape, number keys  
- **Accessible**: WCAG 2.1 Level AA compliance, customizable audio  
- **Game Mechanics**: Rhythm, memory, navigation, strategy, reaction, audio creation

## 🛠️ Technology Stack

- **.NET 8.0 / C# 12**  
- **Raylib-cs 7.0.2** (graphics & audio)  
- **Cross-platform**: Windows, macOS, Linux

## 📝 Development Guidelines

**Code Organization:**
Models          →  nothing
Domain          →  Models only
Abstractions    →  nothing (primitive types only)
Application     →  Domain + Models + Abstractions
Raylib          →  Abstractions only  ← no longer needs Models
Desktop         →  Everything (wires it all together)


