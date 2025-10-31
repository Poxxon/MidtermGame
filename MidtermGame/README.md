# Mini 3D Explorer (GAM Midterm)

Small interactive 3D scene built with **C# + OpenTK**.

## Gameplay Overview

Explore a small 3D environment featuring a textured wooden floor and simple geometric shapes. Use the keyboard and mouse to move around and look in all directions. Toggle the point light on and off to see how lighting affects the scene. The environment provides a smooth and immersive experience with collision detection to prevent walking through objects.

## Controls

- `W/A/S/D`: Move forward/left/backward/right  
- `Mouse`: Look around  
- `Space` / `Ctrl`: Move up / down (To keep off from flying, collision on top and bottom has been added)
- `E`: Toggle light on/off  
- `Esc`: Grab/release cursor  

## Features Implemented

- Multiple meshes (plane, cube, pyramid)  
- FPS camera (mouse + keyboard)  
- Phong lighting (ambient/diffuse/specular)  
- One point light (toggle with `E`)  
- Textured environment with wooden floor texture  
- Clean classes: `Shader`, `Texture`, `Mesh`, `Camera`  
- Collision detection to prevent walking through objects (bonus)  
- Optional animations on some meshes (bonus)  

## Bonus Features

- Collision detection system for realistic movement constraints  
- Simple animations applied to selected meshes for added visual interest  

## Build & Run

- Requirements: .NET 8 (or .NET 6), OpenTK 4.x  
- NuGet packages: `OpenTK`, `StbImageSharp`  
- Run the project:  
  ```bash
  dotnet run
  ```
