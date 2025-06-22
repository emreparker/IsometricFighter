# Isometric Fighter 🥊

A 3D isometric fighting game built in Unity where players can move, jump, and engage in combat with AI enemies.

## 🎮 Game Overview

Isometric Fighter is a prototype fighting game featuring:
- **Isometric 3D perspective** for strategic gameplay
- **Physics-based movement** with responsive controls
- **Combat system** with punching mechanics and recoil effects
- **Two enemy types**: Dummy enemies (stationary) and AI enemies (chasing)
- **Health system** with visual feedback and UI
- **Smooth camera following** for dynamic gameplay

## 🎯 Features

### Player Mechanics
- **Movement**: WASD keys for isometric movement
- **Jumping**: Spacebar for vertical movement
- **Combat**: V key for punching with cooldown system
- **Health**: Player health system with damage and death mechanics
- **Recoil**: Knockback effects when taking damage

### Enemy System
- **Dummy Enemy**: Stationary target that takes damage and respawns
- **AI Enemy**: Chases player, attacks, and applies damage
- **Health Management**: Both enemy types have health systems
- **Respawn System**: Enemies respawn after death

### UI & Feedback
- **Health Bars**: Visual health display for player and enemies
- **Punch Effects**: Visual feedback when landing hits
- **Death/Respawn Notifications**: Clear status updates
- **Controls Display**: On-screen control instructions

## 🛠️ Technical Details

### Unity Version
- **Unity 2023.2.x** (3D Core template)
- **Universal Render Pipeline (URP)**

### Key Scripts
- `PlayerController.cs` - Player movement, jumping, and combat
- `PlayerHealth.cs` - Player health and damage system
- `EnemyAI.cs` - AI enemy behavior and chasing logic
- `DummyEnemy.cs` - Stationary enemy behavior
- `EnemyHealth.cs` - Enemy health and damage system
- `CameraFollow.cs` - Smooth isometric camera following
- `GameUI.cs` - UI management and visual feedback

### Physics System
- **Rigidbody-based movement** for realistic physics
- **Custom gravity** for better jump control
- **Collision detection** for combat interactions
- **Force-based recoil** system for dramatic effects

## 🎮 Controls

| Action | Key |
|--------|-----|
| Move | WASD |
| Jump | Spacebar |
| Punch | V |
| Camera | Automatic following |

## 🚀 Getting Started

### Prerequisites
- Unity 2023.2.x or later
- Basic knowledge of Unity Editor

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/emreparker/IsometricFighter.git
   ```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click "Open" → "Add" → Select the `IsometricFighter` folder
   - Open the project

3. Load the main scene:
   - Navigate to `Assets/Scenes/MainScene.unity`
   - Click "Play" to start the game

### Project Structure
```
IsometricFighter/
├── Assets/
│   ├── Scripts/
│   │   ├── Player/          # Player-related scripts
│   │   ├── Enemy/           # Enemy AI and behavior
│   │   ├── Camera/          # Camera control
│   │   └── UI/              # User interface
│   ├── Scenes/              # Game scenes
│   ├── Materials/           # Visual materials
│   └── Prefabs/             # Reusable game objects
├── ProjectSettings/         # Unity project settings
└── README.md               # This file
```

## 🎨 Development Notes

### Code Architecture
- **Modular design** with separate scripts for different functionalities
- **Beginner-friendly** code with detailed comments
- **Unity best practices** followed throughout
- **Physics-based** movement for realistic feel

### Performance Considerations
- **Optimized physics** with reasonable mass and drag values
- **Efficient collision detection** using OverlapSphere
- **Minimal Update() calls** to maintain performance

### Future Enhancements
- [ ] Additional enemy types
- [ ] Power-ups and special abilities
- [ ] Multiple levels/environments
- [ ] Sound effects and music
- [ ] Particle effects for combat
- [ ] Multiplayer support

## 🤝 Contributing

This is a learning project for Unity development. Feel free to:
- Fork the repository
- Create feature branches
- Submit pull requests
- Report issues or bugs

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👨‍💻 Author

**Emre** - Unity Developer

---

*Built with ❤️ using Unity and C#*
