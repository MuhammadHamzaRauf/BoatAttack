# Boat Attack - Optimization & Porting Guide

## Project Overview

Boat Attack is a Unity URP demo showcasing high-quality water rendering, boat physics, and environmental effects. This guide focuses on **optimizing and porting the main menu and level_Island scenes** to your other game project.

## Target Scenes

### 1. Main Menu Scene (`Assets/scenes/main_menu.unity`)
- **Purpose**: Entry point with boat selection, level configuration, and game setup
- **Key Features**: UI system, boat customization, level selection, lighting setup
- **Size**: ~50KB scene file + lighting data

### 2. Level Island Scene (`Assets/scenes/_levels/level_Island.unity`)
- **Purpose**: Main gameplay environment with island, water, and race track
- **Key Features**: Terrain, water system, waypoints, lighting, reflections
- **Size**: ~11KB scene file + extensive lighting data (~10MB+)

## Core Systems Architecture

### Game Management
```
AppSettings (Manager) → RaceManager → WaypointGroup → Boat System
```

#### AppSettings.cs
- **Role**: Global application manager and settings
- **Key Features**:
  - Resolution and framerate control
  - Dynamic render scaling
  - Loading screen management
  - Camera cleanup
- **Porting Notes**: Core manager system, easily adaptable

#### RaceManager.cs
- **Role**: Race coordination and game state management
- **Key Features**:
  - Game type management (Singleplayer, Multiplayer, Spectator)
  - Boat spawning and configuration
  - Race timing and completion
  - Scene loading coordination
- **Porting Notes**: Central game logic, modify for your game type

#### WaypointGroup.cs
- **Role**: Race track definition and waypoint management
- **Key Features**:
  - Track waypoints with width and rotation
  - Checkpoint system
  - Starting position calculation
  - Track distance measurement
- **Porting Notes**: Adapt for your level design system

### Boat System
```
Boat.cs → Engine.cs → HumanController/AiController
```

#### Boat.cs
- **Role**: Main boat controller and race logic
- **Key Features**:
  - Controller switching (Human/AI)
  - Race statistics tracking
  - Camera integration (Cinemachine)
  - Livery customization
- **Porting Notes**: Core vehicle system, highly reusable

#### Engine.cs
- **Role**: Physics-based boat movement
- **Key Features**:
  - Water buoyancy integration
  - Force-based propulsion
  - Steering torque system
  - Audio pitch control
- **Porting Notes**: Physics core, adapt for your vehicle type

### Environment System
```
DayNightController → SkyboxSystem → CloudManager → VegetationSystem
```

#### DayNightController.cs
- **Role**: Dynamic lighting and time-of-day system
- **Key Features**:
  - Sun position calculation
  - Skybox color gradients
  - Fog color transitions
  - Reflection probe updates
- **Porting Notes**: Excellent lighting system, highly portable

#### SkyboxSystem.cs
- **Role**: 3D skybox rendering
- **Key Features**:
  - Dynamic skybox scaling
  - Camera-based rendering
  - Performance optimization
- **Porting Notes**: Advanced skybox solution

## Technical Dependencies

### Unity Packages
- **URP 14.0.11**: Universal Render Pipeline
- **Cinemachine 2.10.1**: Camera system
- **Addressables 1.22.2**: Asset management
- **Input System 1.7.0**: Modern input handling
- **Mathematics 1.2.6**: Math utilities
- **Burst 1.8.17**: Performance optimization

### Custom Systems
- **Water System**: Local package with buoyancy and wave simulation
- **Gameplay Ingredients**: Manager framework (net.peeweek.gameplay-ingredients)

## Scene Optimization Strategy

### Main Menu Scene
#### Current State
- **Lighting**: Baked lightmaps with reflection probes
- **UI**: TextMeshPro-based interface
- **Assets**: Boat previews, lighting data

#### Optimization Targets
1. **Reduce Lightmap Resolution**: From 7 to 2-3
2. **Simplify Reflection Probes**: Reduce from 3 to 1-2
3. **UI Optimization**: Consolidate UI elements
4. **Asset Streaming**: Use Addressables for boat previews

#### Porting Steps
1. Copy `MainMenuHelper.cs` and UI prefabs
2. Adapt `RaceManager` for your game type
3. Modify boat selection system
4. Update level selection logic

### Level Island Scene
#### Current State
- **Lighting**: High-resolution lightmaps (3.9MB)
- **Reflections**: 10 reflection probes
- **Occlusion**: 5.7MB occlusion data
- **Terrain**: Complex island geometry

#### Optimization Targets
1. **Lightmap Resolution**: From 2.5 to 1-1.5
2. **Reflection Probes**: Reduce from 10 to 4-6
3. **Occlusion Culling**: Simplify or remove
4. **Terrain LOD**: Implement level-of-detail system

#### Porting Steps
1. Copy `WaypointGroup.cs` and waypoint system
2. Adapt terrain and water for your environment
3. Modify lighting setup for your needs
4. Update waypoint logic for your gameplay

## Performance Optimization

### Rendering
- **URP Settings**: Optimize for target platform
- **Lightmap Resolution**: Balance quality vs. memory
- **Reflection Probes**: Strategic placement, reduce count
- **Occlusion Culling**: Essential for complex scenes

### Memory Management
- **Addressables**: Stream assets as needed
- **Texture Compression**: Use appropriate formats
- **Model LODs**: Implement for complex geometry
- **Audio**: Compress and stream large files

### Code Optimization
- **Burst Compilation**: Enable for math-heavy operations
- **Job System**: Use for water physics
- **Object Pooling**: Implement for particles and effects
- **Caching**: Cache frequently accessed components

## Porting Checklist

### Phase 1: Core Systems
- [ ] Copy `AppSettings.cs` and adapt for your project
- [ ] Copy `RaceManager.cs` and modify game logic
- [ ] Copy `WaypointGroup.cs` and adapt waypoint system
- [ ] Copy `Boat.cs` and `Engine.cs` for vehicle system

### Phase 2: Environment
- [ ] Copy `DayNightController.cs` for lighting system
- [ ] Copy `SkyboxSystem.cs` for 3D skybox
- [ ] Adapt terrain and water systems
- [ ] Modify lighting and reflection setup

### Phase 3: UI and Menus
- [ ] Copy `MainMenuHelper.cs` and UI prefabs
- [ ] Adapt boat selection system
- [ ] Modify level selection logic
- [ ] Update UI styling for your game

### Phase 4: Optimization
- [ ] Reduce lightmap resolutions
- [ ] Optimize reflection probe placement
- [ ] Implement LOD systems
- [ ] Profile and optimize bottlenecks

## File Structure for Porting

```
YourProject/
├── Scripts/
│   ├── Core/
│   │   ├── AppSettings.cs          # Adapted from BoatAttack
│   │   ├── GameManager.cs          # Your game logic
│   │   └── LevelManager.cs         # Your level system
│   ├── Vehicle/
│   │   ├── Boat.cs                 # Adapted from BoatAttack
│   │   ├── Engine.cs               # Adapted from BoatAttack
│   │   └── WaypointSystem.cs       # Adapted from BoatAttack
│   ├── Environment/
│   │   ├── DayNightController.cs   # From BoatAttack
│   │   ├── SkyboxSystem.cs         # From BoatAttack
│   │   └── WaterSystem.cs          # Your water system
│   └── UI/
│       ├── MainMenuHelper.cs       # Adapted from BoatAttack
│       └── UIManager.cs            # Your UI system
├── Scenes/
│   ├── MainMenu.unity              # Adapted from main_menu
│   └── YourLevel.unity             # Adapted from level_Island
└── Assets/
    ├── Materials/                   # Optimized materials
    ├── Textures/                    # Compressed textures
    └── Prefabs/                    # Optimized prefabs
```

## Performance Targets

### Main Menu Scene
- **Target FPS**: 60+ (UI responsiveness)
- **Memory**: <100MB total
- **Load Time**: <3 seconds
- **Lighting**: Simple, efficient setup

### Level Scene
- **Target FPS**: 30-60 (depending on platform)
- **Memory**: <500MB total
- **Load Time**: <10 seconds
- **Lighting**: Optimized for performance

## Platform Considerations

### Mobile (Android/iOS)
- **Lightmap Resolution**: 1-1.5
- **Reflection Probes**: 2-4 maximum
- **Texture Compression**: ASTC or ETC2
- **LOD System**: Essential for performance

### PC/Console
- **Lightmap Resolution**: 2-3
- **Reflection Probes**: 4-8
- **Texture Quality**: High with compression
- **Advanced Features**: Enable water effects, particles

### WebGL
- **Lightmap Resolution**: 1 maximum
- **Reflection Probes**: 1-2
- **Texture Compression**: Maximum compression
- **Feature Reduction**: Disable complex effects

## Troubleshooting

### Common Issues
1. **Lighting Artifacts**: Reduce lightmap resolution
2. **Memory Spikes**: Check texture compression and LODs
3. **Performance Drops**: Profile and optimize bottlenecks
4. **Build Errors**: Verify package dependencies

### Optimization Tools
- **Unity Profiler**: Identify performance bottlenecks
- **Frame Debugger**: Analyze rendering pipeline
- **Memory Profiler**: Track memory usage
- **Build Report**: Analyze build size and assets

## Next Steps

1. **Analyze Current Performance**: Profile both scenes
2. **Identify Bottlenecks**: Focus on lighting and reflections
3. **Plan Optimization**: Set target performance metrics
4. **Begin Porting**: Start with core systems
5. **Iterate and Test**: Optimize based on performance data

## Support

For technical questions about the BoatAttack systems:
- Check the original README.md for project overview
- Review script comments for implementation details
- Test optimizations incrementally to avoid breaking changes

---

**Note**: This guide focuses on the main menu and level_Island scenes as requested. The demo_Island scene contains additional racing logic that may not be needed for your port.

