# Quick Start Development Prompt

## 🎮 Start Working on Snake Prototype

**Project**: Snake Prototype v0.5.0  
**Unity**: 6000.3.8f1 • **Architecture**: ServiceLocator + EventBus  
**Status**: Ready for Phase 6 development

---

## 🚀 Immediate Actions

### 1. Choose Your Task Type
```
[Unity Development]     → unity-developer skill
[Performance]          → unity-ecs-patterns skill  
[Game Design]          → game-design-theory skill
[UI/UX Design]         → game-ui-design skill
[Level Design]         → level-design skill
[Architecture]         → game-development skill
```

### 2. Describe Your Task
Be specific about what you want to accomplish:
- ✅ "Optimize snake movement performance"
- ✅ "Balance detection difficulty for better engagement"
- ✅ "Improve HUD clarity for competitive play"
- ✅ "Generate procedural levels with adaptive difficulty"

### 3. Automatic Skill Selection
The system will automatically:
- Select the appropriate AI skill based on your task
- Apply project standards from `.windsurf/rules/`
- Follow ServiceLocator + EventBus architecture
- Ensure quality gate compliance

---

## 📋 Quick Reference

### Core Architecture
```csharp
// Service Registration
ServiceLocator.Register<ISnakeManager>(snakeManager);

// Event Communication  
GameEventManager.Publish<SnakeMovedEvent>(new SnakeMovedEvent());

// Manager/View Separation
public class SnakeManager : IGameManager { /* Logic */ }
public class SnakeView : MonoBehaviour { /* Visual */ }
```

### Key Standards
- **Code Style**: Follow `coding-standards.md`
- **Architecture**: Use ServiceLocator + EventBus patterns
- **Testing**: Unit + Integration + Play mode tests
- **Documentation**: Update relevant docs

### Documentation Links
- **Project Overview**: `README.md`
- **Architecture Guide**: `Architecture.md`
- **Setup Instructions**: `Walkthrough_Setup.md`
- **AI Skills**: `AGENTS.md`
- **Standards**: `.windsurf/rules/`

---

## 🎯 Current Development Priorities

### Phase 6 Focus Areas
- Polishing already implemented mechanics, test for bugs, and refine gameplay
- Advanced AI behaviors
- Mobile platform optimization
- Performance profiling

### Known Issues
- Mobile performance optimization needed
- UI accessibility improvements
- Level generation balancing
- Detection system tuning

---

## 🛠️ Development Environment

### Prerequisites
- Unity 6000.3.8f1 or later
- URP 17.3.0+ package
- Target platform: Desktop (Windows/Mac/Linux)
- .NET Standard 2.1

### Project Structure
```
Assets/
├── Scripts/Core/           # ServiceLocator, GameEventManager
├── Scripts/Systems/        # Manager classes
├── Scripts/Views/          # View components  
├── ScriptableObjects/      # Configuration data
├── Prefabs/               # Game objects
├── Materials/             # Visual assets
└── Scenes/                # Level files
```

---

## 🤖 AI Agent Integration

This project uses specialized AI agents for different development aspects:

### How It Works
1. **Task Analysis**: System analyzes your request
2. **Skill Selection**: Automatic skill selection based on context
3. **Standards Application**: Applies project governance standards
4. **Quality Assurance**: Validates against quality gates
5. **Documentation**: Updates relevant documentation

### Example Interactions
```
You: "Optimize rendering performance for mobile"
→ unity-developer skill invoked
→ Applies mobile optimization patterns
→ Follows coding standards
→ Updates performance documentation

You: "Balance game difficulty for better engagement"  
→ game-design-theory skill invoked
→ Analyzes current balance
→ Applies MDA framework
→ Updates design documentation
```

---

## 📊 Project Status

### Completed Features ✅
- Core snake movement system
- ServiceLocator + EventBus architecture
- Detection system with visual feedback
- Procedural level generation
- Dynamic audio layering
- VFX system for events
- Complete documentation suite

### In Progress 🔄
- Performance optimization
- UI/UX improvements
- Mobile platform support

### Planned 📋
- Multiplayer functionality
- Advanced AI behaviors
- Additional platform support

---

## 🎮 Ready to Start!

**Simply describe what you want to work on** and the appropriate AI skill will be automatically selected to help you accomplish the task following all project standards and best practices.

*Example: "I want to optimize the snake movement system for better performance"*
