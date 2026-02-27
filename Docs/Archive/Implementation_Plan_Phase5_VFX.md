# Implementation Plan - Phase 5: Visual Polish & VFX

Focus: Elevating the game's aesthetic through dynamic shaders, particle effects, and post-process-like overlays.

## 1. Shaders & Materials
- [x] **Screen Overlay Shader**: Scanlines, vignette, and chromatic aberration.
- [x] **Pulse Ripple Shader**: For core collection and event feedback.
- [x] **Snake Dissolve/Transition**: Update `SnakeBody.shader` with a "Dissolve" property for spawning.
- [x] **Guard Vision Cone Polish**: Update `DetectionConeView` to use a more "scanned" texture or pattern.

## 2. VFX Systems
- [x] **VFXAnimateProperties**: Utility to drive shader values via curves.
- [x] **Collection Burst**: Create a prefab using `PulseRipple` and `VFXAnimateProperties` (Logic ready in `VFXManager`).
- [x] **Snake Trail**: Added `SnakeVisualPolish` to segments.
- [x] **Screen Glitch Controller**: Logic to ramp up chromatic aberration when detection is high.

## 3. Environment & Atmosphere
- [x] **Dynamic Background**: Add a subtle "grid flow" or moving patterns to the background.
- [x] **Lighting Manager Integration**: Pulse the global lighting on "Snake Spotted".

## 4. Integration
- [x] Update `VFXManager` to handle screen-space effects.
- [x] Hook into `DetectionLevelChangedEvent` to drive scanline intensity.
- [x] Update `LevelGenerator` (or `GridView`) to apply the "Digital Grid" shader settings.

## 5. Walkthrough Updates
- [x] Update `Walkthrough_Setup.md` with instructions for setting up the Overlay and VFX prefabs.
