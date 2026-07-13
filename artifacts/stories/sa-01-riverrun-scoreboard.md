# Story: RiverRun Scoreboard

## Dependencies
- None

## Description
Players can save their score with their name after losing a RiverRun run, and view the best scores later.
After the player loses, an end-game menu appears with the options: Try again, Submit score, and Main Menu.
Submitting a score asks for the player's name. Entering a name is optional — if no name is entered, the score is not saved.
The scoreboard keeps the top 10 scores and is viewable from RiverRun's menu.

## Acceptance Criteria
- When the player loses, an end-game menu appears with options in this order: Try again, Submit score, Main Menu.
- The end-game menu is fully playable by audio alone: opening it, focus changes, and selections are announced via TTS.
- Choosing Submit score opens a name entry screen where the player types their name; each typed character is echoed via TTS.
- Confirming name entry with a non-empty name saves the score to the scoreboard; cancelling or confirming with an empty name saves nothing.
- A score can be submitted at most once per run.
- The scoreboard persists across game restarts and keeps only the top 10 scores, highest first; ties are broken by earliest achieved.
- RiverRun opens with a start menu (Play, Scoreboard, Back); its Scoreboard option shows entries as navigable items announcing rank, name, and score via TTS; an empty scoreboard is announced as such.
- Duplicate player names are allowed.

## Notes
- All menus (start, pause, end-game, scoreboard, name entry) are overlays swapped within the RiverRun scene; no new scenes.
- Scoreboard persistence follows the same approach as existing settings storage; the store is generic, keyed by game id, one file per game.
- Top-10 rules (highest first, earliest wins ties) live in a shared, unit-tested domain model.
- Typing a name requires a new platform text-input capability; Enter confirms, Escape cancels, Backspace deletes.
- Names accept letters, digits, and spaces only; other characters are ignored. Trimmed on submit; all-spaces counts as empty.
- Visuals remain supplementary — the whole flow must work with audio alone.
- Names are limited to 12 characters.
- Submitting gives a plain "Score saved" confirmation; no rank announcement.

## Open Questions
- None.
