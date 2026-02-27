# Changelog
All notable changes to the Snake Prototype project will be documented in this file.

## [v0.5.0] – 2026-02-13
### Highlights
- **Phased VFX Implementation**: New `VFXManager` system for energy collection, death, and level completion effects.
- **Dynamic Audio Layering**: Re-engineered `AudioManager` with 3-layer adaptive music (Base, Tension, Alert) and real-time Low-Pass Filter modulation.
- **Advanced Shaders**:
    - `SnakeBody`: Procedural gradients and movement pulse.
    - `GridDynamic`: Reactive exposure and pulsing alert states.
    - `CorePulse`: Procedural glow for collectibles.
    - `GuardSurface`: Visual alert blinking for AI.
- **Cinematic Experience**: 
    - `CameraSetup` with smooth zoom-in transitions on level start.
    - Level Intro screen with palette previews and summary stats.
- **Theme Polish**: Updated `UIManager` to support direct element tinting and USS-based transitions.
- **Phase 5 VFX (2026-02-13)**:
    - **Screen Overlay**: CRT scanlines, vignette, and dynamic chromatic aberration (glitch) on detection.
    - **Collection Ripple**: Expanding energy rings on pickup.
    - **Snake Polish**: Digital Fresnel rim lighting, dissolve spawn effect, and idle micro-animations.
    - **Reactive Environment**: Global red lighting pulse on detection events.


### Fixed
- Fixed `NullReferenceException` in `VFXManager` when config is missing.
- Fixed Snake not resetting/respawning on new level starts.
- Resolved compilation issues with `IStyle.SetVariable` in `UIManager`.
- Fixed Snake segment orientation via `Billboard` auto-injection.
- Standardized `SnakeView` gradient calculations for growing bodies.

## [v0.3.1] – 2026-02-13

## [v0.3] – 2026-02-13
### Highlights
- **Dynamic Procedural Levels**: Implemented `LevelConfig` and `LevelGenerator` for seed-based wall generation and theme application.
- **Improved Respawn Logic**: Score now correctly resets on retry, and energy cores re-spawn properly.
- **Visual Polish**: Fixed grid visibility, background colors, and implemented color-tweened detection cones.
- **AI Guard System**: Guards now spawn dynamically at free grid positions with basic patrol capabilities.
- **Enhanced Detection**: `DetectionManager` supports weighted aggregation from multiple sources with a stress-threshold game-over trigger.

### Bug Fixes
- Fixed Energy Cores not appearing on initial load.
- Fixed Score not resetting on respawn.
- Fixed grid invisibility and cyan background flickering/default.
- Fixed vision cone rendering order (cones no longer hide cores).

### Technical Additions
- `LevelConfig` (ScriptableObject) for per-level metadata and palettes.
- `LevelGenerator` (Service) for orchestration of level setup.
- `GridManager` API expansion: `IsWall`, `SetWall`, `GetRandomSpawnPoints`.
- Render-queue management in `DetectionConeView` (Target: 1999).
- UI Palette propagation via `UIManager.SetPalette`.

### Upcoming (Phase 3.5)
- Procedural lighting & audio feedback.
- Dynamic palette transitions tied to detection intensity.
- Multi-zone AI patrol systems.

## [v0.2] – 2024-02-12
### Added
- Core Snake movement system.
- Detection system foundation.
- Basic URP setup.
