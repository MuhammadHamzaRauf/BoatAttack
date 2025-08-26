# Boat Attack - Unity URP Demo

## Project Overview

Boat Attack is a Unity URP demo showcasing high-quality water rendering, boat physics, and environmental effects. This project demonstrates advanced water simulation, dynamic lighting, and smooth boat controls across multiple platforms.

## Key Features

### 🚤 **Boat Physics & Controls**
- **Realistic Water Physics**: Buoyancy, wave interaction, and engine submersion
- **Smooth Controls**: Responsive steering and throttle with force-based movement
- **Multiple Input Methods**: Keyboard, gamepad, and touch controls
- **Boat Customization**: Hull selection, color schemes, and livery options

### 🌊 **Water System**
- **Gerstner Waves**: Realistic wave simulation with jobs system
- **Dynamic Water Level**: Engine depth detection for realistic physics
- **Water Materials**: High-quality shaders with reflection and refraction
- **Performance Optimized**: Efficient wave calculations using Burst compilation

### 🌅 **Environment & Lighting**
- **Dynamic Day/Night**: Real-time lighting transitions with skybox changes
- **3D Skybox**: Immersive environment with cloud systems
- **Reflection Probes**: Dynamic reflections for water and materials
- **Fog System**: Atmospheric depth with time-of-day variations

### 🏁 **Racing & AI**
- **Waypoint System**: Configurable race tracks with checkpoints
- **AI Controllers**: Smart waypoint following with stuck detection
- **Race Management**: Lap timing, position tracking, and race completion
- **AI Follow Mode**: Boats can follow the player with smooth behavior

### 🧭 **Chase Mode (Additive)**
- Add-on AI behavior for non-player boats to pursue the player while maintaining spacing.
- Components:
  - `AIChaseController` (per boat): minimal brain that drives the existing `Engine` with steer/throttle.
  - `AIChaseManager` (optional): scene helper to toggle chase across multiple boats.
- Public API (`AIChaseController`):
  - `StartChasing(Transform player)`
  - `StopChasingAndReturnToBase()`
  - `Deactivate()`
- Tunables (serialized): `player`, `chaseMinDistance`, `desiredSpeed`, `steerGain`, `throttleGain`, `throttleSlewRate`, `steerSlewRate`, `returnTolerance`, `returnSpeed`.
- Events: `onInsideMinDistance`, `onArrivedAtBase`.
- Gizmos: base/home point and min-distance ring.

### 📱 **Mobile Support**
- **Touch Controls**: On-screen joystick and buttons for mobile devices
- **Auto-Detection**: Automatically enables on Android/iOS builds
- **Editor Testing**: Toggle to test mobile controls in Unity Editor
- **Responsive UI**: Canvas-based controls that adapt to screen size

## Quick Start

### 1. **Open the Project**
- Open Unity 2022.3 LTS or later
- Open the BoatAttack project folder
- Wait for package imports to complete

### 2. **Run the Demo**
- Open `Assets/scenes/main_menu.unity`
- Press Play to enter the main menu
- Select your boat and level preferences
- Click "Start Race" to begin gameplay

### 3. **Basic Controls**
- **WASD/Arrows**: Move the boat
- **Mouse**: Look around (if enabled)
- **R**: Reset boat position
- **F**: Pause/unpause game
- **Space**: Toggle time of day (debug)

## Mobile Controls

### **How to Enable**
1. **Automatic**: Mobile controls appear automatically on Android/iOS builds
2. **Editor Testing**: Set `forceEnable = true` in `MobileControlsManager`
3. **Manual Setup**: Add `MobileControls.prefab` to your scene

### **Control Layout**
- **Left Button**: Turn boat left
- **Right Button**: Turn boat right  
- **Forward Button**: Accelerate forward
- **Reverse Button**: Move backward
- **Simple Setup**: Easy to attach to any UI buttons

### **Touch Sensitivity**
- Adjust `throttleSensitivity` and `steerSensitivity` in `MobileBoatInput`
- Modify `brakeThreshold` for brake activation sensitivity
- Customize joystick `maxRadius` for different screen sizes

### **Testing in Editor**
```csharp
// Add SimpleMobileControls component to any GameObject
// Assign your UI buttons in the Inspector
// Use SimpleMobileControlsExample for automatic setup
```

## AI Follow Mode

### **How to Use**
1. **Add AI Follow**: Use `AIFollowDemo` utility script
2. **Manual Setup**: Add `AIFollowInput` component to any boat
3. **Configure Behavior**: Adjust parameters in the Inspector

### **AI Behavior**
- **Target Following**: Smooth pursuit of player boat
- **Distance Management**: Maintains optimal follow distance
- **Speed Control**: Accelerates/brakes to match target speed
- **Look-Ahead**: Predicts target movement for better tracking

### **Configuration Parameters**
```csharp
[Header("Target Settings")]
desiredSpeed = 12f;        // Target speed in m/s
minFollowDist = 10f;       // Minimum distance to maintain
maxFollowDist = 25f;       // Maximum distance before catching up

[Header("Control Tuning")]
steerGain = 1.0f;         // Steering responsiveness
throttleGain = 1.0f;      // Acceleration responsiveness
lookAheadTime = 0.75f;    // Prediction time for smooth following
```

### **Visual Debugging**
- **Yellow Sphere**: Minimum follow distance
- **Red Sphere**: Maximum follow distance  
- **Cyan Sphere**: Look-ahead position
- **Green Line**: Direction to target

### **Quick Setup Example**
```csharp
// Add AIFollowDemo to any GameObject in your scene
// Assign the AI boat and player boat in the Inspector
// The script will automatically set up AI follow mode
```

## Chase Mode Usage

### Enable on an AI Boat
1. Add `AIChaseController` to any AI boat GameObject (keep existing AI untouched for other modes).
2. Optionally assign `basePoint`. If left empty, current position at Start is used as home/base.
3. From code, call one of the public methods:
```csharp
// Begin chasing the player transform
aiChaseController.StartChasing(playerTransform);

// Stop chasing and navigate back to base, then idle
aiChaseController.StopChasingAndReturnToBase();

// Fully deactivate/idle (no forces applied)
aiChaseController.Deactivate();
```

### RaceManager Integration
The chase mode is fully integrated into the existing race system:

1. **New Game Type**: `RaceManager.GameType.Chase` - automatically sets up chase mode
2. **Public API Methods**:
   ```csharp
   // Start chase mode for all AI boats
   RaceManager.StartChaseMode();
   
   // Stop chase and return to base
   RaceManager.StopChaseMode();
   
   // Deactivate all AI boats
   RaceManager.DeactivateChaseMode();
   
   // Get count of boats currently chasing
   int chaseCount = RaceManager.GetChaseModeBoatCount();
   ```

3. **Automatic Setup**: When using `GameType.Chase`, AI boats automatically get `AIChaseController` components and start chasing the player.

### ChaseModeController (Optional)
Add `ChaseModeController` to any GameObject for easy scene-level control:

1. **UI Integration**: Assign buttons for Start/Stop/Deactivate chase mode
2. **Status Display**: Shows count of boats currently chasing
3. **Static Access**: Use `ChaseModeController.StartChase()` from anywhere in code
4. **Context Menu**: Right-click component for quick testing

### Tuning Fields (Inspector)
- `player` (Transform): Optional default target. Can be assigned at runtime via API.
- `desiredSpeed` (float): Target forward speed while chasing.
- `chaseMinDistance` (float): Standoff distance; inside this the boat slows/holds.
- `steerGain`, `throttleGain` (floats): Responsiveness for steering and throttle.
- `steerSlewRate`, `throttleSlewRate` (floats): Optional smoothing to reduce oscillation.
- `returnTolerance` (float): Distance to home considered "arrived".
- `returnSpeed` (float): Forward speed while returning to base.

### Optional Scene-Level Control
- Add `AIChaseManager` and it will automatically find boats and player from the scene
- **Automatic Discovery**: Finds all boats with `AiController` and player boats with `HumanController`
- **No Manual Assignment**: Automatically adds `AIChaseController` components to AI boats
- Use `enableOnStart` to automatically start chase when the scene begins
- **Simple Setup**: Just add the component to any GameObject in your scene

Acceptance: Existing race and free-roam modes are unaffected. Chase can be toggled per boat or globally via the manager.



## Mobile Build Notes

### **Build Settings**
- **Scripting Backend**: IL2CPP (recommended for mobile)
- **Target Architecture**: ARM64 for modern devices
- **Graphics API**: OpenGL ES 3.0 or Metal (iOS)

### **Performance Optimization**
- **Multithreaded Rendering**: Enable for better performance
- **VSync**: Disable for maximum frame rate
- **Frame Rate**: Use device-appropriate caps (30/60/120 FPS)
- **Quality Settings**: Adjust based on target device performance

### **Input System Setup**
- **Active Input Handling**: Set to "Both" in Player Settings
- **Input System Package**: Version 1.7.0 or later
- **Touch Input**: Automatically configured for mobile platforms

### **Memory Management**
- **Texture Compression**: Use ASTC (Android) or PVRTC (iOS)
- **Asset Streaming**: Leverage Addressables for large assets
- **LOD Systems**: Implement for complex geometry

## Architecture Overview

### **Core Systems**
```
AppSettings (Manager) → RaceManager → WaypointGroup → Boat System
                    ↓
                Input System → IBoatInput → Engine Physics
```

### **Input Abstraction**
- **IBoatInput Interface**: Common contract for all input sources
- **DesktopBoatInput**: Preserves existing keyboard/gamepad behavior
- **MobileBoatInput**: Touch-based input for mobile devices
- **AIFollowInput**: AI behavior that implements the same interface

### **Component Structure**
- **Boat.cs**: Main controller and race logic
- **Engine.cs**: Physics-based movement and water interaction
- **HumanController**: Player input handling
- **AiController**: Waypoint following AI
- **AIFollowInput**: Player-following AI behavior

## Troubleshooting

### **Common Issues**

#### Mobile Controls Not Appearing
- Check `forceEnable` in `MobileControlsManager`
- Verify `MobileControls.prefab` is in the scene
- Ensure Input System package is installed

#### AI Follow Not Working
- Verify target boat has "Player" tag
- Check `AIFollowInput` component is added
- Ensure target transform is assigned

#### Performance Issues
- Reduce lightmap resolution for mobile
- Enable LOD systems for complex models
- Profile with Unity Profiler to identify bottlenecks

#### Build Errors
- Verify all required packages are installed
- Check Input System package version compatibility
- Ensure target platform settings are correct

### **Debug Tools**
- **AIFollowDemo**: Visual setup and testing for AI follow
- **MobileInputTest**: Monitor mobile input values
- **Scene Gizmos**: Visual debugging for AI behavior
- **Console Logs**: Detailed information about system state

## Development Notes

### **Adding New Input Sources**
1. Implement `IBoatInput` interface
2. Create input adapter component
3. Add to boat controller system
4. Test with existing physics

### **Extending AI Behavior**
1. Inherit from `AIFollowInput` or create new `IBoatInput` implementation
2. Implement required interface methods
3. Add configuration parameters
4. Include visual debugging if needed

### **Mobile UI Customization**
1. Modify `MobileControls.prefab` layout
2. Adjust control element positions and sizes
3. Update `MobileControlsManager` references
4. Test on different screen resolutions

## Performance Targets

### **Desktop (1080p)**
- **Target FPS**: 60+
- **Memory**: <2GB
- **Load Time**: <10 seconds

### **Mobile (High-end)**
- **Target FPS**: 30-60
- **Memory**: <1GB
- **Load Time**: <15 seconds

### **Mobile (Mid-range)**
- **Target FPS**: 30
- **Memory**: <500MB
- **Load Time**: <20 seconds

## Future Enhancements

### **Planned Features**
- **Multiplayer Support**: Networked boat racing
- **Advanced AI**: More sophisticated racing AI
- **Weather Effects**: Dynamic weather and water conditions
- **VR Support**: Virtual reality boat racing experience

### **Mobile Improvements**
- **Gesture Controls**: Swipe and pinch gestures
- **Haptic Feedback**: Device vibration for immersion
- **Adaptive Quality**: Automatic quality adjustment based on device
- **Cloud Saves**: Progress synchronization across devices

---

**Note**: This project demonstrates Unity URP best practices for water rendering, physics simulation, and cross-platform input handling. All new features are additive and preserve existing functionality.
