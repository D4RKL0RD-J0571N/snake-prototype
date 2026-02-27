# Architecture Overview - Snake Prototype

This document summarizes the core architectural patterns used in the project.

## Core Patterns

### 1. Service Locator
The project uses a simple `ServiceLocator` for dependency injection.
- **Location**: `Assets/Scripts/Core/ServiceLocator.cs`
- **Usage**: `ServiceLocator.Get<T>()`
- **Registration**: Handled during the bootstrap phase (`GameController` / `Bootstrap`).

### 2. Typed Event Bus (`GameEventManager`)
Communication between decoupled systems is handled via typed events.
- **Location**: `Assets/Scripts/Core/GameEventManager.cs`
- **Events**: Defined in `Assets/Scripts/Events/GameEvents.cs`.
- **Usage**: 
  - `GameEventManager.AddListener<T>(callback)`
  - `GameEventManager.Publish(new T())`

### 3. Manager/View Separation
Most systems are split into a Logic Manager (pure C# or minimal Monobehaviour) and a Visual View.
- **Snake**: `SnakeManager` (logic) vs `SnakeView` (rendering).
- **Grid**: `GridManager` (data) vs `GridView` (rendering).

## System Details

### Level System
Handles procedural generation and visual consistency:
- **LevelConfig**: ScriptableObject containing grid size, seeds, and color palettes.
- **LevelGenerator**: Orchestrates the setup of walls, guards, and visual themes (Camera/UI/Grid colors).

### Detection System
Uses a **Source-Receiver** pattern:
- **DetectionManager**: Central service that tracks the global detection level and triggers "Game Over" on breach. Supports weighted aggregation from multiple sources.
- **DetectionSource**: Component placed on world objects (cameras, guards) that performs raycasting/FOV checks and reports sightings to the manager.
- **DetectionConeView**: Visualization tool for sources using LineRenderers with dynamic color shifting.

### UI System
Built using **UI Toolkit**.
- **UIManager**: Handles HUD updates (Score, Detection Bar) and overlays (GameOver/Retry).
- Responds to `GameStateChangedEvent` to toggle screens.

## Testing
- **EditMode Tests**: Located in `Assets/Tests/EditMode`. Covers core logic in `SnakeManager`, `GridManager`, and `ScoreManager`.
- **Asmdefs**: Core scripts are in `SnakePrototype.Scripts`. Tests have their own assemblies referencing the core.
