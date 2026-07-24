# Digital Fish Swarm Gesture Interactive System

Design and Implementation of a Digital Fish Swarm Gesture Interactive System Based on Unity3D

## Project Overview

This project is an immersive digital art interactive work that combines digital media art, Unity real-time rendering, swarm intelligence algorithms, and AI visual interaction.

### Core Features

- **Boids Fish Swarm Simulation** — Implement four major swarm behaviors: cohesion, separation, alignment, and escape
- **Real-time Gesture Interaction** — Hand detection and tracking based on MediaPipe
- **Visual Effects System** — Shader glow effects, particle system trails
- **Performance Optimization** — Support 100+ fish, ≥30 FPS

## Project Structure

```
Digital-Fish-Swarm-Interactive-System/
├── README_CN.md                    # Chinese documentation
├── README_EN.md                    # English documentation
├── Assets/
│   ├── Scripts/
│   │   ├── Boids/                 # Boids algorithm core
│   │   ├── Fish/                  # Fish models and animations
│   │   ├── Interaction/           # Gesture interaction module
│   │   └── Manager/               # System managers
│   ├── Materials/                 # Shader materials
│   ├── Prefabs/                   # Prefabs
│   ├── Scenes/                    # Scenes
│   ├── Shaders/                   # Shader scripts
│   └── Resources/                 # Configuration resources
├─��� Docs/                          # Documentation
└── .gitignore                     # Git ignore file
```

## Development Progress

- [ ] Basic directory structure
- [ ] Boids algorithm implementation
- [ ] Fish models and prefabs
- [ ] Gesture recognition integration
- [ ] Visual effects
- [ ] Performance optimization
- [ ] Testing and deployment

## Tech Stack

- **Engine**: Unity 2022.3 LTS
- **Language**: C#
- **Gesture Recognition**: MediaPipe
- **Rendering**: Shader Graph
- **Effects**: Particle System, VFX Graph

## Setup Guide

See `Docs/SETUP.md` for detailed setup instructions.

## License

MIT
