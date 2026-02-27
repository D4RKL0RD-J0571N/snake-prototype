---
title: "Snake Prototype - Game Design Document"
version: "v0.3.1 Stability"
author: "Jostin Lopez"
last_updated: "2026-02-13"
engine: "Unity 6000.3.8f1 (URP)"
architecture: "ServiceLocator + EventBus (Jostin Architecture)"
status: "Active Development - Phase 3.5 Initiation"
---

# 1. Overview
“A stealth-inspired evolution of Snake set in an industrial labyrinth, balancing growth and visibility.”

### 1.1 Build Configuration
- **Unity Version**: Unity 6000.3.8f1
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Target Platform**: Desktop (Windows/Mac/Linux)
- **Simulation Mode**: 3D environment using URP materials, logic strictly constrained to 2D grid coordinates.

# 2. Core Concept
**SNEK: Labyrinth Protocol** is a mechanical evolution of the classic Snake. Players control a mechanical serpent navigating an industrial labyrinth. The game balances the traditional growth mechanic with stealth and detection tension. As the snake consumes energy cores, it grows longer, increasing its capacity but also making it more vulnerable to detection systems within the labyrinth.

# 3. Gameplay Loop

| Phase | Interaction | Player/System Activity |
| :--- | :--- | :--- |
| **Exploration** | Navigation | Exploring the grid-based industrial labyrinth. |
| **Collection** | Gathering | Consuming Energy Cores to grow and increase score. |
| **Avoidance** | Stealth | Maneuvering to avoid detection and self-collision. |
| **Survival** | Tension | Managing length and speed as complexity increases. |
| **Exit** | Completion | Reaching specific milestones or surviving as long as possible. |

# 4. Core Mechanics

| System | Description | Implementation Layer |
| :--- | :--- | :--- |
| **Movement** | Grid-based logic with smart 180-turn prevention. | `SnakeManager` (Logic) |
| **Growth** | Tail segment addition upon energy core consumption. | `SnakeManager` (✅ Implemented) |
| **Detection** | Line-of-sight / Vision-cone based raycasting with event alerts. | `DetectionManager` (Mock Active) |
| **Energy Collection** | Core spawning and collection triggers. | `EnergyCoreManager` |
| **UI/HUD** | UXML/USS based real-time stats display with event bindings. | `UIManager` (UI) |
| **Game States** | Bootstrap, Play, Pause, Game Over logic. | `GameBootstrapper` |

# 5. UI Toolkit Features
The project leverages Unity UI Toolkit for a clean, modular interface. The `UIManager` dynamically reacts to game events:
- **ScoreChangedEvent**: Updates the HUD score display in real-time.
- **DetectedEvent**: Triggers visual feedback when the player enters a danger zone.
- **GameOverEvent**: Smoothly transitions to the Game Over overlay.
- **UXML/USS**: Teaches modular layout composition and responsive styling using flexbox.

# 6. Visual Style & Mood
- **Aesthetic**: Industrial stealth with high-contrast volumetric lighting.
- **Color Palette**: Dark metallic surfaces with neon-cyan highlights for interactive elements.
- **Camera**: Top-down or slight isometric perspective. `CameraFollow` bridge component tracks `SnakeManager.HeadPosition` for smooth tracking.
- **Audio**: Ambient machine hums, industrial clicks, and synth-wave undertones.

# 7. Scope Milestones

| Phase | Focus | Status |
| :--- | :--- | :--- |
| **1** | Core Architecture | ✅ Complete |
| **2** | Systems & Visuals | ✅ Complete |
| **2.5** | Feedback Layer & Camera | 🚧 In Progress |
| **3** | UI Polish & Stealth Expansion | ⏳ Planned |
| **4** | Content & Balancing | ⏳ Planned |
| **5** | Audio & FX | ⏳ Planned |

# 8. Folder Structure
```text
Assets/
 ├── Scripts/
 │    ├── Core/          # ServiceLocator, GameBootstrapper, Base classes
 │    ├── Systems/       # Domain logic (Snake, Grid, EnergyCore, Detection)
 │    ├── Events/        # GameEventManager and Event Registry (ScoreChanged, etc.)
 │    └── Utils/         # Math helpers and extensions
 ├── UI/
 │    ├── UXML/          # Visual Tree assets
 │    └── USS/           # Stylesheets (CSS equivalent)
 ├── Prefabs/            # Snake segments, Cores, Map obstacles
 ├── ScriptableObjects/  # Configuration and Data containers
 └── Scenes/             # Bootstrap and Gameplay levels
```

# 9. Architecture Summary
The project follows a **ServiceLocator-based decoupled architecture** emphasizing clarity, modularity, and event-driven design.

- **Pure Logic Systems (Managers)**: Logic-heavy classes (e.g., `GridManager`, `SnakeManager`, `ScoreManager`).
- **MonoBehaviour Bridges (Views)**: Components that interface logic with Unity (e.g., `GridView`, `SnakeView`, `CameraFollow`).
- **EventBus**: Centralized communication via `GameEventManager`.

### Event Flow Diagram
```text
[InputManager] ----> (OnDirectionInput) ----> [SnakeManager]
                                                   |
[SnakeManager] ----> (OnSnakeMoved) ----------+    |
                                              v    v
[EnergyCoreManager] < (OnCorePickedUp) <--- [GameEventManager]
                                              ^    ^
[ScoreManager] ----> (ScoreChangedEvent) -----+    |
                                                   |
[UIManager] <------- (Listen: ScoreChanged) -------+
```

# 10. Current Status Snapshot
- **SnakeManager**: ✅ Ready (Precision grid movement & growth implemented)
- **GridManager**: ✅ Ready (Dynamic grid resizing and wall registry)
- **ScoreManager**: ✅ Ready (Event-based score tracking with respawn reset)
- **InputManager**: ✅ Ready (Refactored to New Input System)
- **UIManager**: ✅ Ready (HUD event bindings & dynamic palette support)
- **EnergyCoreManager**: ✅ Ready (Multi-core spawning & collection triggers)
- **LevelGenerator**: ✅ Ready (Procedural generation & visual theme application)
- **DetectionManager**: ✅ Ready (Weighted aggregation & stress-threshold logic)

# 11. Testing & Audit
- **Architecture Score**: 9.2/10 (Verified during v0.2 Audit).
- **Test-Ready**: All core Managers (`SnakeManager`, `GridManager`, `ScoreManager`) are decoupled for EditMode testing.
- **Verification Plan**: Automated PlayMode tests planned for `DetectionManager` vision cones and `UIManager` event responses.

### 11.1 Performance Metrics
- **Target Framerate**: Constant 60+ FPS on mid-range hardware.
- **Scalability**: Grid systems tested up to 100x100 tiles with minimal overhead.
- **Optimization**: Object pooling utilized for `EnergyCoreManager` and Tail Segments to minimize garbage collection (GC) spikes.
- **Update Cycle**: Service-based logic updates separate from Unity's `Update()` loop where possible.

# 12. Known Technical Issues
- **UI Document Persistence**: Potential for destroyed `UIDocument` references when transitioning or reloading scenes; investigation of reference management required.
- **Input Edge Cases**: Occasional rapid direction changes requiring buffer validation (Smart 180 logic implemented but needs stress testing).

# 13. Future Ideas
- **Energy Overload Mode**: High-risk, high-reward speed boosts.
- **Modular AI Patterns**: Advanced patrolling behaviors for detection units.
- **Procedural Map Generation**: Industrial labyrinth seeds for infinite replayability.

# 14. Vision Statement
“**SNEK: Labyrinth Protocol** is a minimalist stealth-simulation inspired by the purity of Snake and the tension of Metal Gear, built under a pure event-driven ServiceLocator architecture.”

## 📝 Changelog
### v0.3.1 Stability – 2026-02-13
- Resolved UI Toolkit texture errors by standardizing CSS properties.
- Formalized deterministic testing via `ForceCheck` on `DetectionSource`.
- Aligning project for Phase 3.5 (Ambient Feedback).

### v0.3 Procedural – 2026-02-13

### v0.2 Prototype – 2026-02-12
