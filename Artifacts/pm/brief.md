# Sound Arcade — Project Manager Brief

## Overview
Sound Arcade is an accessible, audio-first arcade game collection built in C# using Raylib-cs. It is designed to be fully playable by blind players through spatial audio and keyboard controls alone, while remaining enjoyable for sighted players.

## Goals
- Deliver a collection of self-contained mini-games, each playable without any visual feedback
- Build on a clean, portable architecture that allows the infrastructure layer to be swapped out without affecting game logic.
- Use synthesised speech and 3D positional audio as the primary player feedback channel

## Technical Approach
The project uses Clean Architecture with Lightweight DDD. A Platform Abstraction Layer (PAL) decouples all game logic from Raylib, ensuring the codebase can target new platforms without rewriting the game. Each mini-game is an independent, registered module.

## Constraints
- No gameplay mechanic may depend on visuals
- Each mini-game must ship with its own audio design and spec

## Delivery Strategy
Ship one mini-game at a time. The arcade shell and core architecture are built first, then games are added incrementally.

## Games
1. **RiverRun**: A subway surfer-style endless runner where players run on 3 lanes, avoiding obstacles and collecting coins. See RiverRun.brief.md for details.