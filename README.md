# SHIFT

*A minimalist grid-based puzzle game about movement, positioning, and finding your way out.*

Shift between rows and columns, navigate around obstacles, and find a path to the exit. Think ahead, understand how the grid behaves, and work your way through each puzzle.

## Download

Want to give it a try?

### Windows

Download the ZIP file from the latest release.

Extract the ZIP to a folder.

Run Shift.exe to play.


### Android

Download the Android ZIP file from the latest release onto your phone.

Open your Files app and extract the ZIP.

Open the extracted folder and tap the .apk file to install the game.

If prompted, allow your browser or Files app to install unknown apps, then continue with the installation.

Once installed, open One More Wave and play!

Note: Android may show a security warning because the game isn't installed through the Google Play Store.

This is an early development build. Bugs and gameplay changes are expected!

## Gameplay

Shift is a minimalist puzzle game built around a simple concept: moving through a fixed grid using row and column shifts.

Rather than moving one tile at a time, the player shifts along entire rows and columns. Empty spaces move alongside the player, while walls, voids, and the exit remain fixed in place.

Plan your movements carefully, navigate around obstacles, and find a way to reach the exit.

<img width="800" height="1000" alt="ShiftGameplayresize" src="https://github.com/user-attachments/assets/cf5242c0-33cf-42e7-bbc6-25fc18bec88a" />

<p align="center">
  <i>Think ahead. Shift your position. Find your way out.</i>
</p>

## Features

- **Grid-based puzzles** — solve levels by shifting between rows and columns.
- **Unique movement mechanics** — move along entire rows and columns rather than one tile at a time.
- **Fixed environmental obstacles** — navigate around walls, voids, and the exit, which remain in their positions.
- **Distinct tile behaviours** — understand how different tile types affect movement.
- **Handcrafted levels** — work through carefully designed grid layouts that challenge your understanding of the mechanics.
- **Minimalist visuals** — clean geometric shapes and a simple colour palette keep the focus on gameplay.

## Tile Types

| Tile | Appearance | Behaviour |
|---|---|---|
| Empty | <img width="256" height="256" alt="Empty" src="https://github.com/user-attachments/assets/e0d4f352-7608-425e-ae12-ddbd2ffd28fb" /> | Moves with the player when shifting rows or columns. |
| Player | <img width="256" height="256" alt="Player" src="https://github.com/user-attachments/assets/c4dbade7-d45e-4273-8a8c-4d52a6070716" /> | The player moves along the selected row or column. |
| Wall | <img width="256" height="256" alt="Wall" src="https://github.com/user-attachments/assets/f4b04bee-6cc0-40f2-95d2-f9d62d3df85b" /> | Remains fixed and blocks movement when the player would move into or through it. |
| Void | <img width="256" height="256" alt="Void" src="https://github.com/user-attachments/assets/c771ca2c-5a5e-4090-be5c-73277df2e95e" /> | Remains fixed and creates opportunities to bypass obstacles and reach areas that would otherwise be inaccessible. |
| Exit | <img width="256" height="256" alt="Exit" src="https://github.com/user-attachments/assets/f3ccfa30-10e8-4911-b653-59e46abc0029" /> | Remains fixed. Reaching it completes the level. |


## Core Mechanics

**Row & Column Shifting**

The main mechanic revolves around shifting the player along entire rows and columns rather than moving one tile at a time.

- Shift horizontally along the player's current row.
- Shift vertically along the player's current column.
- Empty spaces move alongside the player.
- Environmental tiles remain fixed in their positions.
- Plan your shifts carefully to navigate around obstacles and reach the exit.

**Void Tiles**

Void tiles introduce another layer of strategy by allowing the player to pass through walls when they are aligned in the same row or column.

**Walls**

Walls act as solid, permanent obstacles. They remain in place and prevent the player from moving through them. A shift is blocked if the player would move into or through a wall.

**Exit**

The exit is the player's ultimate destination. It remains fixed throughout the level, and reaching it triggers the level completion sequence.

## Controls

| Input | Action |
|---|---|
| `WASD` / Arrow Keys | Shift horizontally or vertically |
| `LEFT CLICK` / Swipe | Shift horizontally or vertically |

## Built With

- **Unity** — game engine
- **C#** — gameplay systems and grid logic

## Development

Shift is an game project focused on creating an engaging puzzle experience through a small set of interconnected mechanics.

Development focuses on grid movement, tile interactions, level design, and creating satisfying visual feedback for player interactions.

Particular attention is given to keeping the mechanics understandable while allowing increasingly interesting puzzle layouts to emerge from their interactions.

## Source Code

Shift is developed using Unity and C#.

This repository contains the game's source code and Unity project files. It is publicly available for anyone interested in exploring the implementation of the grid system, tile interactions, movement logic, and puzzle mechanics.

If you're interested in the technical side of the project, feel free to explore the source code!

## Roadmap

- [ ] Expand the collection of handcrafted puzzle levels
- [ ] Refine grid movement and tile interactions
- [ ] Polish movement animations and visual feedback
- [ ] Improve level transitions and completion sequences
- [ ] Refine the overall user interface and menus
- [ ] Continue improving the overall puzzle experience

## Credits & Assets

**Animation Library** — PrimeTween by Kyrylo Kuzyk for its tweening and animation tools.

All third-party assets remain the property of their respective creators and are subject to their applicable licenses.

---

**SHIFT** — every move matters.
