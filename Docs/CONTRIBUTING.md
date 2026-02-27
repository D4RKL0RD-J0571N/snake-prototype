# Contributing Guidelines - Snake Prototype

Thank you for your interest in contributing to the Snake Prototype project! This document provides comprehensive guidelines for contributing code, documentation, and other improvements.

## 🚀 Getting Started

### Prerequisites

- **Unity 6000.3.8f1** or later
- **Git** for version control
- **C#** programming knowledge
- **Unity development experience** (recommended)
- **Understanding of URP** (Universal Render Pipeline)

### Development Environment Setup

1. **Fork the Repository**
   ```bash
   git clone https://github.com/your-username/snake-prototype.git
   cd snake-prototype
   ```

2. **Create a Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Set Up Unity Project**
   - Open the project in Unity 6000.3.8f1
   - Verify all packages are installed
   - Test that the project builds successfully

4. **Run Tests**
   - Open `Window > General > Test Runner`
   - Run all tests to ensure baseline functionality

## 📝 Code Style Standards

### C# Coding Standards

We follow Microsoft's C# coding conventions with Unity-specific adaptations:

#### Naming Conventions
```csharp
// Public fields and properties - PascalCase
public class SnakeManager : IGameService
{
    public float MoveInterval { get; private set; }
    public Vector2Int HeadPosition => _bodyParts[0];
    
    // Private fields - camelCase with underscore prefix
    private List<Vector2Int> _bodyParts;
    private SnakeConfiguration _config;
    
    // Constants - PascalCase
    private const float DEFAULT_MOVE_INTERVAL = 0.15f;
    
    // Static readonly - PascalCase
    private static readonly Vector2Int DEFAULT_DIRECTION = Vector2Int.right;
}
```

#### Method Naming
```csharp
// Public methods - PascalCase, descriptive verbs
public void InitializeService()
public void ProcessMovement()
public bool CanMoveToPosition(Vector2Int position)

// Private methods - camelCase, often verbs
private void updateSnakePosition()
private void handleCollision()
private bool isValidDirection(Vector2Int direction)
```

#### Class and Interface Naming
```csharp
// Classes - PascalCase, nouns
public class SnakeManager
public class GridConfiguration
public class DetectionSystem

// Interfaces - PascalCase, prefix with 'I'
public interface IGameService
public interface IDetectionSource
public interface ILevelGenerator
```

#### File Organization
```csharp
// File: SnakeManager.cs
using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Snake
{
    /// <summary>
    /// Manages snake movement, growth, and collision detection.
    /// Implements the core gameplay logic for the snake entity.
    /// </summary>
    public class SnakeManager : IGameService
    {
        #region Fields
        
        #endregion
        
        #region Properties
        
        #endregion
        
        #region Initialization
        
        #endregion
        
        #region Public Methods
        
        #endregion
        
        #region Private Methods
        
        #endregion
    }
}
```

### Unity-Specific Patterns

#### MonoBehaviour Usage
```csharp
// Use MonoBehaviour only for components that need to be in the scene
public class SnakeView : MonoBehaviour
{
    [SerializeField] private GameObject _segmentPrefab;
    [SerializeField] private float _segmentSpacing = 1.0f;
    
    private void Awake()
    {
        // Initialize components
    }
    
    private void Update()
    {
        // Only use Update for continuous visual updates
    }
}

// Use plain C# classes for logic managers
public class SnakeManager : IGameService
{
    // No MonoBehaviour - pure logic
}
```

#### Service Registration
```csharp
// In GameBootstrapper.cs
private void RegisterServices()
{
    // Register in dependency order
    ServiceLocator.Register(new InputManager(_inputConfig));
    ServiceLocator.Register(new GridManager(_gridConfig));
    ServiceLocator.Register(new SnakeManager(_snakeConfig));
    ServiceLocator.Register(new DetectionManager(_detectionConfig));
}
```

#### Event System Usage
```csharp
// Publishing events
GameEventManager.Publish(new SnakeMovedEvent(HeadPosition, _currentDirection));

// Listening to events
private void OnEnable()
{
    GameEventManager.AddListener<ScoreChangedEvent>(OnScoreChanged);
}

private void OnDisable()
{
    // Note: In this prototype, we rely on scene reload clearing static state
    // In production, implement proper listener removal
}

private void OnScoreChanged(ScoreChangedEvent evt)
{
    UpdateScoreDisplay(evt.NewScore);
}
```

## 🏗️ Architecture Compliance

### ServiceLocator Pattern

All game services must:
1. Implement `IGameService` interface
2. Be registered in `GameBootstrapper`
3. Use `ServiceLocator.Get<T>()` for dependencies
4. Follow initialization order: Core → Systems → UI

```csharp
public class YourManager : IGameService
{
    public void Initialize()
    {
        // Initialize service
        var gridManager = ServiceLocator.Get<GridManager>();
        // Use dependency
    }
    
    public void Shutdown()
    {
        // Clean up resources
    }
}
```

### Manager/View Separation

Keep logic and visualization separate:

```csharp
// Logic Manager (no Unity dependencies)
public class SnakeManager : IGameService
{
    public void MoveSnake(Vector2Int direction)
    {
        // Pure logic, no GameObject manipulation
    }
}

// View Component (Unity-specific)
public class SnakeView : MonoBehaviour
{
    private SnakeManager _snakeManager;
    
    private void Update()
    {
        // Visual representation only
        UpdateVisuals(_snakeManager.BodyParts);
    }
}
```

### Event-Driven Communication

Use events for cross-system communication:

```csharp
// Define events in GameEvents.cs
public class EnergyCollectedEvent : GameEvent
{
    public readonly Vector2Int Position;
    public readonly int Points;
    
    public EnergyCollectedEvent(Vector2Int position, int points)
    {
        Position = position;
        Points = points;
    }
}

// Publish events
GameEventManager.Publish(new EnergyCollectedEvent(position, points));

// Listen to events
GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
```

## 🧪 Testing Requirements

### Unit Testing

All core logic must have unit tests:

```csharp
// Example: SnakeManagerTests.cs
[Test]
public void MoveSnake_ValidDirection_UpdatesPosition()
{
    // Arrange
    var config = ScriptableObject.CreateInstance<SnakeConfiguration>();
    var snakeManager = new SnakeManager(config);
    
    // Act
    snakeManager.SetDirection(Vector2Int.right);
    snakeManager.Update(0.2f); // Simulate time passing
    
    // Assert
    Assert.AreEqual(Vector2Int.right, snakeManager.HeadPosition);
}
```

### Integration Testing

Test system interactions:

```csharp
[Test]
public void SnakeEnergyCollection_UpdatesScore()
{
    // Arrange
    SetupGameSystems();
    var initialScore = ServiceLocator.Get<ScoreManager>().Score;
    
    // Act
    SimulateEnergyCollection();
    
    // Assert
    var finalScore = ServiceLocator.Get<ScoreManager>().Score;
    Assert.IsTrue(finalScore > initialScore);
}
```

### Test Organization

```
Assets/Tests/
├── EditMode/
│   ├── Core/
│   │   ├── ServiceLocatorTests.cs
│   │   └── GameEventManagerTests.cs
│   ├── Systems/
│   │   ├── SnakeManagerTests.cs
│   │   ├── GridManagerTests.cs
│   │   └── DetectionManagerTests.cs
│   └── Utils/
│       └── MathHelperTests.cs
└── PlayMode/
    ├── Integration/
    │   ├── GameFlowTests.cs
    │   └── SystemInteractionTests.cs
    └── Performance/
        └── PerformanceTests.cs
```

## 📋 Pull Request Process

### Before Submitting

1. **Code Quality**
   - [ ] Code follows style guidelines
   - [ ] No compiler warnings
   - [ ] Methods and classes are documented
   - [ ] No unused code or debugging statements

2. **Testing**
   - [ ] All tests pass
   - [ ] New features have tests
   - [ ] Code coverage is maintained or improved
   - [ ] Performance impact is minimal

3. **Functionality**
   - [ ] Feature works as intended
   - [ ] No regressions in existing features
   - [ ] Edge cases are handled
   - [ ] Error conditions are graceful

### Pull Request Template

```markdown
## Description
Brief description of changes and their purpose.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Performance impact assessed

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
- [ ] Ready for review

## Screenshots (if applicable)
Add screenshots to explain changes.

## Additional Notes
Any additional context or considerations.
```

### Review Process

1. **Automated Checks**
   - Code formatting validation
   - Test suite execution
   - Build verification
   - Static analysis

2. **Peer Review**
   - At least one team member approval
   - Architecture compliance check
   - Performance impact assessment
   - Documentation review

3. **Integration Testing**
   - Full game testing
   - Multi-platform verification
   - Performance benchmarking
   - User experience validation

## 📚 Documentation Standards

### Code Documentation

All public APIs must have XML documentation:

```csharp
/// <summary>
/// Manages snake movement, growth, and collision detection.
/// This class handles the core gameplay logic for the snake entity.
/// </summary>
/// <remarks>
/// The snake moves on a 2D grid but is rendered in 3D space.
/// Movement is constrained to cardinal directions (up, down, left, right).
/// </remarks>
public class SnakeManager : IGameService
{
    /// <summary>
    /// Gets the current head position of the snake.
    /// </summary>
    /// <value>
    /// A Vector2Int representing the grid position of the snake's head.
    /// Returns Vector2Int.zero if the snake has no body parts.
    /// </value>
    public Vector2Int HeadPosition => _bodyParts.Count > 0 ? _bodyParts[0] : Vector2Int.zero;
    
    /// <summary>
    /// Moves the snake in the specified direction if valid.
    /// </summary>
    /// <param name="direction">The direction to move (must be cardinal direction).</param>
    /// <returns>True if movement was successful, false if invalid.</returns>
    /// <exception cref="System.ArgumentException">Thrown when direction is not a cardinal direction.</exception>
    public bool TryMove(Vector2Int direction)
    {
        // Implementation
    }
}
```

### Documentation Updates

When adding features:
1. Update relevant README sections
2. Add API documentation to code
3. Update GDD if gameplay changes
4. Add troubleshooting entries to FAQ
5. Update architecture documentation

## 🎯 Feature Development Guidelines

### Small Features

1. **Planning**
   - Create issue in GitHub
   - Define acceptance criteria
   - Estimate effort
   - Assign to sprint

2. **Implementation**
   - Create feature branch
   - Implement core functionality
   - Add tests
   - Update documentation

3. **Review**
   - Self-review code
   - Request peer review
   - Address feedback
   - Merge to main

### Large Features

1. **Design Phase**
   - Technical design document
   - Architecture impact assessment
   - Risk analysis
   - Resource planning

2. **Implementation Phase**
   - Break into smaller tasks
   - Incremental development
   - Regular integration
   - Continuous testing

3. **Stabilization Phase**
   - Performance optimization
   - Bug fixing
   - Documentation
   - User acceptance testing

## 🚫 What Not to Contribute

### Out of Scope

- Major architectural changes without discussion
- Breaking changes to public APIs
- Dependencies on external packages
- Platform-specific features
- Experimental or prototype code

### Quality Standards

- Code that doesn't follow style guidelines
- Features without tests
- Breaking existing functionality
- Performance regressions
- Security vulnerabilities

## 🏆 Recognition

### Contributor Recognition

- Contributors listed in README
- Special thanks in release notes
- Contributor badges on GitHub
- Invitation to core team for significant contributions

### Community Guidelines

- Be respectful and constructive
- Welcome newcomers and help them learn
- Focus on what is best for the community
- Show empathy towards other community members

## 📞 Getting Help

### Resources

- **Documentation**: Check project docs first
- **GitHub Issues**: Search existing issues before creating new ones
- **Discord/Slack**: Join community discussions
- **Code Reviews**: Ask for help during review process

### Contact Information

- **Project Lead**: [Lead Developer Email]
- **Technical Questions**: [Tech Lead Email]
- **Community Issues**: [Community Manager Email]

---

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the same license as the project (MIT License).

---

**Thank you for contributing to Snake Prototype!** Your contributions help make this project better for everyone.

**Last Updated**: 2026-02-20  
**Review Date**: 2026-03-01  
**Maintainer**: Development Team
