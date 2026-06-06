### RiverRun
**One-liner:** An audio-first endless runner where the player survives as long as possible by switching lanes and collecting points while obstacles rush toward them in a three-lane world.
**Core mechanic:** The player auto-runs forward through left, center, and right lanes, using keyboard input to dodge obstacles and collect score items while difficulty steadily increases.
**Audio design:** Spatial audio communicates lane position and distance for obstacles and collectibles, while TTS announces score, pause state, game over, and other significant events. Lane changes, pickups, and collisions each use distinct sound effects so the player can always understand the current game state without visuals.
**Controls:** Left and right arrows or A/D to change lanes, and Escape or P to pause.
**Win/fail condition:** The run continues until the player collides with an obstacle. Score increases by surviving longer and collecting pickups, with the goal of achieving the highest possible score.
**Accessibility check:** Passes all four accessibility tests. The game is playable with audio alone, fully controllable by keyboard, critical state is always audible through spatial audio or TTS, and no mechanic depends on visual-only information.
**Complexity estimate:** Medium
