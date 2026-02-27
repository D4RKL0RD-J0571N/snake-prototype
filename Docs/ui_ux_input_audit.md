# Snake Prototype — UI/UX & Input Systems Focused Audit

> **Scope**: UI Toolkit assets, UIManager hookups, Input System, Audio system, Controller mapping, Mobile support, Accessibility  
> **Date**: 2026-02-27 · **Baseline**: [deep_audit_report.md](file:///d:/Development/Game%20Development/snake-prototype/Docs/deep_audit_report.md)

---

## 1 · Problem List (Severity-Ranked)

### 🔴 Critical (Blocks shipping)

| # | Problem | File(s) | Evidence |
|---|---------|---------|----------|
| C1 | **No Main Menu** — game auto-starts on `Awake()`. No title screen, no entry point for players. `GameState.MainMenu` exists in the enum but is never entered. | [GameBootstrapper.cs:38-41](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameBootstrapper.cs#L38), [LevelFlowManager.cs:38](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs#L38), [GameEvents.cs:132](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#L132) | `LevelFlowManager.Initialize()` immediately calls `PrepareNextLevel(true)` — skips MainMenu entirely |
| C2 | **Pause System Dead** — `OnPause()` handler exists but its body only contains a comment. `PauseInputEvent` is never published. No `Time.timeScale` toggle for pause. No `GameState.Paused` ever emitted. | [InputManager.cs:95-100](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L95) | Line 98: `// GameEventManager.Publish(new PauseInputEvent());` commented out |
| C3 | **Confirm = Unguarded Respawn** — Pressing Enter/Space/A fires `RespawnEvent()` regardless of `GameState`. Mid-gameplay confirm causes teleport-respawn. | [InputManager.cs:56](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L56) | `confirmAction.performed += ctx => GameEventManager.Publish(new RespawnEvent());` — no state guard |
| C4 | **RemoveListener is a no-op** — `GameEventManager.RemoveListener<T>()` has an empty body. All 8 calls in `UIManager.Shutdown()` + calls in every other service are silently ignored, causing **listener leaks** on re-init. | [GameEventManager.cs:38-42](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameEventManager.cs#L38) | Method body is `{ }`. Only `Clear()` actually works. |

### 🟡 High (Blocks polished release)

| # | Problem | File(s) | Evidence |
|---|---------|---------|----------|
| H1 | **No Options/Settings UI** — No UXML for Options. `AudioManager._masterVolume` = 0.6f hardcoded. No way for players to adjust volume, resolution, or controls. | [AudioManager.cs:25](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/AudioManager.cs#L25) | No `Options.uxml` exists. No PlayerPrefs persistence for audio. |
| H2 | **No AudioMixer** — Volume control is done via raw `AudioSource.volume` multiplication. No AudioMixer asset exists in the project. No exposed params for master/music/sfx channels. | Entire `Assets/` folder | `find *.mixer` returns 0 results. `AudioManager` multiplies `_masterVolume` directly into source volumes. |
| H3 | **Zero Mobile Input** — No `<Touchscreen>` bindings in code or in `SnakeControls.inputactions`. No virtual joystick, no swipe handler, no on-screen controls. | [InputManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs), [SnakeControls.inputactions](file:///d:/Development/Game%20Development/snake-prototype/Assets/Settings/SnakeControls.inputactions) | Zero `Touchscreen` bindings anywhere in the project |
| H4 | **InputAction Asset Unused** — `SnakeControls.inputactions` defines Move/Pause/Confirm/Cancel with proper bindings, but `InputManager.cs` creates all actions inline via `new InputAction(...)`. The asset is orphaned. | [InputManager.cs:27-52](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L27), [SnakeControls.inputactions](file:///d:/Development/Game%20Development/snake-prototype/Assets/Settings/SnakeControls.inputactions) | Code constructs `new InputAction("Move", ...)` instead of loading from asset |
| H5 | **No Control Schemes** — `SnakeControls.inputactions` has `"controlSchemes": []`. No Keyboard&Mouse / Gamepad / Touch scheme defined. Cannot disambiguate input devices. | [SnakeControls.inputactions:258](file:///d:/Development/Game%20Development/snake-prototype/Assets/Settings/SnakeControls.inputactions#L258) | Empty array |
| H6 | **SerializeField on Plain C# Class** — `InputManager` has `[Header]` and `[SerializeField]` attributes but is not a MonoBehaviour. These attributes are silently ignored. | [InputManager.cs:14-17](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L14) | `[SerializeField] private float _deadzone = 0.1f;` on non-MonoBehaviour |
| H7 | **Panel Settings Mismatch** — `Panel Settings.asset` has reference resolution `1200×800` but `UIManager.cs` uses `1920×1080` as reference. UI scale factor will be inconsistent. | [Panel Settings.asset:27](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/Panel%20Settings.asset#L27), [UIManager.cs:41](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/UIManager.cs#L41) | `m_ReferenceResolution: {x: 1200, y: 800}` vs `new Vector2(1920, 1080)` |
| H8 | **Highscore Not Integrated** — `HighscoreData.cs` and `HighscoreUI.cs` exist with full DTOs but are never wired into the game-over or main menu flow. No persistence layer (no `PlayerPrefs`/`JsonUtility.Save`). | [HighscoreData.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Score/HighscoreData.cs), [HighscoreUI.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/HighscoreUI.cs) | No `PlayerPrefs` or file I/O in score system |

### 🟢 Medium (Polish / Hardening)

| # | Problem | File(s) | Evidence |
|---|---------|---------|----------|
| M1 | **No Gamepad Detection Feedback** — No callback for `InputSystem.onDeviceChange`. Player has no UI indication when a gamepad connects/disconnects. | [InputManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs) | No `onDeviceChange` subscription |
| M2 | **No Input Rebinding** — Input System's `PerformInteractiveRebinding()` API is not used. No rebind UI. | — | Feature absent |
| M3 | **No Accessibility Controls** — No font-size scaling options, no high-contrast toggle, no colorblind mode (despite procedural palette system). | `Theme.uss` has `--font-size-large: 24px` but no mechanism to change it at runtime | |
| M4 | **Orientation/Scaling on Mobile** — `Panel Settings` `m_ScreenMatchMode: 0` (match width). On tall mobile screens (19.5:9), UI elements may be clipped or too small. `m_Match: 0` should be adjusted. | [Panel Settings.asset:29](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/Panel%20Settings.asset#L29) | No orientation lock; no aspect-ratio-aware layout |
| M5 | **LevelFlowManager Has Misleading `[AddComponentMenu]`** — The class is a plain `IGameService`, not a MonoBehaviour. The attribute does nothing. | [LevelFlowManager.cs:11](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs#L11) | `[AddComponentMenu("SnakePrototype/Level/Level Flow Manager")]` on non-MonoBehaviour |
| M6 | **GameOver Overlay Lacks Score Recap** — `AlertOverlay` shows "SYSTEM FAILURE / GAME OVER" but no final score, level reached, or highscore comparison. | [UIManager.cs:228-243](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/UIManager.cs#L228) | No score data displayed on death |
| M7 | **Confirm and Cancel Actions Not Disposed** — `confirmAction` and `cancelAction` are local variables in `Initialize()`. They're never `Disable()`d or `Dispose()`d in `Shutdown()`. | [InputManager.cs:47-52](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L47), [InputManager.cs:107-116](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#L107) | Only `_moveAction` and `_pauseAction` are disposed |

---

## 2 · Existing Asset Inventory (UI Toolkit)

### ✅ Present
| File | Type | Notes |
|------|------|-------|
| `Assets/UI/HUD.uxml` | Root Layout | ScoreLabel, AlertMeter, AlertOverlay, LevelIntro |
| `Assets/UI/UXML/AlertMeter.uxml` | Component | Standalone detection meter |
| `Assets/UI/UXML/Highscore.uxml` | Screen | High scores with Back/Clear buttons |
| `Assets/UI/USS/HUD.uss` | Stylesheet | Neon theme, overlay, button, animations |
| `Assets/UI/USS/AlertMeter.uss` | Stylesheet | Meter fill + warning/danger states |
| `Assets/UI/USS/Highscore.uss` | Stylesheet | NES-style retro highscore layout |
| `Assets/UI/USS/Theme.uss` | Shared Vars | `--color-primary`, `--color-alert`, `--color-bg` |
| `Assets/UI/Panel Settings.asset` | Panel Config | Scale mode: Scale With Screen Size, ref: 1200×800 |
| `Assets/Settings/SnakeControls.inputactions` | Input Asset | Move/Pause/Confirm/Cancel — **unused** |

### ❌ Missing
| File | Purpose |
|------|---------|
| `Assets/UI/UXML/MainMenu.uxml` | Title screen: Start, Highscores, Options, Quit |
| `Assets/UI/UXML/Pause.uxml` | Pause overlay: Resume, Options, Quit |
| `Assets/UI/UXML/Options.uxml` | Settings: Volume sliders, controls tab |
| `Assets/UI/USS/MainMenu.uss` | Main menu styling |
| `Assets/UI/USS/Pause.uss` | Pause overlay styling |
| `Assets/UI/USS/Options.uss` | Options screen styling |
| `Assets/Audio/MainMixer.mixer` | AudioMixer with Master/Music/SFX groups |

---

## 3 · Reusable MainMenu UXML + USS Scaffolding

### MainMenu.uxml
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements" editor-extension-mode="False">
    <ui:Style src="project:/Assets/UI/USS/Theme.uss" />
    <ui:Style src="project:/Assets/UI/USS/MainMenu.uss" />

    <ui:VisualElement name="MainMenuRoot" class="menu-root">
        <!-- Terminal-Style Header -->
        <ui:VisualElement class="terminal-header">
            <ui:Label text=">" class="terminal-cursor blink" />
            <ui:Label name="TitleLabel" text="SNAKE_PROTOCOL v1.0" class="title-text" />
        </ui:VisualElement>

        <ui:Label name="SubtitleLabel" text="[ SYSTEM READY ]" class="subtitle-text" />

        <!-- Menu Buttons -->
        <ui:VisualElement name="MenuButtons" class="menu-button-column">
            <ui:Button name="StartButton" text="> INITIALIZE" class="menu-button" />
            <ui:Button name="HighscoresButton" text="> HIGH SCORES" class="menu-button" />
            <ui:Button name="OptionsButton" text="> SYSTEM CONFIG" class="menu-button" />
            <ui:Button name="QuitButton" text="> TERMINATE" class="menu-button menu-button--danger" />
        </ui:VisualElement>

        <!-- Version Footer -->
        <ui:Label name="VersionLabel" text="build 0.2.0 // unity 6" class="version-text" />
    </ui:VisualElement>
</ui:UXML>
```

### MainMenu.uss
```css
/* MainMenu.uss — Terminal-styled main menu for Snake Protocol */

.menu-root {
    width: 100%;
    height: 100%;
    background-color: rgba(0, 5, 2, 0.97);
    align-items: center;
    justify-content: center;
    flex-direction: column;
    padding: 40px;
}

.terminal-header {
    flex-direction: row;
    align-items: center;
    margin-bottom: 10px;
}

.terminal-cursor {
    font-size: 48px;
    color: var(--color-primary, #00FF99);
    margin-right: 8px;
    -unity-font-style: bold;
}

.blink {
    transition: opacity 0.5s;
}

.title-text {
    font-size: 48px;
    color: var(--color-primary, #00FF99);
    text-shadow: 0 0 20px rgba(0, 255, 153, 0.6);
    -unity-font-style: bold;
    letter-spacing: 4px;
}

.subtitle-text {
    font-size: 20px;
    color: rgba(0, 255, 153, 0.5);
    margin-bottom: 60px;
    letter-spacing: 3px;
}

.menu-button-column {
    flex-direction: column;
    align-items: stretch;
    width: 320px;
}

.menu-button {
    font-size: 22px;
    color: var(--color-primary, #00FF99);
    background-color: transparent;
    border-width: 1px;
    border-color: rgba(0, 255, 153, 0.3);
    margin: 6px 0;
    padding: 14px 24px;
    -unity-font-style: bold;
    letter-spacing: 2px;
    -unity-text-align: middle-left;
    border-radius: 4px;
    transition: background-color 0.15s, border-color 0.15s, scale 0.1s;
}

.menu-button:hover {
    background-color: rgba(0, 255, 153, 0.1);
    border-color: var(--color-primary, #00FF99);
    scale: 1.03;
}

.menu-button:active {
    background-color: rgba(0, 255, 153, 0.2);
    scale: 0.98;
}

.menu-button:focus {
    border-color: var(--color-primary, #00FF99);
    border-width: 2px;
}

.menu-button--danger {
    color: var(--color-alert, #FF3333);
    border-color: rgba(255, 51, 51, 0.3);
}

.menu-button--danger:hover {
    background-color: rgba(255, 51, 51, 0.1);
    border-color: var(--color-alert, #FF3333);
}

.version-text {
    font-size: 12px;
    color: rgba(255, 255, 255, 0.2);
    margin-top: 60px;
    letter-spacing: 1px;
}
```

---

## 4 · Input Mapping Plan

### 4.1 Recommended InputAction Asset Structure

> **Goal**: Migrate from inline `new InputAction(...)` to the existing `SnakeControls.inputactions` asset. Add proper Control Schemes and a UI action map.

```
SnakeControls.inputactions
├── ActionMap: "Gameplay"
│   ├── Move        (Value / Vector2)   → WASD, Arrows, Gamepad/leftStick, Gamepad/dpad, Touchscreen/swipe*
│   ├── Pause       (Button)            → Escape, P, Gamepad/start
│   ├── Confirm     (Button)            → Enter, Space, Gamepad/buttonSouth
│   └── Cancel      (Button)            → Backspace, Escape, Gamepad/buttonEast
│
├── ActionMap: "UI" (for menus)
│   ├── Navigate    (Value / Vector2)   → WASD, Arrows, Gamepad/leftStick, Gamepad/dpad
│   ├── Submit      (Button)            → Enter, Space, Gamepad/buttonSouth
│   ├── Cancel      (Button)            → Escape, Backspace, Gamepad/buttonEast
│   └── Point       (Value / Vector2)   → Mouse/position, Touchscreen/primaryTouch/position
│
├── ActionMap: "Touch" (mobile only)
│   ├── SwipeDirection (Value / Vector2) → Custom composite or interaction
│   └── Tap            (Button)          → Touchscreen/primaryTouch/tap
│
└── ControlSchemes:
    ├── "Keyboard&Mouse"  → <Keyboard>, <Mouse>
    ├── "Gamepad"         → <Gamepad>
    └── "Touch"           → <Touchscreen>
```

### 4.2 Typed Event Mapping

```
InputAction         →   GameEvent               →   Consumers
─────────────────────────────────────────────────────────────
Move.performed      →   MoveInputEvent(dir)     →   SnakeManager
Pause.performed     →   PauseInputEvent()       →   PauseService (NEW)
Confirm.performed   →   ConfirmEvent()          →   UIManager (context-sensitive)
Cancel.performed    →   CancelEvent() (NEW)     →   UIManager (back navigation)
SwipeDirection      →   MoveInputEvent(dir)     →   SnakeManager (via adapter)
```

### 4.3 InputManager Refactor Plan

```csharp
// BEFORE: Inline action creation
_moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");

// AFTER: Load from asset
[SerializeField] private InputActionAsset _controlsAsset; // assigned in Bootstrapper

// In Initialize():
var gameplay = _controlsAsset.FindActionMap("Gameplay");
_moveAction = gameplay.FindAction("Move");
_pauseAction = gameplay.FindAction("Pause");
_confirmAction = gameplay.FindAction("Confirm");
_cancelAction = gameplay.FindAction("Cancel");
```

---

## 5 · Minimal Implementable Tasks

### Task A: PauseService — Event-Based Freeze

**Effort**: S · **Priority**: 🔴 P0

#### New File: `Assets/Scripts/Systems/Game/PauseService.cs`
```csharp
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Game
{
    /// <summary>
    /// Manages pause state via event-driven timeScale toggle.
    /// Listens for PauseInputEvent and publishes GameStateChangedEvent.
    /// </summary>
    public class PauseService : IGameService
    {
        #region Fields
        private bool _isPaused;
        private GameState _previousState = GameState.Playing;
        #endregion

        #region IGameService
        public void Initialize()
        {
            _isPaused = false;
            GameEventManager.AddListener<PauseInputEvent>(OnPauseInput);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<PauseInputEvent>(OnPauseInput);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        }
        #endregion

        #region Event Handlers
        private void OnPauseInput(PauseInputEvent e)
        {
            // Only allow pause during Playing state
            // Only allow unpause during Paused state
            if (!_isPaused && _previousState != GameState.Playing) return;

            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;

            GameEventManager.Publish(new GameStateChangedEvent(
                _isPaused ? GameState.Paused : GameState.Playing));
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            // Track state so we can guard when pause is allowed
            if (e.NewState != GameState.Paused)
            {
                _previousState = e.NewState;
                _isPaused = false;
            }
        }
        #endregion
    }
}
```

#### Changes Required:
1. **InputManager.cs:98** — Uncomment `GameEventManager.Publish(new PauseInputEvent());`
2. **GameBootstrapper.cs** — Register `PauseService` before UIManager
3. **UIManager.cs** — Add `GameState.Paused` handler in `OnGameStateChanged`

#### Acceptance Criteria:
- [ ] Pressing Esc/P/Start publishes `PauseInputEvent`
- [ ] `Time.timeScale` becomes 0 when paused, 1 when resumed
- [ ] `GameStateChangedEvent(Paused)` is published
- [ ] UIManager shows pause overlay (reuse AlertOverlay or new Pause.uxml)
- [ ] Pressing Esc/P/Start again resumes gameplay
- [ ] Pause only works during `GameState.Playing`
- [ ] Cannot pause during MainMenu, GameOver, or LevelIntro

---

### Task B: Options Screen — Audio Sliders

**Effort**: M · **Priority**: 🟡 P1

#### New File: `Assets/UI/UXML/Options.uxml`
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements" editor-extension-mode="False">
    <ui:Style src="project:/Assets/UI/USS/Theme.uss" />
    <ui:Style src="project:/Assets/UI/USS/Options.uss" />

    <ui:VisualElement name="OptionsRoot" class="options-root">
        <ui:Label text="SYSTEM CONFIG" class="options-title" />

        <!-- Audio Section -->
        <ui:VisualElement class="options-section">
            <ui:Label text="// AUDIO" class="section-header" />

            <ui:VisualElement class="slider-row">
                <ui:Label text="MASTER" class="slider-label" />
                <ui:Slider name="MasterVolumeSlider" low-value="0" high-value="1" value="0.6"
                           class="options-slider" />
                <ui:Label name="MasterValueLabel" text="60%" class="slider-value" />
            </ui:VisualElement>

            <ui:VisualElement class="slider-row">
                <ui:Label text="MUSIC" class="slider-label" />
                <ui:Slider name="MusicVolumeSlider" low-value="0" high-value="1" value="0.5"
                           class="options-slider" />
                <ui:Label name="MusicValueLabel" text="50%" class="slider-value" />
            </ui:VisualElement>

            <ui:VisualElement class="slider-row">
                <ui:Label text="SFX" class="slider-label" />
                <ui:Slider name="SFXVolumeSlider" low-value="0" high-value="1" value="0.7"
                           class="options-slider" />
                <ui:Label name="SFXValueLabel" text="70%" class="slider-value" />
            </ui:VisualElement>
        </ui:VisualElement>

        <!-- Back Button -->
        <ui:VisualElement class="button-container" style="margin-top: 40px;">
            <ui:Button name="BackButton" text="< BACK" class="menu-button" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

#### New Service: `Assets/Scripts/Systems/Audio/AudioSettingsService.cs`
```csharp
using UnityEngine;
using SnakePrototype.Core;

namespace SnakePrototype.Systems.Audio
{
    /// <summary>
    /// Persists and applies audio volume settings via PlayerPrefs.
    /// Decoupled from AudioManager — publishes settings; AudioManager consumes.
    /// </summary>
    public class AudioSettingsService : IGameService
    {
        #region Constants
        private const string KeyMaster = "audio_master_volume";
        private const string KeyMusic = "audio_music_volume";
        private const string KeySFX = "audio_sfx_volume";
        #endregion

        #region Properties
        public float MasterVolume { get; private set; } = 0.6f;
        public float MusicVolume { get; private set; } = 0.5f;
        public float SFXVolume { get; private set; } = 0.7f;
        #endregion

        #region IGameService
        public void Initialize()
        {
            Load();
        }

        public void Shutdown()
        {
            Save();
        }
        #endregion

        #region Public API
        public void SetMasterVolume(float value)
        {
            MasterVolume = Mathf.Clamp01(value);
            Save();
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            Save();
        }

        public void SetSFXVolume(float value)
        {
            SFXVolume = Mathf.Clamp01(value);
            Save();
        }
        #endregion

        #region Persistence
        private void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(KeyMaster, 0.6f);
            MusicVolume = PlayerPrefs.GetFloat(KeyMusic, 0.5f);
            SFXVolume = PlayerPrefs.GetFloat(KeySFX, 0.7f);
        }

        private void Save()
        {
            PlayerPrefs.SetFloat(KeyMaster, MasterVolume);
            PlayerPrefs.SetFloat(KeyMusic, MusicVolume);
            PlayerPrefs.SetFloat(KeySFX, SFXVolume);
            PlayerPrefs.Save();
        }
        #endregion
    }
}
```

#### Changes Required:
1. **AudioManager** — Read from `AudioSettingsService` instead of hardcoded `_masterVolume`
2. **UIManager** — Wire slider `RegisterValueChangedCallback` to `AudioSettingsService`
3. **GameBootstrapper** — Register `AudioSettingsService` before `AudioManager`

#### Acceptance Criteria:
- [ ] Options screen accessible from MainMenu and Pause menu
- [ ] Master/Music/SFX sliders move and display percentage
- [ ] Slider changes immediately affect live audio levels
- [ ] Settings persist across sessions via PlayerPrefs
- [ ] Back button returns to previous screen (MainMenu or Pause)

---

### Task C: Gamepad Detection & Simple Remap

**Effort**: M · **Priority**: 🟢 P2

#### New File: `Assets/Scripts/Systems/Input/GamepadDetector.cs`
```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Input
{
    /// <summary>
    /// Monitors gamepad connection/disconnection and publishes events.
    /// Provides simple rebinding support via Input System's interactive rebind API.
    /// </summary>
    public class GamepadDetector : IGameService
    {
        #region Fields
        private bool _gamepadConnected;
        #endregion

        #region IGameService
        public void Initialize()
        {
            // Check initial state
            _gamepadConnected = Gamepad.current != null;

            InputSystem.onDeviceChange += OnDeviceChange;

            Debug.Log($"GamepadDetector: Initial state — " +
                      $"Gamepad {(_gamepadConnected ? "CONNECTED" : "NOT FOUND")}");
        }

        public void Shutdown()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
        #endregion

        #region Properties
        public bool IsGamepadConnected => _gamepadConnected;
        public string GamepadName => Gamepad.current?.displayName ?? "None";
        #endregion

        #region Device Monitoring
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is not Gamepad) return;

            switch (change)
            {
                case InputDeviceChange.Added:
                    _gamepadConnected = true;
                    Debug.Log($"Gamepad connected: {device.displayName}");
                    GameEventManager.Publish(new GamepadChangedEvent(true, device.displayName));
                    break;
                case InputDeviceChange.Removed:
                    _gamepadConnected = Gamepad.current != null;
                    Debug.Log($"Gamepad disconnected: {device.displayName}");
                    GameEventManager.Publish(new GamepadChangedEvent(false, device.displayName));
                    break;
            }
        }
        #endregion

        #region Rebinding
        /// <summary>
        /// Starts an interactive rebind for the given action.
        /// Call from Options UI to let the player press a new button.
        /// </summary>
        public void StartRebind(InputAction action, System.Action onComplete)
        {
            action.Disable();
            var rebind = action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(op =>
                {
                    op.Dispose();
                    action.Enable();
                    onComplete?.Invoke();
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    action.Enable();
                })
                .Start();
        }
        #endregion
    }
}
```

#### New Event (add to GameEvents.cs):
```csharp
public class GamepadChangedEvent : GameEvent
{
    public bool Connected { get; }
    public string DeviceName { get; }
    public GamepadChangedEvent(bool connected, string name)
    {
        Connected = connected;
        DeviceName = name;
    }
}
```

#### Acceptance Criteria:
- [ ] Console logs when gamepad connects/disconnects
- [ ] `GamepadChangedEvent` published on device change
- [ ] `IsGamepadConnected` property accurate at all times
- [ ] Optional: Rebind a single action in Options screen via interactive rebind

---

### Task D: Mobile Virtual Joystick Adapter

**Effort**: L · **Priority**: 🟡 P1

#### Architecture

```
┌─────────────────────────┐
│ On-Screen Stick (UXML)  │──── Touch Input ────┐
│ (Visual draggable knob) │                      │
└─────────────────────────┘                      ▼
                                        MobileTouchAdapter
                                     (pure C# IGameService)
                                              │
                                    MoveInputEvent(dir)
                                              │
                                         SnakeManager
```

#### New File: `Assets/Scripts/Systems/Input/MobileTouchAdapter.cs`
```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using SnakePrototype.Core;
using SnakePrototype.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SnakePrototype.Systems.Input
{
    /// <summary>
    /// Converts touch swipe gestures into MoveInputEvents.
    /// Runs alongside InputManager for mobile platforms.
    /// </summary>
    public class MobileTouchAdapter : IGameService
    {
        #region Fields
        private float _swipeThreshold = 50f; // pixels
        private Vector2 _touchStartPos;
        private bool _isSwiping;
        #endregion

        #region IGameService
        public void Initialize()
        {
            if (!IsMobilePlatform()) return;

            EnhancedTouchSupport.Enable();
            Debug.Log("MobileTouchAdapter: Initialized for touch input");
        }

        public void Shutdown()
        {
            if (EnhancedTouchSupport.enabled)
                EnhancedTouchSupport.Disable();
        }
        #endregion

        #region Tick (called from Bootstrapper)
        public void Tick()
        {
            if (!EnhancedTouchSupport.enabled) return;

            foreach (var touch in Touch.activeTouches)
            {
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        _touchStartPos = touch.screenPosition;
                        _isSwiping = true;
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        if (!_isSwiping) break;
                        var delta = touch.screenPosition - _touchStartPos;
                        if (delta.magnitude >= _swipeThreshold)
                        {
                            var dir = SnapToCardinal(delta);
                            GameEventManager.Publish(new MoveInputEvent(dir));
                            _isSwiping = false; // one swipe per touch
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        _isSwiping = false;
                        break;
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Snaps a raw delta vector to the nearest cardinal direction.
        /// </summary>
        private Vector2 SnapToCardinal(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2.right : Vector2.left;
            else
                return delta.y > 0 ? Vector2.up : Vector2.down;
        }

        private bool IsMobilePlatform()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld;
#endif
        }
        #endregion
    }
}
```

#### UI Addition: On-Screen Pause Button for Mobile
Add to `HUD.uxml`:
```xml
<!-- Mobile Pause Button (hidden on desktop via USS) -->
<ui:Button name="MobilePauseButton" text="⏸" class="mobile-pause-button" />
```

#### Acceptance Criteria:
- [ ] Swiping in a cardinal direction publishes `MoveInputEvent`
- [ ] Snake responds to swipe input on Android/iOS
- [ ] On-screen pause button visible on mobile, hidden on desktop
- [ ] Tap pause button triggers `PauseInputEvent`
- [ ] No conflict with existing keyboard/gamepad input
- [ ] Touch start position resets on each new touch

---

## 6 · Testing Checklist

### Desktop (Windows/macOS)

| # | Test | Expected Result | Tools |
|---|------|----------------|-------|
| D1 | Launch game → Main Menu appears | Title screen with Start/Highscores/Options/Quit buttons visible | Visual |
| D2 | Click Start → Level Intro → Begin → Gameplay | Full flow: MainMenu → Intro → Playing state transition | Visual + Console |
| D3 | Press Esc during gameplay → Pause overlay | Game freezes, pause overlay shows, Esc resumes | Check `Time.timeScale == 0` |
| D4 | Press Esc during MainMenu → Nothing | Pause is guarded to `GameState.Playing` only | Console: no errors |
| D5 | Press Enter during gameplay → No respawn | Confirm is guarded to `GameState.GameOver` only | Snake position unchanged |
| D6 | Die → GameOver → Press Enter → Respawn | Confirm fires RespawnEvent only in GameOver | Score reset, snake at start |
| D7 | Open Options → Move sliders → Audio changes live | Volume updates immediately, persists after restart | PlayerPrefs verified |
| D8 | Connect gamepad → Console log appears | `GamepadChangedEvent(true)` logged | Console |
| D9 | Disconnect gamepad → Console log appears | `GamepadChangedEvent(false)` logged | Console |
| D10 | Gamepad: DPad/Stick steers snake | MoveInputEvent with correct direction | Gameplay |
| D11 | Gamepad: Start pauses, A confirms, B cancels | Correct events published | Console + visual |

### Mobile (Android Emulator / Device)

| # | Test | Expected Result | Tools |
|---|------|----------------|-------|
| M1 | Swipe right → Snake turns right | MoveInputEvent(1,0) published | Console |
| M2 | Swipe up → Snake turns up | MoveInputEvent(0,1) published | Console |
| M3 | Tap pause button → Game pauses | PauseInputEvent published, timeScale=0 | Visual |
| M4 | UI elements readable on 16:9 and 19.5:9 | No clipping, text ≥ 14px effective | Visual inspection |
| M5 | Orientation change → UI reflows | Panel Settings handles scale properly | Visual |
| M6 | No phantom touches during menu → gameplay transition | Touch adapter only active during Playing | Console: no stray events |

### Automated (Test Runner)

| # | Test | Mode | Validates |
|---|------|------|-----------|
| T1 | `PauseServiceTests.TogglePause_SetsTimeScaleZero` | EditMode | PauseService toggles timeScale |
| T2 | `PauseServiceTests.PauseOnlyDuringPlaying` | EditMode | Guard prevents pause in MainMenu/GameOver |
| T3 | `InputManagerTests.ConfirmOnlyInGameOver` | EditMode | State guard on confirm action |
| T4 | `AudioSettingsServiceTests.PersistsViaPlayerPrefs` | EditMode | Save/Load round-trip |
| T5 | `GamepadDetectorTests.DetectsConnection` | EditMode | Device change callback fires event |

### Commands to Run Tests
```bash
# EditMode tests
Unity.exe -batchmode -runTests -testPlatform EditMode -projectPath . -testResults TestResults/editmode.xml

# PlayMode tests
Unity.exe -batchmode -runTests -testPlatform PlayMode -projectPath . -testResults TestResults/playmode.xml

# Or via Unity Test Runner: Window > General > Test Runner > EditMode > Run All
```

---

## 7 · Implementation Priority Order

```
1. [C4] Fix RemoveListener (foundation for everything else)
2. [C3] Gate Confirm→Respawn by GameState
3. [C2] PauseService + uncomment PauseInputEvent
4. [C1] MainMenu UXML + UIManager state wiring
5. [H6] Remove invalid attributes from InputManager
6. [H7] Align Panel Settings and UIManager reference resolution
7. [H4] Migrate InputManager to use SnakeControls.inputactions asset
8. [H1] Options screen + AudioSettingsService
9. [H2] Create AudioMixer (optional — raw volume works for prototype)
10. [M1] GamepadDetector service
11. [H3] MobileTouchAdapter + on-screen pause
12. [M6] GameOver score recap
13. [H8] Wire HighscoreUI into flow
14. [M2] Input rebinding UI (stretch)
15. [M3] Accessibility controls (stretch)
```
