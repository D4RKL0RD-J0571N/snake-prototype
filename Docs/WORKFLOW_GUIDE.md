# Development Workflow Guide

## 🎯 Getting Started with Snake Prototype Development

This guide provides the complete workflow for starting development on the Snake Prototype project with integrated AI agent system.

---

## 📋 Prerequisites Checklist

### Environment Setup ✅
- [ ] Unity 6000.3.8f1 or later installed
- [ ] URP 17.3.0+ package installed
- [ ] Project cloned and opened in Unity
- [ ] All required packages installed (Input System, Test Framework, etc.)

### Documentation Review ✅
- [ ] Read [README.md](README.md) for project overview
- [ ] Review [Architecture.md](Architecture.md) for ServiceLocator + EventBus patterns
- [ ] Study [DEVELOPMENT_PROMPT.md](DEVELOPMENT_PROMPT.md) for AI integration
- [ ] Check [QUICK_START.md](QUICK_START.md) for immediate actions

### Standards Understanding ✅
- [ ] Review [coding-standards.md](../.windsurf/rules/coding-standards.md)
- [ ] Understand [architecture-patterns.md](../.windsurf/rules/architecture-patterns.md)
- [ ] Check [testing-standards.md](../.windsurf/rules/testing-standards.md)
- [ ] Read [ai-integration.md](../.windsurf/rules/ai-integration.md)

---

## 🚀 Development Workflow

### Step 1: Task Definition
1. **Identify Your Goal**: What do you want to accomplish?
2. **Select Task Type**: Choose from development categories
3. **Define Scope**: Be specific about requirements and constraints

### Step 2: AI Skill Selection
The system automatically selects appropriate skill based on your task:

| Task Type | AI Skill | Documentation |
|------------|------------|----------------|
| Unity Engine Issues | `unity-developer` | [AGENTS.unity-developer.md](AGENTS.unity-developer.md) |
| Performance Optimization | `unity-ecs-patterns` | [AGENTS.unity-ecs-patterns.md](AGENTS.unity-ecs-patterns.md) |
| Game Balance/Design | `game-design-theory` | [AGENTS.game-design-theory.md](AGENTS.game-design-theory.md) |
| UI/UX Improvements | `game-ui-design` | [AGENTS.game-ui-design.md](AGENTS.game-ui-design.md) |
| Level Generation | `level-design` | [AGENTS.level-design.md](AGENTS.level-design.md) |
| Architecture Decisions | `game-development` | [AGENTS.game-development.md](AGENTS.game-development.md) |

### Step 3: Development Process
1. **Standards Compliance**: Follow `.windsurf/rules/` standards
2. **Architecture Patterns**: Use ServiceLocator + EventBus
3. **Quality Gates**: Pass automated quality checks
4. **Documentation**: Update relevant documentation
5. **Testing**: Follow testing standards

### Step 4: Quality Assurance
- **Code Review**: Automated review against standards
- **Architecture Check**: ServiceLocator + EventBus compliance
- **Testing**: Unit, Integration, Play mode tests
- **Documentation**: Updates with cross-references

---

## 🛠️ Common Development Tasks

### Performance Optimization
```
Task: "Optimize snake movement for mobile"
→ Skill: unity-ecs-patterns
→ Standards: coding-standards.md, testing-standards.md
→ Quality Gates: Performance benchmarks, memory usage
→ Documentation: Update AGENTS.unity-ecs-patterns.md
```

### Game Balance
```
Task: "Balance detection difficulty for better engagement"
→ Skill: game-design-theory  
→ Standards: MDA framework, player psychology
→ Quality Gates: Balance metrics, player retention
→ Documentation: Update AGENTS.game-design-theory.md
```

### UI/UX Improvements
```
Task: "Improve HUD clarity for competitive play"
→ Skill: game-ui-design
→ Standards: Nintendo clarity, WCAG compliance
→ Quality Gates: Accessibility, readability
→ Documentation: Update AGENTS.game-ui-design.md
```

### Level Generation
```
Task: "Implement procedural levels with adaptive difficulty"
→ Skill: level-design
→ Standards: Spatial design, player flow
→ Quality Gates: Level balance, generation algorithms
→ Documentation: Update AGENTS.level-design.md
```

---

## 📊 Project Status

### Current Version: v0.5.0
**Phase 5 Complete** - Ready for Phase 6 development

### Completed Systems ✅
- Core snake movement and growth mechanics
- ServiceLocator + EventBus architecture
- Detection system with visual feedback
- Procedural level generation
- Dynamic audio layering system
- VFX system for game events
- Complete documentation suite

### Development Focus 🎯
- Multiplayer architecture planning
- Advanced AI behaviors
- Mobile platform optimization
- Performance profiling and optimization

---

## 🔧 Development Tools

### AI Agent System
- **Automatic Skill Selection**: Based on task context
- **Quality Gates**: Automated standards compliance
- **Cross-References**: Smart documentation linking
- **Performance Monitoring**: Track skill effectiveness

### Documentation System
- **Living Documentation**: Always up-to-date
- **Cross-Referenced**: All documents linked
- **Standards-Based**: Follows governance rules
- **AI-Optimized**: Structured for agent consumption

---

## 🚨 Getting Help

### Troubleshooting
1. **Check Standards**: Verify `.windsurf/rules/` compliance
2. **Review Documentation**: Check relevant AGENTS.*.md files
3. **Quality Gates**: Ensure all quality checks pass
4. **Architecture**: Follow ServiceLocator + EventBus patterns

### Support Resources
- **Development Prompt**: [DEVELOPMENT_PROMPT.md](DEVELOPMENT_PROMPT.md)
- **Quick Start**: [QUICK_START.md](QUICK_START.md)
- **AI Agents**: [AGENTS.md](AGENTS.md)
- **Governance**: [.windsurf/rules/](../.windsurf/rules/)

---

## 🎮 Ready to Develop!

**Choose your task and the appropriate AI skill will be automatically selected to help you accomplish it following all project standards and best practices.**

*Example: "I want to implement multiplayer networking for the snake game"*
