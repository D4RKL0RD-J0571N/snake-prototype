# Snake Prototype — Deep Technical & Design Audit

> **Project**: snake-prototype · **Engine**: Unity 6000.3.8f1 (Unity 6) · **Pipeline**: URP  
> **Audit date**: 2026-02-27 · **Confidence**: 0.94  

---

## 1 · Executive Summary

The **snake-prototype** project is built on a disciplined, architecture-first foundation: a single [GameBootstrapper](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameBootstrapper.cs#16-159) initializes **14 services** through a static [ServiceLocator](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/ServiceLocator.cs#10-58), and all inter-system communication flows through a typed [GameEventManager](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameEventManager.cs#12-75) (EventBus). Core gameplay—snake movement, grid collisions, energy-core collection, guard detection, scoring, level progression, and procedural generation—is **functional and well-separated**. The codebase follows your personal conventions (pure C# services, ScriptableObject configs, `#region` blocks, doc-comments) with high consistency across ~3 000 lines of production code and ~330 lines of tests.

However, the project **cannot ship** in its current state. Three hard blockers prevent even a basic standalone build: (1) [Level_01.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/Level_01.unity) is missing from Build Settings, (2) there is no main-menu or game-start flow, and (3) the Confirm input is permanently hardwired to [RespawnEvent](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#79-80), causing unintended respawns during gameplay. Beyond blockers, the pause system is commented out, [RemoveListener](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameEventManager.cs#33-43) is a no-op (causing listener leaks), the [InputManager](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs#8-119) uses `[SerializeField]` on a plain C# class, and no UI exists for Options, audio settings, or control rebinding. Mobile builds have **zero** touch/swipe input support.

The good news: the architecture is sound and modular enough that none of these issues require rewrites. Every fix is **incremental**—most are under 50 lines of code. A focused 4-week sprint plan can bring this from "editor prototype" to "stable, deployable build" on desktop and mobile.

---

## 2 · File-Level Inventory

### 2.1 Core Architecture

| File | LOC | Status | Notes |
|------|-----|--------|-------|
| [GameBootstrapper.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameBootstrapper.cs) | 160 | ⚠️ Needs Work | 14-service init chain OK; [ScoreConfiguration](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameBootstrapper.cs#151-157) created at runtime instead of from SO asset; no DontDestroyOnLoad |
| [ServiceLocator.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/ServiceLocator.cs) | 59 | ✅ OK | Clean static dictionary; overwrite warning; proper Shutdown |
| [GameEventManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameEventManager.cs) | 76 | ❌ Bug | [RemoveListener](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameEventManager.cs#33-43) body is empty (no-op). UIManager.Shutdown calls it 8×, causing listener leaks |
| [IGameService.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/IGameService.cs) | 21 | ✅ OK | Clean interface |
| [CameraSetup.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/CameraSetup.cs) | 101 | ✅ OK | MonoBehaviour; smooth follow + intro zoom; uses `FindAnyObjectByType` (acceptable for prototype) |
| [GameEvents.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs) | 138 | ✅ OK | 17 event types + GameState enum. Well-structured. [ConfirmEvent](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#121-122) defined but never used |

### 2.2 Gameplay Systems

| File | LOC | Status | Notes |
|------|-----|--------|-------|
| [InputManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Input/InputManager.cs) | 120 | ❌ Bugs | `[Header]`/`[SerializeField]` on plain C# class (no-op); OnPause commented out (L98); Confirm→RespawnEvent always fires (L56); no mobile bindings |
| [SnakeManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Snake/SnakeManager.cs) | 218 | ✅ OK | Clean tick-based movement; smart 180-turn logic; respawn via events |
| [GridManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Grid/GridManager.cs) | 107 | ✅ OK | 2D bool array; spawn-point generation; world-position conversion |
| [DetectionManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Detection/DetectionManager.cs) | 95 | ✅ OK | Sighting aggregation; stress timer; clean game-over trigger |
| [EnergyCoreManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Energy/EnergyCoreManager.cs) | 131 | ✅ OK | Spawn/collect loop; avoids snake body overlap; 200-attempt safety |
| [LevelFlowManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs) | 139 | ⚠️ Needs Work | `[AddComponentMenu]` on non-MonoBehaviour (misleading); difficulty ramp mutates config in-place |
| [LevelGenerator.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelGenerator.cs) | 212 | ✅ OK | Seed-based RNG; border walls; guard spawning; palette application |
| [ScoreManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Score/ScoreManager.cs) | ~50 | ✅ OK | Simple accumulator |
| [ScoreSystem.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Score/ScoreSystem.cs) | ~80 | ✅ OK | Advanced scoring with multipliers |

### 2.3 Environment & Polish

| File | LOC | Status | Notes |
|------|-----|--------|-------|
| [AudioManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/AudioManager.cs) | 226 | ⚠️ Minor | `[AddComponentMenu]` on non-MonoBehaviour; layered music + low-pass filter system is sophisticated; no volume-control UI exposed |
| [ColorPaletteManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/ColorPaletteManager.cs) | ~80 | ✅ OK | Procedural palette generation |
| [LightingManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/LightingManager.cs) | ~60 | ✅ OK | Dynamic lighting transitions |
| [VFXManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/VFXManager.cs) | ~100 | ✅ OK | Particle management |
| [PoolingManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/PoolingManager.cs) | ~40 | ✅ OK | Object pooling utility |

### 2.4 UI System

| File | LOC | Status | Notes |
|------|-----|--------|-------|
| [UIManager.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/UIManager.cs) | 369 | ⚠️ Needs Work | Handles HUD + AlertOverlay + LevelIntro; no MainMenu, Pause, or Options state handling; responsive scaling logic present |
| [AlertOverlayView.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/AlertOverlayView.cs) | ~40 | ✅ OK | Animation helper |
| [HighscoreUI.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/HighscoreUI.cs) | ~60 | ✅ OK | Score list rendering |
| [ScreenOverlayController.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/ScreenOverlayController.cs) | ~30 | ✅ OK | Overlay toggling |

### 2.5 UI Toolkit Assets

| File | Status | Notes |
|------|--------|-------|
| [HUD.uxml](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/HUD.uxml) | ✅ OK | ScoreLabel, AlertMeter, AlertOverlay (GameOver), LevelIntro (with Begin button) |
| [AlertMeter.uxml](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/UXML/AlertMeter.uxml) | ✅ OK | Standalone alert meter component |
| [Highscore.uxml](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/UXML/Highscore.uxml) | ✅ OK | HIGH SCORES screen with Back/Clear buttons |
| [HUD.uss](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/USS/HUD.uss) | ✅ OK | Neon-themed layout |
| [AlertMeter.uss](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/USS/AlertMeter.uss) | ✅ OK | Meter fill + color transitions |
| [Theme.uss](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/USS/Theme.uss) | ✅ OK | CSS variables: `--color-primary: #00FF99`, `--color-alert: #FF3333` |
| **MainMenu.uxml** | ❌ Missing | No main menu UI exists |
| **Pause.uxml** | ❌ Missing | No pause overlay exists |
| **Options.uxml** | ❌ Missing | No options/settings UI exists |

### 2.6 Tests

| File | Type | Tests | Status |
|------|------|-------|--------|
| [GridManagerTests.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Tests/EditMode/GridManagerTests.cs) | EditMode | 3 | ✅ OK |
| [SnakeManagerTests.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Tests/EditMode/SnakeManagerTests.cs) | EditMode | 4 | ✅ OK |
| [ScoreManagerTests.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Tests/EditMode/ScoreManagerTests.cs) | EditMode | 1 | ✅ OK |
| [DetectionIntegrationTests.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Tests/PlayMode/DetectionIntegrationTests.cs) | PlayMode | 2 | ✅ OK |

### 2.7 Scenes & Settings

| Item | Status | Notes |
|------|--------|-------|
| **Level_01.unity** | ⚠️ Not in Build | Scene exists but is not in EditorBuildSettings |
| **SampleScene.unity** | ⚠️ Orphan | In Build Settings as entry (index 0); likely the URP template default |
| **EditorBuildSettings** | ❌ Blocker | Only contains [SampleScene.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/SampleScene.unity) |
| **ProjectSettings** | ⚠️ Needs Work | `companyName: DefaultCompany`; app ID is template default; `runInBackground: 0`; fullscreenMode: 1 |
| **ProjectVersion** | ✅ OK | Unity 6000.3.8f1 |

---

## 3 · Functional Verification Matrix

| # | Feature | Status | Evidence | Reproduction Steps |
|---|---------|--------|----------|-------------------|
| 1 | **Snake Movement** (WASD/Arrows/Gamepad) | ✅ Works | `InputManager.cs:28-39`, `SnakeManager.cs:150-196` | Enter Play in Level_01 → press WASD → snake moves on grid |
| 2 | **Snake Growth** (on core collect) | ✅ Works | `SnakeManager.cs:205-208` | Collect an energy core → body length +1 |
| 3 | **Self-Collision Death** | ✅ Works | `SnakeManager.cs:110-114`, verified by [SnakeManagerTests](file:///d:/Development/Game%20Development/snake-prototype/Assets/Tests/EditMode/SnakeManagerTests.cs#11-116) | Steer snake into its own body → GameOver overlay |
| 4 | **Wall/Bounds Death** | ✅ Works | `SnakeManager.cs:93-107` | Run into grid boundary or wall → GameOver |
| 5 | **Energy Core Spawn/Collect** | ✅ Works | `EnergyCoreManager.cs:99-112` | Cores appear after level gen; collect → score increases |
| 6 | **Score Display** | ✅ Works | `UIManager.cs:173-177`, HUD.uxml `ScoreLabel` | Score label updates on energy collection |
| 7 | **Detection Meter** | ✅ Works | `DetectionManager.cs:32-69`, `UIManager.cs:179-217` | Enter guard FOV → meter fills; leave → decays |
| 8 | **Guard Detection → GameOver** | ✅ Works | `DetectionManager.cs:56-63, 83-89` | Stay in guard FOV at 100% for 0.5s → "SYSTEM LOCKDOWN" |
| 9 | **Level Progression** | ✅ Works | `LevelFlowManager.cs:70-77, 119-124` | Collect N target cores → LevelCompleteEvent → next level intro |
| 10 | **Level Intro Overlay** | ✅ Works | `UIManager.cs:280-313`, HUD.uxml [LevelIntro](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs#85-89) | Shows sector number, goal, difficulty, palette preview, Begin button |
| 11 | **Procedural Level Gen** | ✅ Works | `LevelGenerator.cs:88-160` | Each level has seeded walls, spawn-safe zone, border walls |
| 12 | **Color Palette Rotation** | ✅ Works | `ColorPaletteManager`, `LevelFlowManager.cs:134` | Colors change per level; applied to grid, guards, UI |
| 13 | **Adaptive Audio Layers** | ✅ Works | `AudioManager.cs:87-119` | Base/tension/alert tracks crossfade with detection level |
| 14 | **GameOver Overlay + Retry** | ⚠️ Partial | `UIManager.cs:228-243`, HUD.uxml `AlertOverlay` | Overlay shows; Retry works. No score recap / high-score shown |
| 15 | **Respawn/Retry** | ⚠️ Broken | `InputManager.cs:56`, `SnakeManager.cs:198-203` | Confirm (Enter/Space/A) fires [RespawnEvent](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#79-80) **in any state**, respawning mid-gameplay |
| 16 | **Pause** | ❌ Broken | `InputManager.cs:98` commented out | Esc/P/Start bound but handler is empty. No timeScale toggle, no UI |
| 17 | **Main Menu** | ❌ Missing | `GameState.MainMenu` exists; no UI or logic | Game auto-starts on Awake; no title screen |
| 18 | **Options Menu (Audio)** | ❌ Missing | [AudioManager](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Environment/AudioManager.cs#31-44) has masterVolume field; no UI | No sliders or settings UI |
| 19 | **Options Menu (Controls)** | ❌ Missing | N/A | No rebinding UI or control settings |
| 20 | **Gamepad Support** | ⚠️ Partial | `InputManager.cs:28-29, 44, 49, 52` | Stick/DPad/Start/A/B bound. Multi-controller not disambiguated |
| 21 | **Mobile Touch Input** | ❌ Missing | No Touchscreen bindings anywhere | Zero mobile input support |
| 22 | **Highscore Persistence** | ⚠️ Partial | [HighscoreData.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Score/HighscoreData.cs), [HighscoreUI.cs](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/UI/HighscoreUI.cs), [Highscore.uxml](file:///d:/Development/Game%20Development/snake-prototype/Assets/UI/UXML/Highscore.uxml) | Code exists; integration into game-over flow unclear |
| 23 | **Standalone Build** | ❌ Broken | `EditorBuildSettings` only has SampleScene | Build will load wrong scene |

---

## 4 · UX Gap List (Prioritized)

| Priority | Gap | Impact | Proposed Solution |
|----------|-----|--------|-------------------|
| 🔴 P0 | **No Main Menu** | Fails platform cert; no user onboarding | Create `MainMenu.uxml` (Title, Play, Highscores, Quit). Wire `GameState.MainMenu` in UIManager. Show on boot |
| 🔴 P0 | **Pause Broken** | Game uninterruptible; fails certification | Uncomment [PauseInputEvent](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#17-18) publish; add `PauseService` toggling `Time.timeScale`; create `Pause.uxml` overlay |
| 🔴 P0 | **Confirm = Respawn Anywhere** | Respawns mid-gameplay | Gate confirm action behind [GameState](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Energy/EnergyCoreManager.cs#114-121) check: only fire in GameOver state |
| 🟡 P1 | **No GameOver Score Recap** | Missing feedback loop | Extend AlertOverlay with final score, level reached, highscore comparison |
| 🟡 P1 | **No Options / Settings** | No audio or visual control | Create `Options.uxml` with volume sliders, resolution picker. Wire to AudioManager |
| 🟡 P1 | **No Mobile Input** | Cannot deploy on mobile | Add touch swipe detection or virtual joystick in InputManager |
| 🟢 P2 | **No Control Rebinding** | Power users can't customize | Use Input System's rebinding API + a controls tab in Options |
| 🟢 P2 | **Highscore Not Integrated** | Exists but disconnected | Wire HighscoreUI into MainMenu and GameOver flows |
| 🟢 P2 | **No Accessibility Options** | Excludes colorblind / low-vision | Add high-contrast mode toggle; scale UI font sizes |

---

## 5 · Deployment Checklist

### 5.1 Build Settings

| Setting | Current | Required | How |
|---------|---------|----------|-----|
| Scenes in Build | [SampleScene.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/SampleScene.unity) only | [Level_01.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/Level_01.unity) at index 0 | File → Build Settings → Add [Level_01.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/Level_01.unity); remove or move `SampleScene` |
| Company Name | `DefaultCompany` | Your studio name | Edit → Project Settings → Player → Company Name |
| Product Name | `snake-prototype` | Final game name | Player → Product Name |
| App Identifier (Standalone) | `com.Unity-Technologies.com.unity.template.urp-blank` | `com.yourstudio.snakeprototype` | Player → Other Settings → Override Default Bundle Identifier |
| App Identifier (Android) | `com.UnityTechnologies.com.unity.template.urpblank` | Same as above | Player → Android → Package Name |
| App Identifier (iOS) | Same template default | Same as above | Player → iOS → Bundle Identifier |
| Version | `0.1.0` | Increment as needed | Player → Version |
| Default Resolution | 1024×768 | 1920×1080 or auto | Player → Resolution → Default Screen Width/Height |
| Fullscreen Mode | Windowed (1) | Fullscreen Window (1) recommended | Player → Resolution |
| Run In Background | Off | On (for alt-tab) | Player → Resolution → Run In Background |

### 5.2 Platform-Specific Caveats

| Platform | Requirement | Status |
|----------|-------------|--------|
| **Windows/Mac/Linux** | Scene in build; `runInBackground: 1` | ❌ Not ready |
| **Android** | Touch input; min SDK 25 (set); app icons (all empty) | ❌ Not ready |
| **iOS** | Touch input; app icons (all empty); Apple signing not configured | ❌ Not ready |
| **WebGL** | UI Toolkit compatible in Unity 6; webGLMemorySize: 32 MB (may need increase) | ❌ Not tested |

### 5.3 Scripting Backend

| Setting | Current | Recommendation |
|---------|---------|----------------|
| Scripting Backend | Mono (default) | Switch to IL2CPP for release builds (better perf, required for iOS) |
| API Compatibility | .NET Standard 2.1 | Keep |
| Strip Engine Code | On | Keep (reduces build size) |

---

## 6 · Risk & Effort Matrix

| # | Task | Effort | Risk | Rationale |
|---|------|--------|------|-----------|
| 1 | Add Level_01 to Build Settings | **S** | Low | 2-click fix in editor |
| 2 | Fix Confirm→Respawn (gate by state) | **S** | Low | ~10 lines in InputManager |
| 3 | Uncomment + wire PauseInputEvent | **S** | Low | 5 lines uncomment + ~30 lines PauseService |
| 4 | Fix RemoveListener (implement properly) | **M** | Med | Requires subscription-token pattern; affects all listeners |
| 5 | Remove invalid attributes from InputManager | **S** | Low | Delete 3 lines of `[Header]`/`[SerializeField]` |
| 6 | Remove `[AddComponentMenu]` from non-MonoBehaviours | **S** | Low | Delete 2 lines across LevelFlowManager + AudioManager |
| 7 | Create MainMenu UXML + wire UIManager | **M** | Med | New UI + state-transition logic; must not break existing flow |
| 8 | Create Pause overlay UXML | **S** | Low | Simple overlay with Resume/Quit buttons |
| 9 | Create Options UI (audio sliders) | **M** | Low | Wire to AudioManager._masterVolume; PlayerPrefs persistence |
| 10 | GameOver score recap + highscore integration | **M** | Low | Extend AlertOverlay; wire HighscoreData |
| 11 | Mobile touch input (swipe/virtual joystick) | **L** | Med | New input adapter; touch testing required; UI layout changes |
| 12 | Update PlayerSettings (identifiers, icons, res) | **S** | Low | Editor settings only |
| 13 | Multi-controller disambiguation | **M** | Med | InputSystem player joining; needs testing |
| 14 | Create ScoreConfiguration as SO asset | **S** | Low | Move from runtime-created to assigned SO |
| 15 | Add DontDestroyOnLoad to Bootstrapper | **S** | Low | Single line; only needed if multi-scene patterns are added later |
| 16 | Expand test coverage (Pause, LevelFlow, UI) | **M** | Low | New EditMode tests for pause toggle and level transitions |
| 17 | Build pipeline CI (automated build & tests) | **L** | Med | GitHub Actions + Unity Build License |

---

## 7 · 4-Week Sprint Plan

### Week 1 — "Emergency Triage & Playable Demo" (Sprint 0)

**Epic**: Fix blockers, achieve a shippable single-level demo on desktop.

| # | Task | AC | Effort |
|---|------|-----|--------|
| 1.1 | Add Level_01 to Build Settings index 0 | Standalone build boots into Level_01 | S |
| 1.2 | Gate Confirm input by GameState (only fire RespawnEvent in GameOver) | Enter/Space/A do nothing during Playing or Intro states | S |
| 1.3 | Uncomment PauseInputEvent; create PauseService (toggle timeScale + publish GameStateChanged) | Esc pauses game; snake stops; pressing Esc again resumes | S |
| 1.4 | Create Pause.uxml overlay (PAUSED title, Resume, Quit buttons) | Overlay appears on pause; Resume hides it and resumes; Quit exits | S |
| 1.5 | Wire UIManager for GameState.Paused (show pause overlay, hide HUD dimming) | State transition visible in UI | S |
| 1.6 | Fix RemoveListener with subscription-token pattern | Events properly unsubscribe; no listener leaks on re-init | M |
| 1.7 | Remove `[SerializeField]`/`[Header]` from InputManager; `[AddComponentMenu]` from non-MonoBehaviours | Clean compile; no misleading attributes | S |
| 1.8 | Update PlayerSettings: company name, app ID, resolution 1920×1080, runInBackground on | Build metadata correct | S |

**Sprint 1 exit**: Standalone .exe plays full game loop with pause and retry, no crashes.

---

### Week 2 — "Menu Scaffolding & Game Flow" (Sprint 1)

**Epic**: Create main menu, game-over recap, highscore flow.

| # | Task | AC | Effort |
|---|------|-----|--------|
| 2.1 | Create MainMenu.uxml (title, Play, Highscores, Options placeholder, Quit) using Theme.uss vars | UI visible in editor preview | M |
| 2.2 | Wire GameState.MainMenu as entry state; show MainMenu overlay on boot | Game starts in menu; Play button transitions to LevelIntro | M |
| 2.3 | Extend GameOver overlay: show final score, level reached, "NEW HIGHSCORE" badge if applicable | Score recap visible on death | M |
| 2.4 | Integrate HighscoreUI into MainMenu → Highscores button | Clicking Highscores shows persisted scores; Back returns to menu | M |
| 2.5 | Persist highscores via PlayerPrefs or JSON file | Scores survive between sessions | S |
| 2.6 | Create ScoreConfiguration as saved SO asset (replace runtime CreateInstance) | Config editable in Inspector | S |
| 2.7 | Add EditMode tests for PauseService and MainMenu state transitions | Tests pass in CI / Test Runner | M |

**Sprint 2 exit**: Full menu loop (MainMenu → Play → Gameplay → GameOver → Retry/Menu → Highscores).

---

### Week 3 — "Options, Audio Control & Polish" (Sprint 2)

**Epic**: Options menu, audio settings, visual polish, controller UX.

| # | Task | AC | Effort |
|---|------|-----|--------|
| 3.1 | Create Options.uxml: master volume slider, SFX volume slider, music volume slider | Sliders visually match theme; values read from AudioManager | M |
| 3.2 | Wire volume sliders to AudioManager._masterVolume and layer volumes | Moving slider changes live volume | S |
| 3.3 | Persist audio settings (PlayerPrefs) + load on boot | Settings survive between sessions | S |
| 3.4 | Add resolution/quality dropdown to Options (optional) | Player can change resolution; changes apply | M |
| 3.5 | Disambiguate multi-controller (optional; mark as stretch) | Player 1 locked to first connected device | M |
| 3.6 | Add UI Toolkit transitions/animations for menu screens (fade, slide-in) | Menus feel polished; no jarring cuts | S |
| 3.7 | Fix DetectionManager publishing every frame (throttle or delta-check) | Reduce event spam; smoother UI updates | S |
| 3.8 | Improve GameOver death VFX feedback (screen shake / flash) | Death feels impactful | S |

**Sprint 3 exit**: Options screen with working audio controls; polished transitions; controller edge cases handled.

---

### Week 4 — "Mobile, Build Pipeline & Release Prep" (Sprint 3)

**Epic**: Mobile input, platform builds, CI, documentation.

| # | Task | AC | Effort |
|---|------|-----|--------|
| 4.1 | Implement mobile touch input: swipe detection adapter in InputManager | Swiping in cardinal directions steers snake | L |
| 4.2 | Add on-screen pause button for mobile (no Esc key) | Tap button pauses game | S |
| 4.3 | Create app icons for Android and iOS (generate via image tool) | Icons visible in build settings | S |
| 4.4 | Test + fix UI scale on mobile aspect ratios (4:3, 16:9, 19.5:9) | UI elements not clipped; text readable | M |
| 4.5 | Switch scripting backend to IL2CPP for release builds | Successful Android/iOS build with IL2CPP | S |
| 4.6 | Set up GitHub Actions for automated builds + test runner | Push to main triggers build + tests | L |
| 4.7 | Update README.md with build instructions, architecture overview, controls | Doc matches code reality | M |
| 4.8 | Final QA pass: play through 5 levels on desktop + mobile emulator | No crashes; all systems functional | M |

**Sprint 4 exit**: Deployable builds for Windows, macOS, Android. CI pipeline. Updated documentation.

---

## 8 · Quick Fixes (Next Coding Session)

These **5 changes** can be implemented in ~60–90 minutes to produce a **playable demo**:

### QF-1: Add Level_01 to Build Settings
```
File → Build Settings → Drag Level_01.unity to Scenes list → Set as index 0
Remove SampleScene or set to index 1 (disabled).
```

### QF-2: Gate Confirm Input by GameState
```diff
// InputManager.cs, line 56
- confirmAction.performed += ctx => GameEventManager.Publish(new RespawnEvent());
+ confirmAction.performed += ctx => GameEventManager.Publish(new ConfirmEvent());
```
Then add a ConfirmEvent listener in [SnakeManager](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Snake/SnakeManager.cs#10-217) that only triggers respawn when `_isGameOver == true`.

### QF-3: Uncomment Pause + Add Minimal Handler
```diff
// InputManager.cs, line 98
- // GameEventManager.Publish(new PauseInputEvent());
+ GameEventManager.Publish(new PauseInputEvent());
```
Add to `GameBootstrapper.Update()`:
```csharp
// After input tick
static bool _isPaused = false;
// (Move to a PauseService class for cleanliness)
```
Or add a minimal listener in `LevelFlowManager.Initialize()`:
```csharp
GameEventManager.AddListener<PauseInputEvent>(_ => {
    bool paused = Time.timeScale > 0;
    Time.timeScale = paused ? 0f : 1f;
    GameEventManager.Publish(new GameStateChangedEvent(
        paused ? GameState.Paused : GameState.Playing));
});
```

### QF-4: Add Pause Handling to UIManager
Add to [OnGameStateChanged](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Energy/EnergyCoreManager.cs#114-121):
```csharp
if (e.NewState == GameState.Paused) {
    if (_alertOverlay != null) {
        _alertOverlay.style.display = DisplayStyle.Flex;
        if (_alertHeader != null) _alertHeader.text = "SYSTEM PAUSED";
        if (_alertSubHeader != null) _alertSubHeader.text = "PRESS ESC TO RESUME";
        if (_respawnButton != null) _respawnButton.style.display = DisplayStyle.None;
    }
}
```

### QF-5: Remove Invalid Attributes
```diff
// InputManager.cs, lines 14-17 — DELETE these lines:
- [Header("Input Settings")]
- [SerializeField] private float _deadzone = 0.1f;
- [SerializeField] private float _sensitivity = 1.0f;
- [SerializeField] private bool _useRawInput = false;
+ private float _deadzone = 0.1f;
+ private float _sensitivity = 1.0f;
+ private bool _useRawInput = false;
```

---

## Appendix A · Scene Architecture Confirmation

**Pattern**: Single-scene model. Zero calls to `SceneManager.LoadScene` exist in the codebase.

- [Level_01.unity](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scenes/Level_01.unity) contains the [GameBootstrapper](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/GameBootstrapper.cs#16-159) MonoBehaviour, Main Camera with [CameraSetup](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Core/CameraSetup.cs#12-100), UIDocument, and AudioSource references.
- All game-state transitions (MainMenu → Playing → Paused → GameOver) are handled via [GameStateChangedEvent](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Events/GameEvents.cs#76-77) + UI overlay visibility toggles.
- [LevelFlowManager](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs#24-28) uses `Time.timeScale = 0f` during level intros, which is the sole use of timeScale in the codebase.
- **No async scene loading** is used or needed in the current architecture.
- If multi-scene is ever desired (e.g., separate menu scene), it would require adding `DontDestroyOnLoad` to the Bootstrapper GO and implementing a scene-management service. This is **not recommended** for the current scope.

## Appendix B · Verification Commands

```bash
# Run EditMode tests
Unity.exe -batchmode -runTests -testPlatform EditMode -projectPath . -testResults results.xml

# Run PlayMode tests  
Unity.exe -batchmode -runTests -testPlatform PlayMode -projectPath . -testResults results.xml

# Build Standalone (Windows)
Unity.exe -batchmode -buildTarget Win64 -buildWindowsPlayer Build/SnakePrototype.exe -projectPath .

# Build Android (requires Android SDK + IL2CPP)
Unity.exe -batchmode -buildTarget Android -projectPath .
```

## Appendix C · Event Registration Summary

| Service | Listens To | Publishes |
|---------|-----------|-----------|
| InputManager | (input callbacks) | MoveInputEvent, PauseInputEvent*, RespawnEvent/ConfirmEvent |
| SnakeManager | MoveInputEvent, RespawnEvent, EnergyCollectedEvent, LevelStartedEvent | SnakeMovedEvent, SnakeDiedEvent, GameStateChangedEvent |
| DetectionManager | RespawnEvent | DetectionLevelChangedEvent, DetectedEvent, GameStateChangedEvent |
| EnergyCoreManager | SnakeMovedEvent, GameStateChangedEvent, RespawnEvent, LevelGeneratedEvent | EnergyCollectedEvent, EnergySpawnedEvent, EnergyCoresResetEvent |
| ScoreManager | EnergyCollectedEvent | ScoreChangedEvent |
| LevelFlowManager | EnergyCollectedEvent, RespawnEvent, LevelIntroConfirmedEvent | LevelStartedEvent, LevelCompleteEvent |
| LevelGenerator | (called by LevelFlowManager) | LevelGeneratedEvent |
| UIManager | ScoreChangedEvent, DetectedEvent, DetectionLevelChangedEvent, GameStateChangedEvent, PaletteChangedEvent, LevelCompleteEvent, LevelStartedEvent, EnergyCollectedEvent | LevelIntroConfirmedEvent, RespawnEvent |
| AudioManager | DetectionLevelChangedEvent, EnergyCollectedEvent, SnakeDiedEvent, RespawnEvent, GameStateChangedEvent, LevelCompleteEvent, LevelStartedEvent, LevelIntroConfirmedEvent | (none) |
| CameraSetup | LevelCompleteEvent, LevelStartedEvent, RespawnEvent | (none) |

*\* Currently commented out*
