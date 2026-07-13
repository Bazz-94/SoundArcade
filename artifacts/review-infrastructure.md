# Infrastructure Code Review

Scope: `Source/Infrastructure` — all PAL implementations (`RaylibAudio`, `SpatialAudioMath`, `WaveSynthesizer`, `RaylibInput`, `TextToSpeech`, `RaylibRenderer`, `FileSettingsStore`, `RaylibWindow`).

## Robustness

- [x] 1. **Corrupt JSON crashes startup**: `RaylibInput.cs:121` and `FileSettingsStore.cs:39` - `JsonSerializer.Deserialize` throws `JsonException` on malformed file; both load in startup path, so hand-edited or corrupt settings file crashes app.
  A. Wrap deserialize in try/catch and fall back to defaults (delete or ignore bad file).
  B. Validate with `JsonDocument.TryParse`-style guard before deserialize (more code, same effect).
  Chosen: A.

- [x] 2. **TrySetMapping accepts undefined keys**: `RaylibInput.cs:90` - `Enum.TryParse` accepts numeric strings ("999" becomes undefined `KeyboardKey`) and `"Null"`. Mapping then silently never fires.
  A. Add `Enum.IsDefined(parsedKey)` check and reject `KeyboardKey.Null`.

- [x] 3. **Audio device ownership on Dispose**: `RaylibAudio.cs:94` - constructor only inits device if not already ready, but `Dispose` always closes it. Second `RaylibAudio` instance (or another device user) gets its device closed by the first one disposed.
  A. Track `ownsDevice` flag set true only when this instance called `InitAudioDevice`; close only if owned.
  B. Accept: app has single instance via DI. Document assumption in class doc.
  Chosen: A (`ownsAudioDevice` flag).

- [x] 4. **Dispose skips lock**: `RaylibAudio.cs:70-92` - `Dispose` iterates/clears dictionaries without `lock (this.sync)` while every other access locks. TTS events fire on background thread, so race is real during shutdown.
  A. Take `lock (this.sync)` around the unload/clear block (after detaching TTS handlers).

## Standards compliance (`artifacts/standards.md`)

- [x] 5. **Missing `this.` on instance members**: `RaylibInput.cs` (`settingsPath:39`, `mappings:74,97,104,108,115...`), `TextToSpeech.cs` (`synth:27,36,48,57,65,72`), `RaylibRenderer.cs` (`camera:26,67,108-116`), `FileSettingsStore.cs` (`settingsPath:25,33,38,53,67`). Standard: always `this.` for instance members.
  A. Add `this.` everywhere.

- [x] 6. **Hardcoded magic value**: `RaylibRenderer.cs:61` - point radius `0.07f` inline. Standard: define constants for meaningful values.
  A. `private const float PointRadius = 0.07f;`

- [x] 7. **Single-use locals**: `RaylibInput.cs:65-66` (`key`), `TextToSpeech.cs:71` (`volumePercent`), `FileSettingsStore.cs:66` (`json`), `RaylibInput.cs:164` (`json`). Standard: inline single-use locals.
  A. Inline expressions.

- [x] 8. **Unused constant**: `RaylibAudio.cs:15` - `DefaultMasterVolume` never referenced.
  A. Delete.
  B. Apply it in constructor via `SetMasterVolume(DefaultMasterVolume)` if intent was explicit default.
  Chosen: A.

## Design / Consistency

- [x] 9. **Duplicated settings-path logic**: `RaylibInput.cs:15-16,37-39` duplicates `FileSettingsStore.cs:13-14,23-25` (same `SettingsDirectoryName`, same AppData resolution).
  A. Extract shared static helper (e.g. `SettingsPaths.GetPath(fileName)`) in Infrastructure.
  B. Leave: only two call sites, low churn.
  Chosen: A (`SettingsPaths.GetPath`).

- [x] 10. **Query with side effect**: `RaylibInput.cs:49-60` - `InputPressed` (a poll/query) raises `Pressed` event. Callers polling for other reasons unknowingly broadcast events; double-poll same frame fires event twice.
  A. Move event raising to explicit `Update()`/pump method that scans all inputs once per frame.
  B. Leave and document the poll-raises-event contract on `IInput`.
  Chosen: B — no subscribers exist anywhere, so the pump refactor buys nothing today; contract now documented on `IInput.Pressed` and `InputPressed`.

- [x] 11. **Mode switch per primitive**: `RaylibRenderer.cs:123-128` - every draw call wraps its own `BeginMode3D`/`EndMode3D`. Works, but N state switches per frame for N primitives.
  A. Add `BeginScene()`/`EndScene()` batching to renderer (interface change).
  B. Leave: primitive counts tiny, no measured cost. (Recommended)
  Chosen: B — no code change.

- [x] 12. **Shared base sound for non-positional play**: `RaylibAudio.cs:225-235` - `PlaySound` mutates pitch/volume on the single base `Sound` and replays it; overlapping plays of same id restart the sound and inherit last-set pitch/volume, unlike `PlaySoundAt` which rotates alias voices.
  A. Route `PlaySound` through same alias voice pool (pan 0).
  B. Leave: current games never overlap non-positional plays of same id.
  Chosen: A — `PlaySound` rotates same alias pool, pan reset to Raylib center (0.0, range -1..1); unused `GetSound`/`TryGetSound`/`GetGain` helpers removed.

- [x] 13. **Doc style mix**: `RaylibAudio.cs:102-108,173-178`, `TextToSpeech.cs:42-66` - interface members sometimes full XML docs, sometimes `<inheritdoc />`.
  A. Use `<inheritdoc />` on all interface implementations; keep full docs only on non-interface members.
