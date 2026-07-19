# Plan for Implementing Story: sa-01-riverrun-scoreboard

Story: `artifacts/stories/sa-01-riverrun-scoreboard.md`

## Tasks
1. text-input-pal
  - Description: Add `ReadTypedCharacters()` to `IInput` and `Backspace` to the `Input` enum with default mapping. Implement in `RaylibInput` (Raylib `GetCharPressed`), update `MockInput`.
  - Acceptance Criteria: Typed characters drain once per frame; Backspace pollable like other inputs; mock supports queuing characters.
  - Status: Completed
2. scoreboard-contract
  - Description: Add `ScoreEntry(Name, Score, AchievedAt)` record and `IScoreboardStore` (`Load(gameId)`, `Save(gameId, entries)`) to `Abstractions`.
  - Acceptance Criteria: Contract uses primitives/records only; XML docs on all members.
  - Status: Completed
3. scoreboard-model
  - Description: `Scoreboard` model in `Domain`: add entry, keep top 10 highest first, ties broken by earliest `AchievedAt`. Unit tests.
  - Acceptance Criteria: Tests cover insert, overflow trim, tie-break, ordering.
  - Status: Completed
4. file-scoreboard-store
  - Description: `FileScoreboardStore` in Infrastructure: JSON per game id via `SettingsPaths`, corrupt/missing file loads empty.
  - Acceptance Criteria: Round-trips entries; malformed JSON returns empty list without throwing.
  - Status: Completed
5. name-entry-component
  - Description: Name entry UI component in `Domain`: letters/digits/space only, 12-char cap, TTS echo per character, Backspace deletes with echo, Enter confirms trimmed non-empty name, Escape cancels. Unit tests with mock input/TTS.
  - Acceptance Criteria: Tests cover filtering, cap, backspace, empty-name confirm rejected, cancel.
  - Status: Completed
6. start-menu-overlay
  - Description: RiverRun start menu overlay (Play, Scoreboard, Back). Scene enters showing menu; Play starts run; Back returns to game selection.
  - Acceptance Criteria: Run no longer auto-starts on scene enter; menu announced via TTS.
  - Status: Completed
7. end-game-menu-overlay
  - Description: End-game menu overlay on `SessionState.GameOver`: Try again, Submit score, Main Menu. Try again restarts; menu announced after game-over TTS.
  - Acceptance Criteria: Options in required order; Try again starts new run; Main Menu leaves scene.
  - Status: Completed
8. submit-score-flow
  - Description: Submit score opens name entry overlay; confirmed name saves score through `Scoreboard` + store; TTS "Score saved"; cancel saves nothing; one submit per run.
  - Acceptance Criteria: Score persisted only with non-empty name; second submit attempt in same run not offered.
  - Status: Completed
9. scoreboard-view-overlay
  - Description: Scoreboard overlay: navigable menu of entries ("Rank 1, Name, Score") plus Back; empty scoreboard announced.
  - Acceptance Criteria: Arrow navigation announces each entry; Back/Escape returns to start menu.
  - Status: Completed
10. wiring
  - Description: Register `FileScoreboardStore` in DI; pass store into `RiverRunScene` via `SceneFactory`.
  - Acceptance Criteria: Solution builds; full flow works end to end; all tests pass.
  - Status: Completed

## Excludes
- Rank announcement after submitting (plain "Score saved" only).
- Key remapping UI for Backspace.
- `IGame` scene-provider refactor (selection still launches `SceneType.Run`).
- Scoreboards for other games (store is ready for them).
