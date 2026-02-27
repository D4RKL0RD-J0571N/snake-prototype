# Snake Prototype — Level Design & Game Flow Audit

> **Project**: snake-prototype · **Engine**: Unity 6000.3.8f1 (URP)  
> **Audit scope**: Procedural generation, difficulty progression, environmental storytelling, player flow  
> **Audit date**: 2026-02-27 · **Baseline**: [deep_audit_report.md](file:///d:/Development/Game%20Development/snake-prototype/Docs/deep_audit_report.md), [ui_ux_input_audit.md](file:///d:/Development/Game%20Development/snake-prototype/Docs/ui_ux_input_audit.md)

---

## 1 · Executive Summary

Snake Prototype is a **stealth-collection arcade game** built on a retro terminal-tech aesthetic, where the player navigates a grid-based procedural level collecting energy cores while evading detection by patrolling guards. The current gameplay loop is functional: `LevelIntro → Play → Collect N Cores → LevelComplete → Next Level` (or `Die → Retry same level`). The `LevelFlowManager` drives progression across an infinite level sequence with a linear difficulty ramp that scales wall density (4%→20%), guard count (1→N/2), and core targets (3→3+2N). Four color themes (`Preset`, `RandomHue`, `Complementary`, `HighContrast`) rotate every 4 levels, providing visual novelty. Audio, lighting, and VFX layers react dynamically to detection stress, creating a responsive atmosphere.

However, the **game flow model is incomplete**: there is no main menu entry point, no pause system, and death/retry bypasses any meaningful pacing. The procedural generator uses **uniform random wall scatter** (no room/corridor structure), meaning levels feel like random noise rather than designed spaces. Guard patrol is limited to a single waypoint (stationary pivot), and energy cores spawn without spatial reasoning — no clustering, no narrative placement, no relationship to guard coverage. The difficulty ramp is purely linear with no plateau, recovery, or player-skill adaptation, leading to a relentless squeeze that will frustrate mid-skill players around Level 6–7.

This audit defines a **10-level progression curve**, **4 environmental themes**, a **procedural parameter matrix**, and a **player flow model** that can be implemented entirely through data-driven tuning of existing systems — no architectural rewrites required. The goal is to transform the current "infinite grind" into a **rhythmic arc with tension/release cycles**, environmental variety, and a clear sense of mastery and narrative progression.

---

## 2 · Progression Curve

### 2.1 Intended Gameplay Loop

```
┌──────────────────────────────────────────────────────────────────┐
│  MainMenu ──► LevelIntro ──► Playing ──► LevelComplete ──┐      │
│      ▲                          │                         │      │
│      │                      GameOver ──► Retry ──┐        │      │
│      │                                           │        │      │
│      └───────── Quit / Highscores ◄──────────────┴────────┘      │
└──────────────────────────────────────────────────────────────────┘
```

- **Session target**: 15–25 minutes for a full 10-level run (skilled player), 8–12 minutes for average player reaching Level 5–6.
- **Level duration**: 45s (early) → 120s (late), dictated by core count and spatial complexity.
- **Tension cycle**: Each level follows **Explore → Collect → Evade → Complete**, with tension peaking mid-level when guards patrol near the remaining cores.

### 2.2 Level Curve Table (Levels 1–10)

| Lvl | Grid | Wall% | Walls† | Guards | Guard Speed | View Range | Core Target | Initial Cores | Theme | Mood | Est. Duration |
|-----|------|-------|--------|--------|-------------|------------|-------------|---------------|-------|------|---------------|
| 1 | 20×20 | 3% | ~12 | 1 | 1.0 | 6 | 3 | 3 | Industrial | Tutorial calm | 30–45s |
| 2 | 22×22 | 5% | ~24 | 1 | 1.2 | 7 | 4 | 3 | Industrial | First challenge | 40–55s |
| 3 | 24×24 | 7% | ~40 | 2 | 1.5 | 8 | 5 | 3 | Laboratory | Rising pressure | 50–70s |
| 4 | 24×24 | 9% | ~52 | 2 | 1.8 | 8 | 6 | 4 | Laboratory | Maze practice | 55–75s |
| 5 | 26×26 | 10% | ~68 | 3 | 2.0 | 9 | 7 | 4 | Server Room | Midpoint spike | 60–90s |
| 6 | 26×26 | 8% | ~54 | 2 | 1.8 | 8 | 6 | 4 | Server Room | _Breather_ | 50–70s |
| 7 | 28×28 | 12% | ~94 | 3 | 2.2 | 10 | 8 | 5 | Maintenance | Dense navigation | 70–100s |
| 8 | 28×28 | 14% | ~110 | 4 | 2.5 | 10 | 9 | 5 | Maintenance | Peak tension | 80–110s |
| 9 | 30×30 | 16% | ~144 | 4 | 2.5 | 11 | 10 | 5 | Final Sector | Pre-boss gauntlet | 90–120s |
| 10 | 30×30 | 12% | ~108 | 5 | 3.0 | 12 | 12 | 6 | Final Sector | Boss — many guards, open space | 100–130s |

† = approximate wall count at given density for the grid area.

### 2.3 Design Principles

1. **Breather at Level 6**: After the midpoint spike (Lvl 5), Level 6 deliberately relaxes wall density and guard count, giving the player a psychological reset before the second-half escalation.
2. **Variable grid sizes**: Smaller grids early create approachability; larger grids later demand navigation planning.
3. **Non-linear wall density**: Level 10 reduces wall density vs Level 9 to create open arenas where guards are the primary threat, not walls.
4. **Core surplus → deficit**: Early levels have `cores_spawned >= target` immediately; later levels respawn cores one-at-a-time forcing repeated exploration.

### 2.4 Win/Lose Definitions

| Condition | Trigger | Result |
|-----------|---------|--------|
| **Level Win** | Collect `TargetCores` in a level | `LevelCompleteEvent` → advance to next Level Intro |
| **Level Lose (Detection)** | Detection meter at 100% for ≥0.5s | `DetectedEvent` → `GameOver` state |
| **Level Lose (Collision)** | Snake hits wall, border, or self | `SnakeDiedEvent` → `GameOver` state |
| **Game Win** | Complete Level 10 | Victory screen (proposed; currently loops infinitely) |
| **Session End** | Player quits from Main Menu or Pause | Save highscore, return to desktop |

---

## 3 · Level Theme Bible

### 3.1 Industrial (Levels 1–2)

**Narrative**: The snake program has just been deployed into a legacy mainframe. The environment is a **cold, grid-lined factory floor** with minimal surveillance. This is the onboarding phase — the system is old, neglected, and barely monitored.

**Mood**: Calm, mechanical, slightly eerie. Low hum of machinery.  
**Palette**: `Preset` — cyan primary (`#00FF99`), black background, dark gray grid.  
**Audio**: Slow ambient drone, minimal tension layers; collection SFX is a clean digital *ping*.  
**Visual Elements**: Sparse walls (low density), dim static lighting, single stationary guard.  
**Storytelling Cues**: UXML Level Intro reads "SECTOR 01: INDUSTRIAL ZONE // SECURITY: MINIMAL". Occasional flicker on grid lines suggests aging infrastructure.

### 3.2 Laboratory (Levels 3–4)

**Narrative**: The snake has infiltrated a mid-security research wing. Equipment is more sophisticated, surveillance has doubled, and the pathways are more convoluted.

**Mood**: Clinical, bright spots amid darkness, focused scanning beams.  
**Palette**: `RandomHue` — shifts per level via HSV generation; tends toward blue-green.  
**Audio**: Tension layer begins crossfading in; heartbeat-like bass when detection reaches 40%.  
**Visual Elements**: Increased wall clusters forming room-like pockets; guards begin shallow patrol (2-waypoint routes); energy cores clustered in "lab stations."  
**Storytelling Cues**: "SECTOR 03: LABORATORY // ALERT LEVEL: ELEVATED". Warning labels on walls: `⚠ RESTRICTED`. Detection cone visualization becomes slightly wider.

### 3.3 Server Room (Levels 5–6)

**Narrative**: Deep in the facility's data center. Rows of server racks create narrow corridors with limited sightlines. This is the **midpoint crucible** — the player must master evasion or fail.

**Mood**: Dense, claustrophobic, buzzing with electromagnetic noise.  
**Palette**: `Complementary` — contrasting hues create visual tension; warm-vs-cool split.  
**Audio**: Full 3-layer audio engaged; alert music is aggressive synth. Low-pass filter engages during pause.  
**Visual Elements**: Wall patterns form parallel corridors (stripe patterns); guards have 3-waypoint patrol routes; energy cores are scarce and placed at corridor ends.  
**Storytelling Cues**: "SECTOR 05: SERVER CORE // WARNING: ACTIVE MONITORING". Level 6 intro shows "MAINTENANCE CYCLE: PARTIAL SHUTDOWN" to signal the breather.

### 3.4 Maintenance / Final Sector (Levels 7–10)

**Narrative**: The snake approaches the central processing core. Infrastructure is unstable — walls shift between dense and open configurations. Maximum guard presence. This is the endgame gauntlet.

**Mood**: Unstable, intense, final-push urgency.  
**Palette**: `HighContrast` — black background, white grid, pure-hue accents; maximum readability under stress.  
**Audio**: All layers at maximum. Death SFX triggers screen flash + bass drop.  
**Visual Elements**: Mixed density creates open arenas surrounded by maze wings; guards use longest patrol routes (4+ waypoints); lighting pulses intensely at high detection.  
**Storytelling Cues**: "SECTOR 09: CORE ACCESS // THREAT LEVEL: CRITICAL". Level 10 intro: "FINAL PROTOCOL: SYSTEM OVERRIDE". Victory text: "EXTRACTION COMPLETE."

---

## 4 · Procedural Parameters Matrix

### 4.1 Current System Audit

| Component | Implementation | Status | Gap |
|-----------|---------------|--------|-----|
| **Wall Placement** | Uniform random (`rng.NextDouble() < WallProbability`) | ✅ Works | No structural patterns (rooms, corridors) — feels like noise |
| **Spawn Safety** | 5×5 zone around (5,5) kept clear; right-of-spawn corridor | ✅ Works | Hardcoded to (5,5); no parametric spawn location |
| **Border Walls** | Full perimeter at grid edges, instantiated as prefabs | ✅ Works | OK |
| **Guard Spawning** | `GridManager.GetRandomSpawnPoints()` → random free cells | ⚠️ Partial | Uses `UnityEngine.Random` not seeded RNG; guards only get 1 waypoint (stationary) |
| **Guard Patrol** | `PatrolAgent` with `List<Vector2Int> Waypoints` | ⚠️ Partial | Only 1 waypoint assigned by generator → guards are stationary pivots |
| **Energy Spawn** | `EnergyCoreManager.SpawnCore()` — random non-wall, non-snake | ✅ Works | No spatial reasoning; cores can cluster or be unreachable behind wall RNG |
| **Color Theme** | `(levelIndex - 1) % 4` cycling through 4 `ColorThemeMode` values | ✅ Works | OK, but theme doesn't encode narrative meaning |
| **Difficulty Ramp** | `ApplyDifficultyRamp()` — linear formulas mutate `LevelConfig` in-place | ⚠️ Fragile | Mutates SO at runtime; no ceiling/floor caps except `WallProbability` Clamp |
| **Seed** | `DateTime.UtcNow.Ticks ^ _currentLevel` | ⚠️ Partial | Different on every run; palette seed is deterministic (`42 + level * 777`) |

### 4.2 Recommended Parameters by Difficulty Tier

| Parameter | Tier 1 (Lv1–2) | Tier 2 (Lv3–4) | Tier 3 (Lv5–6) | Tier 4 (Lv7–8) | Tier 5 (Lv9–10) |
|-----------|:-:|:-:|:-:|:-:|:-:|
| **Grid Width** | 20 | 24 | 26 | 28 | 30 |
| **Grid Height** | 20 | 24 | 26 | 28 | 30 |
| **WallProbability** | 0.03–0.05 | 0.07–0.09 | 0.08–0.10 | 0.12–0.14 | 0.12–0.16 |
| **GuardCount** | 1 | 2 | 2–3 | 3–4 | 4–5 |
| **Guard.MoveSpeed** | 1.0 | 1.5–1.8 | 1.8–2.0 | 2.2–2.5 | 2.5–3.0 |
| **Guard.ViewRange** | 6 | 8 | 8–9 | 10 | 11–12 |
| **Guard.ViewAngle** | 45° | 45° | 60° | 60° | 75° |
| **Guard Waypoints** | 1 (stationary) | 2 (short patrol) | 2–3 | 3–4 | 4+ (long patrol) |
| **TargetCores** | 3 | 5–6 | 6–7 | 8–9 | 10–12 |
| **InitialCores** | 3 | 3 | 4 | 5 | 5–6 |
| **DetectionDecay** | 15 | 15 | 12 | 10 | 8 |
| **StressThreshold** | 0.5s | 0.5s | 0.5s | 0.4s | 0.3s |
| **ColorTheme** | Preset | RandomHue | Complementary | HighContrast | HighContrast |

### 4.3 Implementation Strategy

All parameters above can be driven by a `DifficultyTable` — a `ScriptableObject` containing a `LevelTierData[]` array indexed by `Mathf.Clamp(levelIndex, 1, tiers.Length)`. The existing `ApplyDifficultyRamp()` in `LevelFlowManager` would read from this table instead of using inline math:

```csharp
// Replace LevelFlowManager.ApplyDifficultyRamp() with table lookup:
var tier = _difficultyTable.GetTier(_currentLevel);
_config.Width = tier.GridWidth;
_config.Height = tier.GridHeight;
_config.WallProbability = tier.WallProbability;
_config.GuardCount = tier.GuardCount;
_config.TargetCores = tier.TargetCores;
_config.ColorTheme = tier.ColorTheme;
```

---

## 5 · Player Flow Model

### 5.1 Typical Player Movement Pattern

```
                    ┌──────────── EXPLORE ────────────┐
                    │                                  │
                    ▼                                  │
              ┌──────────┐     ┌──────────┐     ┌──────────┐
  SPAWN ────► │ SAFE ZONE│────►│ CORRIDOR │────►│ OPEN AREA│
  (5,5)       │ (start)  │     │ (walls)  │     │ (cores?) │
              └──────────┘     └──────────┘     └──────────┘
                    │                │                │
                    │           ┌────┴────┐          │
                    │           │  GUARD   │          │
                    │           │  PATROL  │          │
                    │           └────┬────┘          │
                    │                │                │
                    │           DETECTION?            │
                    │            ┌──┴──┐              │
                    │           YES    NO             │
                    │            │      │              │
                    │        EVADE   COLLECT           │
                    │            │      │              │
                    │            └──┬──┘              │
                    │               │                  │
                    └───────────────┴──────────────────┘
                                    │
                              ALL CORES?
                             ┌───┴───┐
                            YES      NO
                             │        │
                        COMPLETE   CONTINUE
```

### 5.2 Decision Points & Spatial Design

| Decision | Location | Player Choice | Design Lever |
|----------|----------|--------------|--------------|
| **Route Selection** | Corridor junction | Go left (guard) or right (wall maze) | Wall pattern creates 2+ viable paths |
| **Risk Assessment** | Approaching guard zone | Wait for patrol pass or rush through | Guard waypoint timing and detection range |
| **Core Prioritization** | Multiple cores visible | Closest core vs safest core | Core placement relative to guard coverage |
| **Escape Planning** | Detection meter rising | Continue collecting or retreat | Decay rate and stress threshold |
| **Dead-End Gambit** | Cul-de-sac with core | Enter dead end for reward, risk trapped | Wall patterns creating recognizable alcoves |

### 5.3 Recommended Corridor/Room Ratios

| Tier | Corridors (%) | Open Rooms (%) | Dead Ends (%) | Design Feel |
|------|:---:|:---:|:---:|---|
| 1–2 | 30% | 60% | 10% | Open, explorable, forgiving |
| 3–4 | 45% | 40% | 15% | Structured, room-based |
| 5–6 | 55% | 25% | 20% | Corridor-heavy, claustrophobic |
| 7–8 | 50% | 30% | 20% | Mixed, unpredictable |
| 9–10 | 40% | 45% | 15% | Open arenas with guard threat |

> [!NOTE]
> Achieving these ratios requires upgrading from uniform random walls to a **pattern-based generator** (recommended: Room → Corridor → Junction pipeline). This can be done incrementally by adding a post-processing pass after the existing random wall placement.

### 5.4 Orientation & Landmarks

The current generator provides no landmarks — every cell looks the same except for wall/no-wall. For spatial orientation, recommend:

| Landmark Type | Placement Logic | Purpose |
|---------------|----------------|---------|
| **Light Clusters** | 1 per quadrant, placed on empty cells far from spawn | Quadrant identification |
| **Warning Signs** | Adjacent to guard patrol routes | Threat proximity cues |
| **Beacon Cores** | First energy core placed at farthest navigable point from spawn | Establish player direction |
| **Grid Color Gradient** | Subtle HSV shift across grid X-axis using `Palette.GridAccent` + offset | Subconscious orientation |

---

## 6 · Testing & Validation

### 6.1 Playtest Metrics

| Metric | Formula | Target (Lv1) | Target (Lv5) | Target (Lv10) |
|--------|---------|:---:|:---:|:---:|
| **Avg Completion Time** | `Time.time - levelStartTime` at LevelComplete | 30–45s | 60–90s | 100–130s |
| **Death Rate** | `deaths / totalAttempts` per level | ≤20% | 40–50% | 60–70% |
| **Cores/Minute** | `coresCollected / (timeAlive / 60)` | 4–6 | 3–5 | 2–4 |
| **Detection Incidents** | Count of `DetectionLevel > 50` per level | 0–1 | 2–3 | 3–5 |
| **Detection Deaths** | `DetectedEvent` / `totalDeaths` ratio | ≤30% | 40–50% | 50–60% |
| **Wall Deaths** | `SnakeDied("Wall")` / `totalDeaths` ratio | 50–60% | 30–40% | 20–30% |
| **Self-Collision Deaths** | `SnakeDied("Self-Collision")` / `totalDeaths` | 20–30% | 15–25% | 10–20% |
| **Retry Rate** | `retries / totalLevelAttempts` | ≤1.5 | ≤3.0 | ≤5.0 |

### 6.2 Visualization Methods

| Visualization | Data Source | Implementation |
|---------------|-----------|----------------|
| **Death Heatmap** | `SnakeDiedEvent.Position` aggregated across N runs | 2D grid overlay, color-coded by death frequency per cell |
| **Movement Heatmap** | `SnakeMovedEvent.HeadPosition` sampled per tick | Reveals player paths, ignored areas, common routes |
| **Detection Pressure Map** | `DetectionLevelChangedEvent` correlated with snake position | Shows where detection spikes occur spatially |
| **Guard Coverage Map** | `DetectionSource.ViewRange + ViewAngle` projected on grid | Identifies safe corridors vs danger zones |
| **Core Collection Timeline** | `EnergyCollectedEvent` timestamps per level | Shows collection pacing — bursts vs steady |

> [!TIP]
> All visualizations can be built as **Editor-only gizmo** classes consuming the existing event bus. No runtime overhead. Simplest MVP: accumulate positions in a `static Dictionary<Vector2Int, int>`, render with `OnDrawGizmos`.

### 6.3 Acceptance Criteria

| # | Criterion | Pass Condition |
|---|-----------|---------------|
| AC1 | **Level generates without errors** | 100 seeded runs per tier produce no `LogError` or exceptions |
| AC2 | **Spawn zone is always clear** | Snake at (5,5) with 3-block right corridor has 0% chance of immediate wall death |
| AC3 | **All cores reachable** | Flood-fill from spawn covers all energy core positions (no walled-off cores) |
| AC4 | **Guard patrol routes are valid** | All waypoints are on non-wall cells within grid bounds |
| AC5 | **Level is completable** | At least 1 valid path exists from spawn to each core location |
| AC6 | **Difficulty scales per table** | Parameters at Lv1 match Tier 1 column; Lv5 → Tier 3; Lv10 → Tier 5 |
| AC7 | **Theme rotates correctly** | Palette mode at Lv1 = Preset, Lv3 = RandomHue, Lv5 = Complementary, Lv7 = HighContrast |
| AC8 | **Completion time within target** | 90th percentile of bot playthroughs within ±30% of target duration |
| AC9 | **Detection decay feels fair** | Player escaping guard FOV sees meter drop to 0 within 4–7 seconds |
| AC10 | **No softlocks** | Snake cannot be trapped with 0 valid moves (excluding intentional dead-ends) |

### 6.4 Automated Test Proposals

```csharp
// EditMode Tests → LevelGenerationTests.cs
[Test] public void Level_SpawnZone_IsAlwaysClear() { /* Generate 100 seeds, assert cells around (5,5) == false */ }
[Test] public void Level_AllCores_AreReachable() { /* Flood-fill from (5,5), assert all core positions in fill set */ }
[Test] public void Level_WallDensity_MatchesTier() { /* Generate per-tier, count walls, assert within ±15% of expected */ }
[Test] public void Level_GuardCount_MatchesTier() { /* Generate, count guards, assert == tier.GuardCount */ }
[Test] public void Level_GridSize_MatchesTier() { /* Assert gridManager.Width/Height == tier values */ }
```

---

## 7 · Recommendations (Prioritized)

| # | Priority | Recommendation | Effort | Impact | Systems Affected |
|---|:---:|---|:---:|:---:|---|
| 1 | 🔴 P0 | **Create `DifficultyTable` ScriptableObject** — Replace inline difficulty math in `ApplyDifficultyRamp()` with a lookup table keyed by level index. Include grid size, wall%, guards, core targets, theme mode, guard speed/range. | S | High | `LevelFlowManager`, new `DifficultyTable.cs` SO |
| 2 | 🔴 P0 | **Assign multiple waypoints to guards** — Modify `LevelGenerator.SpawnGuards()` to generate 2–4 waypoints per guard (adjacent free cells radiating from spawn point). Pass RNG for determinism. | M | High | `LevelGenerator`, `PatrolAgent`, `GridManager` |
| 3 | 🟡 P1 | **Add reachability validation** — After wall generation, run flood-fill from (5,5). Remove any wall that isolates >10% of free cells. Guarantees all cores are reachable. | M | High | `LevelGenerator` (post-process pass) |
| 4 | 🟡 P1 | **Pass seeded RNG to `GridManager.GetRandomSpawnPoints()`** — Currently uses `UnityEngine.Random`, breaking seed determinism for guard placement. Pass `System.Random` instance. | S | Med | `GridManager`, `LevelGenerator` |
| 5 | 🟡 P1 | **Scale guard `ViewAngle` and `ViewRange` per tier** — Currently `DetectionSource` uses hardcoded `ViewAngle=45°` and `ViewRange=10`. Read values from `DifficultyTable` and apply during guard spawn. | S | Med | `DetectionSource`, `LevelGenerator` |
| 6 | 🟡 P1 | **Add Level 6 "breather" mechanic** — Explicitly reduce difficulty at Level 6 (lower wall%, fewer guards) to create a tension/release cycle. Encode in `DifficultyTable`. | S | Med | `DifficultyTable` data only |
| 7 | 🟢 P2 | **Implement victory condition at Level 10** — Add a `GameState.Victory` enum value, trigger it when Level 10 is completed, show a "EXTRACTION COMPLETE" overlay instead of advancing to Level 11. | S | Med | `LevelFlowManager`, `GameEvents`, `UIManager` |
| 8 | 🟢 P2 | **Add room/corridor structure patterns** — Introduce a post-processing step in `LevelGenerator` that carves room shapes (3×3 to 5×5) and connects them with 1-wide corridors. Apply after random wall scatter. | L | High | `LevelGenerator` (new methods) |
| 9 | 🟢 P2 | **Implement death/movement heatmaps** — Create an Editor-only `PlaytestRecorder` class that subscribes to `SnakeDiedEvent` and `SnakeMovedEvent`, accumulates position data, and renders as gizmos. | M | Med | New `PlaytestRecorder.cs` (Editor-only) |
| 10 | 🟢 P2 | **Add spatial landmarks** — Place 4 distinct light objects (one per grid quadrant) during generation to provide orientation cues. Use palette accent colors with slight variation per quadrant. | S | Med | `LevelGenerator` (new method: `SpawnLandmarks()`) |

### Implementation Sequencing

```
Phase 1 (Week 1): Rec #1 + #4 + #6 → Data-driven difficulty with deterministic seeding
Phase 2 (Week 2): Rec #2 + #5 + #3 → Guards become dynamic threats, levels are guaranteed fair
Phase 3 (Week 3): Rec #7 + #10 → Victory state and landmarks polish the experience
Phase 4 (Week 4): Rec #8 + #9 → Structural generation and playtest instrumentation
```

---

## Appendix A · Current Difficulty Formulas (For Reference)

From [LevelFlowManager.ApplyDifficultyRamp()](file:///d:/Development/Game%20Development/snake-prototype/Assets/Scripts/Systems/Level/LevelFlowManager.cs#L126):

```csharp
_config.TargetCores = 3 + (_currentLevel - 1) * 2;          // L1=3, L5=11, L10=21 ← too aggressive
_config.GuardCount = 1 + (_currentLevel / 2);                // L1=1, L5=3, L10=6
_config.WallProbability = Clamp(0.04 + level*0.01, 0.04, 0.20); // L1=5%, L5=9%, L10=14%
_config.ColorTheme = (ColorThemeMode)((level - 1) % 4);      // Cycles: 0,1,2,3,0,1,...
```

> [!CAUTION]
> `TargetCores` grows unbounded (21 at Level 10, 41 at Level 20). The difficulty table approach in Rec #1 resolves this by capping target cores per tier. The current formula also mutates the `LevelConfig` ScriptableObject's runtime values in-place, which could cause issues if the SO is shared or inspected.

## Appendix B · System Responsibilities Map

```
LevelFlowManager ──► ApplyDifficultyRamp()  → mutates LevelConfig
                 ──► PrepareNextLevel()      → palette preview + LevelCompleteEvent
                 ──► StartLevel()            → seed + LevelStartedEvent + LevelGenerator.GenerateLevel()

LevelGenerator   ──► GenerateLevel(seed, idx) → grid resize + wall placement + guards + borders + palette
                 ──► SpawnGuards()            → instantiate guards from config.GuardPrefab
                 ──► SpawnBorderWalls()       → perimeter closure
                 ──► ApplyPalette()           → ColorPaletteManager + AudioManager

GridManager      ──► Resize() / SetWall()     → manages bool[,] wall array
                 ──► GetRandomSpawnPoints()    → free-cell selection (non-deterministic)

DetectionSource  ──► CheckForSnake()          → cone-cast detection → DetectionManager.ReportSighting()
PatrolAgent      ──► UpdatePatrol()           → waypoint movement (currently 1 waypoint = stationary)
DetectionManager ──► Tick()                   → aggregate sightings → DetectionLevelChangedEvent
                 ──► StressTimer ≥ 0.5s       → TriggerGameOver()
```
