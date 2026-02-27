# Snake Prototype v0.5.0 - Project Set-Up Walkthrough

This guide details how to set up the **Snake Prototype** in Unity 6000.3.8f1 from a clean state using the established **ServiceLocator + EventBus** architecture, incorporating all Phase 5 visual and audio polish.

---

## 1. Scene Architecture (The Bootstrap Pattern)
The game uses a **Bootstrap** pattern. A single persistent GameObject initializes and manages all the game's logic.

### Setup the Camera
1. Select the **Main Camera**.
2. Add the `CameraSetup` component.
3. Configure the **Settings**:
   - `FollowOffset`: `(0, 0, -2)` (Gameplay focus)
   - `IntroOffset`: `(0, 40, 0)` (High-altitude overview)
   - `SmoothSpeed`: `8`
   - `ZoomSpeed`: `2`

### Create the Bootstrapper
1. Create an empty GameObject named `[GameBootstrap]`.
2. Add the `GameBootstrapper` component.
3. Add a `UIDocument` component and assign `HUD.uxml`.
4. Add the `SnakeView` and `EnergyCoreView` components (to handle visual instantiation).
   - **IMPORTANT**: On `SnakeView`, you **MUST** assign the `Segment Prefab` field. Use a simple Cube or Quad with the `SnakeBody.shader` material. If this is missing, the snake will be invisible and the camera will not follow.

---

## 2. Data-Driven Configuration (ScriptableObjects)
All game settings are stored in assets. Create these via `Assets -> Create -> SnakePrototype`:

1. **GridConfig**:
   - Set Width/Height (e.g., 30x30).
   - Assign the **Neon Grid** material (using `SnakePrototype/GridDynamic` shader).
2. **SnakeConfig**:
   - Set Move Interval (0.15 - 0.20s).
   - Assign your **Segment Prefab** (must use `SnakePrototype/SnakeBody` shader for length gradients).
3. **LevelConfig**:
   - Assign the **Guard Prefab** and **Wall Prefab**.
   - Configure the **Audio Layering**: Assign `MusicBase`, `MusicTension`, and `MusicAlert` for dynamic detection response.
4. **VFXConfiguration**:
   - Assign particle prefabs for `EnergyPickup`, `SnakeDeath`, and `LevelComplete`.

---

## 3. Wiring the Inspector
Select `[GameBootstrap]` and assign the configs and audio sources:

- **Config Slots**: Drag your Grid, Snake, Level, and VFX SOs into their respective fields.
- **UI Document**: Drag the `UIDocument` on the same object into the **Main UI Document** field.
- **Audio References**: 
  - Create five child GameObjects with `AudioSource` components: `BaseMusic`, `TensionMusic`, `AlertMusic`, `SFX`, `Ambient`.
  - Drag these into the corresponding fields in `GameBootstrapper`.

---

## 4. Visual Assets & Shaders
To ensure the premium Phase 5 look, verify your prefabs use the correct custom shaders:

| Element | Shader Path | Key Properties |
| :--- | :--- | :--- |
| **Grid** | `SnakePrototype/GridDynamic` | `_AlertIntensity` (modulated by Detection) |
| **Snake Segments** | `SnakePrototype/SnakeBody` | `_GradientRatio`, `_Dissolve`, `_FresnelPower` |
| **Energy Cores** | `SnakePrototype/CorePulse` | `_PulseSpeed`, `_GlowIntensity` |
| **AI Guards** | `SnakePrototype/GuardSurface` | `_AlertIntensity` (flashes red when detecting) |
| **Screen Overlay** | `SnakePrototype/ScreenOverlay` | `_ScanlineIntensity`, `_AbberationStrength` |
| **Collection FX** | `SnakePrototype/PulseRipple` | `_Lifetime` (driven by `VFXAnimateProperties`) |

### NEW: Setup the Screen Overlay
1. Create a large **Quad** parented to the **Main Camera**.
2. Name it `ScreenOverlay_Effect`.
3. Create a material using `SnakePrototype/ScreenOverlay` and assign it.
4. Add the `ScreenOverlayController` component to the Quad.
5. Push the Quad forward just enough to cover the view (e.g., `Z = 0.5`) and scale it to fill the screen.
6. **Controller Settings**: Assign the Quad's Renderer to the controller's `_overlayRenderer` field.

### NEW: Setup the Collection Ripple
1. Create a prefab `VFX_CollectionRipple`.
2. Add a sub-object with a Renderer using the `SnakePrototype/PulseRipple` material.
3. Add the `VFXAnimateProperties` component.
4. **Settings**: Set `PropertyName` to `_Lifetime`, `Duration` to `0.8`, and use a linear curve from 0 to 1.
5. Assign this prefab to your **VFXConfiguration** asset in the `EnergyCollectionRipple` slot.

---

## 5. UI Polishing (USS Integration)
The UI is automatically updated via `UIManager`. 
- **Theming**: Ensure `HUD.uxml` is correctly layered. The system will query all `Button` and `Label` (class `alert-text`) elements to apply palette colors at runtime.
- **Animations**: The CSS classes `ui-fade-in` and `level-title-anim` handle cinematic transitions.

---

## 6. Execution Flow
When you press **Play**:
1. `GameBootstrapper` registers all services (Input, Grid, Snake, Detection, UI, Audio, VFX, Lighting, LevelFlow).
2. **Level Intro Screen** appears, showing the target goal and palette preview.
3. **Camera** starts at `IntroOffset` (highOverview).
4. Upon clicking **BEGIN**:
   - `LevelGenerator` builds the procedural corridors.
   - `SnakeManager` resets the player to `(5,5)`.
   - `CameraSetup` smoothly zooms into `FollowOffset`.
   - Music starts at the **Base Layer** (Low Intensity).
5. As you approach Guards, the `DetectionManager` ramps up detection:
   - **Tension Layer** fades in.
   - **Low-Pass Filters** muffle the music for "claustrophobic" feel.
   - **Neon Grid** begins to pulse red.

---

## 🛠 Troubleshooting
- **No Grid Visible?** Ensure the `GridMaterial` in your `GridConfig` has the `GridDynamic` shader assigned and isn't set to 0 scale.
- **Snake Death crashing?** Check the **VFXConfiguration**; if the `SnakeDeathVFX` prefab is missing, `VFXManager` will log a warning.
- **Audio too quiet?** The `AudioManager` sets music layers to 0 volume by default, fading them in only as detection levels rise. Check the **Base Volume** in the manager.
- **Compilation Error `SetVariable`?** Ensure you are using the latest `UIManager.cs` which uses direct element tinting instead of CSS variables for compatibility.
