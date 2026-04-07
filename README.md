# Escape from Shady Pines Assisted Living (Unity Prototype)

This repository contains a **small, playable retro FPS prototype** design for Unity 2022+.

## Included

- Modular C# scripts for core systems:
  - `PlayerController`
  - `WeaponSystem`
  - `EnemyAI`
  - `HUDManager`
  - `GameManager`
- Interaction system for keycard + exit objective
- Three weapons (Walker, Pill Bottle, FBP 9000)
- Enemy archetypes supported by one AI script, now including **Patient** enemies
- DOOM-style controls (keyboard + mouse first)
- HUD state mapping for Earl portrait states
- Optional intro screen text:
  - Title: `ESCAPE FROM SHADY PINES`
  - Subtitle: `there's only one way folks leave shady pines… body bags.`

## Unity Setup (Quick)

1. Create/open a Unity 2022+ 3D (Built-in or URP) project.
2. Copy this repository files into the project root.
3. Create a scene `Assets/Scenes/ShadyPines_EastWing.unity`.
4. Build a simple level blockout:
   - hallways + branching rooms
   - patient rooms with loot pickups/upgrades
   - nurses station
   - locked exit door
5. Add:
   - Player object with `CharacterController`, Camera, `PlayerController`, `WeaponSystem`, `PlayerHealth`
   - Canvas HUD with `HUDManager`
   - Empty object with `GameManager`
   - Optional intro scene object with `IntroScreenController`
6. Assign references in inspector according to script headers.

## Controls

- `W/S`: forward/back
- `A/D`: strafe
- `Mouse`: look
- `Left Click`: fire
- `Right Click`: secondary action (optional)
- `Space`: interact (doors, keycards, room loot pickups)
- `Left Shift`: sprint (stamina-limited)
- `R`: reload (for ranged if desired)
- `1/2/3`: switch weapons

## Room Loot / Upgrade Suggestions

Use `RoomLootPickup` in patient rooms to reward exploration:
- Extra pill ammo
- Extra bed pan ammo
- Walker damage upgrades
- Pill Bottle damage upgrades
- FBP 9000 fire-rate upgrade
- Small healing pickups

## Patient Enemy Voice Setup

For Patient enemies, assign `patientVoiceLines` in `EnemyAI` using clips such as:
- "Take me! Take me with you!"
- "I fought in Vietnam!!"

## Audio

- Use `BackgroundMusicPlayer` with a looping hold-music style track.
- Use `AmbientAnnouncer` for distorted PA stingers.
- Keep footsteps/combat SFX as placeholders for rapid iteration.

## Notes

- Pixel-art portrait textures can be 32x32 with magenta transparency key.
- Audio uses placeholders through `AudioSource` fields.
- Movement tuning intentionally arcade-like/floaty for retro feel.

