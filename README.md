# VRClassroom

A walk-in virtual classroom for the Meta Quest, built with Unity as a student hobby project. Step into the room, draw on the whiteboard, pick colours, pick up objects and flip through the map on the wall.

## Features

- **Whiteboard drawing:** point the right controller at the board and hold the trigger to draw. Strokes are smoothed between frames.
- **Colour palette:** six colour cubes and an eraser to change the pen.
- **Far grab:** pick up objects from a distance with the controller ray. Only objects with the `VRGrabbableObject` script can be grabbed.
- **Wall map:** point at the map and pull the trigger to cycle through images.

## Controls

| Input | Action |
|-------|--------|
| Right controller ray | Aim at the board, palette, map or an object |
| Right trigger (held) | Draw, or hold a grabbed object |
| Right trigger (press) | Pick a colour, or switch the map image |

## Requirements

- Unity **2022.3.62f3** (Built-in render pipeline)
- Meta XR SDK (`com.meta.xr.sdk.all` 201.0.0), installed through the Package Manager
- Meta Quest headset (Android build target, minimum API 32)

## Getting started

1. Clone the repo and open the folder with Unity Hub (Unity 2022.3.62f3). The first import takes a few minutes because Unity regenerates the `Library/` folder.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Switch the build target to Android and connect your Quest.
4. Build and run.

## Project layout

```
Assets/
  Scenes/SampleScene.unity   the classroom scene
  Classroom/                 room model and textures
  ClassObjects/, BookCase/   props
  ControllerRayFollower.cs   right-hand ray and input hub
  WhiteboardDrawer.cs        drawing on the board texture
  ColorPickerPanel.cs        colour cubes and eraser
  ColorPanelSetup.cs         builds the colour panel
  VRGrabbableObject.cs       marks an object as grabbable
  MapImageCycler.cs          wall map image switcher
  Editor/FixMaterials.cs     Tools > Fix Everything (batch-assigns textures)
```

## Credits

3D models and textures in `Assets/` come from third-party creators and belong to their original authors. The scripts are original work. Build outputs (`.apk`, `.app`) are not included in this repository.
