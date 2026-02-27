# SNEK: Labyrinth Protocol

*A stealth-inspired evolution of Snake set in an industrial labyrinth, balancing growth and visibility.*

![Unity Version](https://img.shields.io/badge/Unity-6000.3.8f1-blue)
![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP-green)
![License](https://img.shields.io/badge/License-MIT-yellow)
![Version](https://img.shields.io/badge/Version-v0.5.0-orange)

## 🚀 Quick Start

**New to the project?** Start here:

1. **Read the Development Prompt**: [DEVELOPMENT_PROMPT.md](DEVELOPMENT_PROMPT.md) - Complete project overview
2. **Quick Start Guide**: [QUICK_START.md](QUICK_START.md) - Immediate development actions
3. **Project Setup**: [Walkthrough_Setup.md](Walkthrough_Setup.md) - Detailed setup instructions

**Choose your development path:**
- Unity-specific tasks → `unity-developer` skill
- Performance optimization → `unity-ecs-patterns` skill  
- Game design/balance → `game-design-theory` skill
- UI/UX improvements → `game-ui-design` skill
- Level generation → `level-design` skill
- High-level architecture → `game-development` skill

## Overview

**SNEK: Labyrinth Protocol** is a mechanical evolution of the classic Snake game built in Unity 6000.3.8f1 using the Universal Render Pipeline (URP). Players control a mechanical serpent navigating an industrial labyrinth, balancing traditional growth mechanics with stealth and detection tension. As the snake consumes energy cores, it grows longer, increasing capacity but also vulnerability to detection systems.

### Key Features

- **ServiceLocator + EventBus Architecture**: Clean, decoupled systems using the "Jostin Architecture"
- **Procedural Level Generation**: Seed-based labyrinth generation with configurable themes
- **Advanced Detection System**: Multi-source detection with weighted aggregation and visual feedback
- **Dynamic Audio Layering**: 3-layer adaptive music system with real-time modulation
- **Premium Visual Effects**: Custom shaders, procedural materials, and cinematic transitions
- **UI Toolkit Integration**: Modern, responsive UI with USS-based theming

## Quick Start

### Prerequisites

- **Unity 6000.3.8f1** or later
- **Universal Render Pipeline (URP) 17.3.0+**
- Windows, macOS, or Linux (Desktop target)

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-username/snake-prototype.git
   cd snake-prototype
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and navigate to the cloned directory
   - Unity will automatically detect the project version

3. **Verify Setup**
   - Open `Assets/Scenes/Level_01.unity`
   - Ensure all ScriptableObject configurations are assigned
   - Press Play to test the game

### Basic Controls

| Input | Action |
|-------|--------|
| **W/A/S/D** or **Arrow Keys** | Move snake in grid directions |
| **Space** | Pause/Resume game |
| **R** | Restart current level |
| **Escape** | Return to main menu |

## Gameplay

### Core Loop

1. **Exploration**: Navigate the grid-based industrial labyrinth
2. **Collection**: Consume Energy Cores to grow and increase score
3. **Avoidance**: Maneuver to avoid detection and self-collision
4. **Survival**: Manage length and speed as complexity increases

### Detection System

The game features a sophisticated detection system where multiple sources (cameras, guards) contribute to a global detection level. When detection exceeds the stress threshold, the game triggers a "Game Over" state.

### Visual Features

- **Dynamic Grid**: Reactive lighting that pulses with detection intensity
- **Procedural Snake**: Length-based gradients and dissolve effects
- **Energy Cores**: Glowing collectibles with pulse animations
- **Screen Effects**: CRT scanlines, vignette, and chromatic aberration on detection

## Architecture

The project uses a **ServiceLocator + EventBus** architecture ("Jostin Architecture") for clean separation of concerns:

### Core Systems

- **ServiceLocator**: Central dependency injection container
- **GameEventManager**: Typed event bus for decoupled communication
- **Manager/View Pattern**: Logic separated from visualization

### Key Managers

- **SnakeManager**: Snake movement, growth, and collision logic
- **GridManager**: 2D grid data and wall management
- **DetectionManager**: Multi-source detection aggregation
- **UIManager**: HUD updates and overlay management
- **AudioManager**: Dynamic music layering and SFX
- **VFXManager**: Particle effects and visual polish

## Screenshots & Media

*Note: Screenshots will be added as the project progresses*

- [Gameplay Screenshot] - Main gameplay showing snake and detection system
- [UI Screenshot] - HUD with score and detection meter
- [VFX Screenshot] - Visual effects and particle systems

## Build Requirements

### Minimum Requirements

- **Unity**: 6000.3.8f1
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Target Platform**: Desktop (Windows/Mac/Linux)
- **.NET**: .NET Standard 2.1

### Dependencies

- `com.unity.render-pipelines.universal`: 17.3.0
- `com.unity.inputsystem`: 1.18.0
- `com.unity.test-framework`: 1.6.0
- `com.unity.ugui`: 2.0.0

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on:

- Code style standards
- Pull request process
- Testing requirements
- Architecture compliance

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Follow the [Architecture Compliance](.windsurf/rules/architecture-compliance.md) guidelines
4. Submit a pull request with proper documentation

## Documentation

- [Game Design Document](GDD.md) - Complete game design specifications
- [Architecture Overview](Architecture.md) - Technical architecture details
- [Setup Walkthrough](Walkthrough_Setup.md) - Step-by-step setup guide
- [API Documentation](TODO.md) - Code documentation (in progress)
- [FAQ](FAQ.md) - Frequently asked questions
- [Roadmap](ROADMAP.md) - Development roadmap and future plans

## Testing

The project includes comprehensive test coverage:

- **Edit Mode Tests**: Core logic testing for managers
- **Play Mode Tests**: Integration testing for game systems
- **Test Coverage**: GridManager, SnakeManager, ScoreManager, Detection system

Run tests via `Window > General > Test Runner` in Unity.

## Performance

### Target Specifications

- **Frame Rate**: 60 FPS on mid-range hardware
- **Resolution**: 1920x1080 (scalable)
- **Memory**: < 200MB runtime memory usage

### Optimization Features

- Object pooling for visual effects
- Efficient grid-based collision detection
- Optimized custom shaders
- UI Toolkit for performant UI rendering

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Unity Technologies for the Unity Engine and URP
- The open-source community for various tools and utilities
- Contributors and testers who helped shape the game

## Contact

- **Project Lead**: Jostin Lopez
- **Repository**: https://github.com/your-username/snake-prototype
- **Issues**: https://github.com/your-username/snake-prototype/issues

---

**Version**: v0.5.0  
**Last Updated**: 2026-02-20  
**Status**: Active Development - Phase 5 Complete
