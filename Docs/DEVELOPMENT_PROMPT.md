# Snake Prototype Development Prompt

## Project Overview
You are now working on the **Snake Prototype v0.5.0** project, a stealth-inspired evolution of Snake set in an industrial labyrinth. The project uses Unity 6000.3.8f1 with Universal Render Pipeline (URP) and implements a ServiceLocator + EventBus architecture ("Jostin Architecture").

## Current Project State
- **Version**: v0.5.0 (Phase 5 Complete)
- **Architecture**: ServiceLocator + EventBus pattern
- **Unity Version**: 6000.3.8f1
- **Render Pipeline**: URP 17.3.0+
- **Status**: Active development, ready for Phase 6 initiation

## Available AI Skills
The project has integrated AI agents with specialized skills:

### Core Development Skills
- **unity-developer**: Unity-specific implementation and optimization
- **unity-ecs-patterns**: High-performance ECS systems
- **game-development**: High-level orchestration and platform guidance

### Design Skills  
- **game-design-theory**: Game balance, MDA framework, player psychology
- **game-ui-design**: Nintendo UI clarity, accessibility, competitive gaming
- **level-design**: Procedural generation, spatial design, environmental storytelling

## Documentation References
All documentation is available in the `Docs/` directory:
- **README.md**: Project overview and quick start
- **Architecture.md**: ServiceLocator + EventBus architecture details
- **Walkthrough_Setup.md**: Complete setup guide
- **AGENTS.md**: AI agent system overview
- **AGENTS.*.md**: Specialized skill documentation
- **.windsurf/rules/**: Governance and standards

## Governance Standards
Follow the established standards in `.windsurf/rules/`:
- **coding-standards.md**: C# code style and patterns
- **architecture-patterns.md**: ServiceLocator + EventBus implementation
- **testing-standards.md**: Testing framework and coverage
- **documentation-standards.md**: Documentation quality requirements

## Current Development Focus
The project is ready to begin **Phase 6** development, focusing on:
- Multiplayer architecture and networking
- Advanced AI behaviors
- Platform expansion strategies
- Performance optimization

## Getting Started Instructions

### 1. Project Understanding
First, review the core documentation:
- Read `README.md` for project overview
- Study `Architecture.md` for ServiceLocator + EventBus patterns
- Review `Walkthrough_Setup.md` for project setup

### 2. Skill Selection
Based on your task, the appropriate AI skill will be automatically selected:
- Unity-specific tasks → `unity-developer`
- Performance optimization → `unity-ecs-patterns`
- Game balance/design → `game-design-theory`
- UI/UX improvements → `game-ui-design`
- Level generation → `level-design`
- High-level architecture → `game-development`

### 3. Development Workflow
All tasks follow this workflow:
1. **Task Analysis**: Understand requirements and context
2. **Skill Selection**: Automatic selection based on task content
3. **Standards Compliance**: Follow `.windsurf/rules/` standards
4. **Quality Gates**: Pass automated quality checks
5. **Documentation**: Update relevant documentation

### 4. Quality Assurance
All changes must pass:
- Code quality standards (`coding-standards.md`)
- Architecture compliance (`architecture-patterns.md`)
- Testing requirements (`testing-standards.md`)
- Documentation standards (`documentation-standards.md`)

## Example Task Prompts

### For Unity Development
"Optimize the snake movement system for better performance using Unity ECS patterns"

### For Game Design
"Balance the detection system difficulty to create better player engagement"

### For UI/UX
"Improve the HUD clarity for competitive gaming following Nintendo UI principles"

### For Level Design
"Implement procedural level generation with adaptive difficulty progression"

### For Performance
"Optimize rendering performance for mobile platforms using URP settings"

## Project Context
The game features:
- **Mechanical serpent** navigating industrial labyrinth
- **Stealth mechanics** with detection systems
- **Growth mechanics** balanced against visibility
- **Procedural level generation** with seed-based layouts
- **Dynamic audio system** with adaptive music layers
- **Advanced VFX system** for energy collection and events

## Architecture Patterns
Follow the established ServiceLocator + EventBus patterns:
- Use `ServiceLocator.Register<T>()` for service registration
- Use `GameEventManager.Publish<T>()` for event communication
- Maintain Manager/View separation
- Use ScriptableObject for configuration data

## Testing Strategy
- Unit tests for core logic
- Integration tests for system interactions
- Play mode tests for gameplay mechanics
- Performance tests for optimization

## Current Challenges
- Multiplayer implementation planning
- Advanced AI behavior systems
- Mobile platform optimization
- Performance profiling and optimization

## Next Steps
Based on your specific task, the appropriate AI skill will be invoked automatically. Each skill has detailed documentation in `AGENTS.{skill}.md` files.

---

**Ready to begin development!** 🚀

*This prompt integrates all project documentation, governance standards, and AI agent capabilities for streamlined development workflows.*
