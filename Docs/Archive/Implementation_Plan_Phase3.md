# Implementation Plan – Phase 3: Stealth Expansion & Alert System

## 🎯 Goal
Transform the polished prototype into a stealth-aware simulation by introducing patrolling guards, detection logic, and player feedback systems. Phase 3 focuses on player tension and visual clarity — merging detection mechanics with alert-based feedback.

## Proposed Changes

### 1. Detection & AI Systems
#### [NEW] [PatrolAgent.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Detection/PatrolAgent.cs)
- Defines basic AI guard behavior using state machine (Idle → Patrol → Alert → Search).
- Movement: Grid-based pathfinding along PatrolPoints.
- Integrates with `DetectionSource` to enter “Alert” when snake is spotted.

#### [MODIFY] [DetectionManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Detection/DetectionManager.cs)
- Replace mock logic with aggregated alert score.
- Compute weighted values from active `DetectionSource` reports.
- Trigger stress threshold logic before hard fail.

### 2. Alert Meter UI
#### [MODIFY] [UIManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/UIManager.cs)
- Subscribe to `DetectionLevelChangedEvent`.
- Update visual bar using UI Toolkit progress bar.
- Animate color shift (green → yellow → red).

#### [NEW] AlertMeter UI Assets
- `Assets/UI/UXML/AlertMeter.uxml`
- `Assets/UI/USS/AlertMeter.uss`

### 3. Gameplay Integration
#### [MODIFY] GameBootstrapper / Controller
- Ensure alert events route to UIManager.
- Handle spawning of AI guards via registry if applicable.

#### [NEW] Prefabs
- `Prefabs/Guards/GuardBasic.prefab`
- `Prefabs/Guards/CameraUnit.prefab`

### 4. Visual Polish
#### [NEW] [AlertOverlayView.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/AlertOverlayView.cs)
- Displays full-screen red tint on high alert.
- Lerps opacity based on `DetectionLevel`.

## Verification Plan

### Automated Tests
- EditMode: `DetectionManager` aggregation and decay logic.
- PlayMode: `DetectionSource` raycast accuracy against moving snake.

### Manual Verification
1. **Snake enters FOV**: Alert Meter fills progressively; UI glows red at high levels.
2. **Snake leaves FOV**: Detection level decays smoothly.
3. **Stress Threshold**: Verify game doesn't end immediately at 100, but enters a short "Stress" state if specified.
4. **Respawn**: Verify all alert levels and AI states reset.
