# Implementation Plan - Phase 3.5: Ambient Feedback

## 🎯 Objectives
Enhance the emotional tension of the stealth gameplay through reactive lighting and audio. The environment should feel "alive" and responsive to the player's detection state.

## 🛠️ Proposed Systems

### 1. Lighting System (`LightingManager`)
A new service to orchestrate global and local light changes.
- **Service**: `LightingManager` (IGameService).
- **Functionality**:
    - Manage intensity, color, and flickering of scene lights.
    - Subscribe to `DetectionLevelChangedEvent`.
    - Transition from "Safe" (Static Cold Blue/Dim) to "Alert" (Pulsing Red) as detection increases.
- **Visuals**: Use URP global volume or direct light manipulation.

### 2. Audio System (`AudioManager`)
A centralized service for all sound effects and procedural music/ambience.
- **Service**: `AudioManager` (IGameService).
- **Functionality**:
    - **Ambient Loop**: Low-frequency industrial hum.
    - **Tension Layer**: Synth-wave layer that increases in volume with detection levels.
    - **SFX**: UI clicks, collection "ping", snake movement "slither" (subtle), and alert sirens.
- **Implementation**: `AudioMixer` with snapshots for smooth state transitions.

### 3. Level Integration
- Extend `LevelConfig` to include Ambient Lighting settings and specific Audio tracks.
- `LevelGenerator` will initialize the `LightingManager` with level-specific palettes.

## 📋 Task List

### Lighting
- [ ] Create `LightingManager.cs`.
- [ ] Implement `SetIntensity(float level)` and `SetColor(Color color)`.
- [ ] Hook into `DetectionLevelChangedEvent` for real-time pulsing logic.
- [ ] Add basic volumetric fog or bloom adjustment tied to stress.

### Audio
- [ ] Create `AudioManager.cs`.
- [ ] Setup `AudioMixer` in the project.
- [ ] Implement `PlaySFX(string id)` and `SetMusicIntensity(float level)`.
- [ ] Add sound triggers for `EnergyCollectedEvent` and `GameStateChangedEvent`.

### Integration & Polish
- [ ] Update `GameBootstrapper` to register new services.
- [ ] Verify transitions don't impact performance.
- [ ] Add basic "flicker" effect to lights when detection is high.
