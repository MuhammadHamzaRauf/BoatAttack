# PROJECT FLOW - Boat Attack

## GAME STARTUP FLOW

### 1. Application Initialization
- **AppSettings.cs** - Main manager class that initializes first
  - Sets up resolution, framerate, and render settings
  - Creates console canvas and main camera reference
  - Handles scene loading and camera cleanup
  - Uses **GameplayIngredients** plugin for manager system

### 2. Main Menu Scene (`main_menu.unity`)
- **MainMenuHelper.cs** - Handles menu interactions
  - Boat selection (hull, colors)
  - Level selection and lap count
  - Game type setup (Singleplayer/Spectator)
  - Calls `RaceManager.SetupSingleplayerGame()` or `SetupSpectatorGame()`

### 3. Race Configuration
- **RaceManager.cs** - Central game state manager
  - Stores race configuration (boats, level, laps, game type)
  - Manages race start/end states
  - Handles boat spawning and race progression
  - Calls `RaceManager.LoadGame()` to start gameplay

## GAMEPLAY FLOW

### 4. Level Loading
- **AppSettings.LoadScene()** - Loads the selected level scene
- **RaceManager** spawns boats based on configuration
- **Boat.cs** - Main boat controller
  - Sets up player/AI controllers
  - Handles boat physics and race stats
  - Manages camera and UI references

### 5. Boat Control Systems
- **HumanController.cs** - Player input handling
  - Reads keyboard/gamepad input
  - Controls boat movement via Engine component
- **AiController.cs** - AI boat behavior
  - Follows waypoints using NavMesh
  - Handles obstacle avoidance
- **Engine.cs** - Physics and movement
  - Applies forces for acceleration and steering
  - Handles water physics and boat dynamics

### 6. Race Progression
- **WaypointGroup.cs** - Defines race track checkpoints
- **RaceManager** tracks lap times and positions
- **RaceUI.cs** - Displays race information
  - Speed, lap count, position
  - Race completion handling

## KEY PLUGINS & THEIR ROLES

### **GameplayIngredients** (`net.peeweek.gameplay-ingredients`)
- Provides manager system for AppSettings
- Handles automatic initialization and lifecycle
- Enables singleton pattern across scenes

### **Unity Input System** (`com.unity.inputsystem`)
- Handles all input (keyboard, gamepad, touch)
- Provides InputActions for boat controls
- Enables cross-platform input handling

### **Cinemachine** (`com.unity.cinemachine`)
- Camera system for boat following
- Handles camera switching and transitions
- Provides smooth camera movement and FOV changes

### **Universal Render Pipeline** (`com.unity.render-pipelines.universal`)
- Modern rendering pipeline
- Handles water shaders and boat materials
- Provides post-processing and lighting

### **Addressables** (`com.unity.addressables`)
- Asset loading system
- Handles boat prefabs, UI, and level assets
- Enables dynamic content loading

### **Water System** (`com.verasl.water-system`)
- Custom water rendering and physics
- Handles boat wake effects
- Provides realistic water interaction

## SCENE STRUCTURE

### **Main Menu Scene**
- UI elements for game setup
- Boat preview and customization
- Level and game type selection

### **Level Scenes** (e.g., `demo_Island.unity`)
- Terrain and water setup
- Waypoint system for race tracks
- Boat spawn points
- Camera setup and lighting

## SCRIPT HIERARCHY

### **Core Systems**
- `AppSettings` - Application manager
- `RaceManager` - Game state controller
- `Boat` - Individual boat controller

### **Input & Control**
- `HumanController` - Player input
- `AiController` - AI behavior
- `Engine` - Physics and movement

### **UI & Feedback**
- `MainMenuHelper` - Menu management
- `RaceUI` - In-game HUD
- `SimpleMobileControls` - Mobile input UI

### **Utilities**
- `Utility` - Global helper methods
- `WaypointGroup` - Track definition
- `CameraManager` - Camera switching

## GAME LOOP

1. **Menu Selection** → Configure boats, levels, game type
2. **Scene Loading** → Load level with configured settings
3. **Boat Spawning** → Create boats with appropriate controllers
4. **Race Start** → Begin lap timing and waypoint tracking
5. **Gameplay** → Player/AI control boats through track
6. **Race End** → Display results and return to menu

## MOBILE & AI FEATURES

### **Mobile Controls**
- `SimpleMobileControls` - Button-based touch input
- `MobileBoatInput` - Touch input adapter
- `MobileControlsManager` - Platform detection and UI management

### **AI Follow Mode**
- `AIFollowInput` - Makes boats follow player
- `AIFollowDemo` - Utility for testing AI behavior
- Smooth following with configurable parameters

## PERFORMANCE & OPTIMIZATION

- **Dynamic Resolution** - Adjusts render scale based on framerate
- **LOD System** - Level of detail for distant objects
- **Burst Compilation** - High-performance math operations
- **Multithreaded Rendering** - Parallel processing for graphics



