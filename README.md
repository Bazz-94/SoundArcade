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
```
SoundArcade/  → Audio, Input, UI systems
Domain/       → Game logic, Rules, Services  
Models/       → Entities, ValueObjects, Enums
```

**Key Rules:**
- Domain and Models must NOT reference Raylib (ensures reusability)
- Use dependency injection (makes code testable)
- Document audio cues (specify what sounds mean)
- Test Domain/Models with xUnit or NUnit

**Example Domain Service:**
```csharp
public class GameService
{
    private readonly IGameRuleValidator _validator;
    
    public GameService(IGameRuleValidator validator) => _validator = validator;
    
    public bool IsValidMove(Player player, Move move) 
        => _validator.Validate(player, move);
}
```



## 🤝 Contributing

1. Define data in **Models**
2. Implement logic in **Domain** (no Raylib)
3. Integrate in **SoundArcade** (Raylib integration)
4. Write unit tests for Domain & Models
5. Document audio cues and accessibility

## 🌟 Philosophy

**The best accessible games are simply good games that don't require a screen.**

Accessibility isn't a feature—it's the foundation. Every design prioritizes audio clarity, keyboard intuitiveness, and meaningful gameplay.

## 📜 License

[Your License Here]

## 📧 Contact

For questions, please open an issue on the project repository.
