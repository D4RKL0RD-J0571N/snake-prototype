# Implementation Plan - Phase 2.5 Finalization

Polish player feedback and visual cohesion to make the game feel alive and responsive.

## Proposed Changes

### 1. Gameplay & UI Flow
- **SnakeManager**: Move timer/game-over reset into `ResetSnake`.
- **UIManager**: Ensure correct alert overlay handling during state transitions.

### 2. Camera Systems
- **CameraSetup**: Update to dynamically find snake head if no target is assigned.
- **SnakeView**: Expose `HeadTransform`.

### 3. Visual Polish & Debug Aids
- **DetectionConeView**: Visualization tool using `LineRenderer`.
- **GridView**: Added detection-based pulse/color shift.

## Verification Plan

### Manual Verification
1. **Respawn Test**: Verify snake resets and game resumes on retry.
2. **Camera Test**: Verify smooth follow of the head.
3. **Visual Feedback**: Verify grid pulses red at high detection levels.
