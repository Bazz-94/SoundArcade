# Application Project Code Review

Scope: `Source/Application` (Program, ArcadeShell, GameRegistry, SceneFactory, ServiceCollectionExtensions, Scenes). Reviewed against `artifacts/standards.md` and CLAUDE.md. Recommendations only — nothing implemented.

## Bugs / Behavior

- [x] **SettingsMenuScene: Escape does nothing.** `Update` only calls `Menu.Update()`; unlike `MainMenuScene`/`GameSelectionMenuScene` it never checks `Input.InputPressed(Abstractions.Input.Back)`. Blind user pressing Escape in settings gets no response. Add Back handling calling `OnBackSelected`. (`Scenes/SettingsMenuScene.cs:151`)
- [x] **CycleVolumeLevel jumps on out-of-list volume.** Volume loaded from settings file that isn't in `VolumeLevels` (e.g. 0.5, or 0.0 after clamp) falls back to index 0, so next press jumps to 0.4. Snap to nearest level instead. (`Scenes/SettingsMenuScene.cs:107`)
- [x] **No mute option.** `VolumeLevels` starts at 0.2; user cannot set volume to 0. Confirmed intended — no change. (`Scenes/SettingsMenuScene.cs:12`)
- [x] **Duplicate `Tts.Stop()` in `CycleTtsVolume`.** Called at line 98 and again at 103. Remove one. (`Scenes/SettingsMenuScene.cs`)
- [x] **Settings saved three times.** Every cycle press saves (`CycleMasterVolume`/`CycleTtsVolume`), then `OnBackSelected` calls `PersistSettings`, then scene change triggers `OnExit` → `PersistSettings` again. `OnExit` save alone suffices; remove redundant saves. (`Scenes/SettingsMenuScene.cs:60,78,146`)
- [x] **`Menu.Items` indexed by enum value.** `RenderSettingsValues` uses `Items[(int)SettingsMenuItem.MasterVolume]` — works only because enum order matches item order, and enum contains unused `ResetDefaults` so `Back = 3` while list has 3 items; indexing `Back` would throw. Look up items by id or hold direct references. (`Scenes/SettingsMenuScene.cs:71-72`)

## Design / Architecture

- [x] **GameSelectionMenuScene ignores `GameRegistry`.** "RiverRun" entry hardcoded; registry exists but only used for a console count in `Program`. Build menu items from `GameRegistry.Games` so new games appear without editing the scene. (`Scenes/GameSelectionMenuScene.cs:29`, `Program.cs:20`)
- [x] **`IGame` registration in wrong extension.** `AddSoundArcade` registers `services.AddSingleton<IGame, RiverRunGame>()`; per CLAUDE.md the game module's own `AddRiverRun` should register its game. Move it. (`DependencyInjection/ServiceCollectionExtensions.cs:28`)
- [x] **`AddRiverRun` registrations appear dead.** `RiverRunSettings`, `RiverRunSession`, `PlayerController`, `Game` singletons are never resolved — `SceneFactory.CreateRunScene` news up `RiverRunScene` with PAL deps only. Also singleton lifetime for a gameplay session would carry stale state across plays if it were used. Remove registrations or wire the scene to use them (with per-run lifetime). (`DependencyInjection/ServiceCollectionExtensions.cs:44-51`, `SceneFactory.cs:94`)
- [x] **`SceneFactory` constructed manually inside `ArcadeShell.Run`.** Composition belongs in DI. Cause is `LoadSettings` replacing the `AppSettings` instance — factory must be built after load. Load into a DI-registered `AppSettings` (copy values instead of replacing the instance) and register `ISceneFactory` as a service. (`ArcadeShell.cs:70`, `ArcadeShell.cs:102`)
- [x] **`AppSettings` created in constructor then discarded.** `new AppSettings()` at construction is thrown away by `LoadSettings`. Dead allocation; related to item above. (`ArcadeShell.cs:59`)
- [x] **`AppSettings` mutated via public setters.** `AppSettings.MasterVolume = ...` from shell and settings scene violates the DDD standard (state changes through explicit methods). Consider `AppSettings.SetMasterVolume(...)` with clamping inside the model — removes the shell's manual `Math.Clamp` too. (`ArcadeShell.cs:103-104`, `Scenes/SettingsMenuScene.cs:86,97`)

## Standards Violations

- [x] **`shouldExit` used without `this.`.** Standard: `this.` for all instance members. (`ArcadeShell.cs:81,97`)
- [x] **Missing XML doc comments.** Standard requires docs on all classes/methods/properties. Missing on: `MainMenuScene` (whole class), `GameSelectionMenuScene` (whole class), `SettingsMenuScene` (whole class), `ServiceCollectionExtensions.AddRiverRun`, `SceneFactory` properties, `Program`.
- [x] **Hardcoded strings throughout.** Menu titles ("Sound Arcade", "Select a Game", "Settings Menu"), item labels ("Play", "Settings", "Exit", "RiverRun", "Back", "Game Volume", "Text to Speech Volume"), TTS phrases (`$"{percent} game volume"`), and `Program`'s console message. Standard: never hardcode string values — move to a constants class (RunConstants pattern). (`Scenes/*.cs`, `Program.cs:20`)
- [x] **Suppression with placeholder justification.** `SuppressMessage(..., Justification = "<Pending>")` on `Main`. Either justify properly or change `Main()` to take no args. (`Program.cs:9`)
- [x] **Duplicated wrap-index helper.** `SettingsMenuScene.WrapArrayIndex` duplicates `Menu.WrapIndex`. Extract shared utility in Domain. (`Scenes/SettingsMenuScene.cs:124`, `Domain/Models/Menu.cs:77`)
- [x] **Duplicated Back-handling in menu scenes.** `MainMenuScene.Update` and `GameSelectionMenuScene.Update` are identical (Menu.Update + Back check); a shared menu-scene base or Back support inside `Menu` would remove the copy — and would have prevented the missing-Escape bug in SettingsMenuScene. (`Scenes/MainMenuScene.cs:48`, `Scenes/GameSelectionMenuScene.cs:47`)

## Consistency / Cleanliness

- [x] **`SettingsMenuScene` not `sealed`.** Other scenes sealed. (`Scenes/SettingsMenuScene.cs:10`)
- [x] **Needlessly public members.** `MainMenuScene.Input`, `GameSelectionMenuScene.Input`, `SettingsMenuScene.Tts`/`Renderer`, `ArcadeShell.Theme` — none used externally; make private. Nested enums inconsistent too: `MainMenuItem`/`GameSelectionMenuItem` public, `SettingsMenuItem` private — make all private.
- [x] **Unused enum member `SettingsMenuItem.ResetDefaults`.** No menu item for it; shifts `Back` to 3. Remove or implement reset-to-defaults. (`Scenes/SettingsMenuScene.cs:166`)
- [x] **Injected dependencies as mutable properties.** `ArcadeShell` and scenes hold deps in `{ get; set; }` / private-set auto-properties never reassigned (except `AppSettings`). Use get-only properties. (`ArcadeShell.cs:21-29`)
- [x] **`RenderSettingsValues` takes instance state as parameters.** (Resolved by refactor: method removed; value rendering moved to `ControlItem.Render`.) `renderer`, `appSettings`, `settingsValueColor` all available as members; drop the parameters. `SettingsValueColor` also never reassigned — make get-only, or inline (single use). (`Scenes/SettingsMenuScene.cs:66,58`)
- [x] **Volume percent formatting duplicated.** `(int)MathF.Round(x * 100.0f)` appears four times in SettingsMenuScene. Extract helper. (`Scenes/SettingsMenuScene.cs:68-69,90,102`)
