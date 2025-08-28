# MANUAL_WIRING.md - Chase Mode Setup Guide

This guide explains how to manually wire up the Chase Mode system for your Unity URP boat project. **DO NOT** save scenes or prefabs during this process - follow the steps exactly as described.

## Overview

The Chase Mode system consists of three main components:
1. **BoatAIModeRouter** - Lives on each AI boat, prevents AI overlap
2. **AIChaseController** - Handles chase behavior (already exists)
3. **ChaseModeManager** - Optional scene-level control

## Step 1: Setup Each AI Boat Prefab

### 1.1 Add BoatAIModeRouter Component
For each AI boat prefab that should support Chase Mode:

1. **Select the AI boat prefab** in the Project window
2. **Add Component** → Search for "BoatAIModeRouter"
3. **Verify the component appears** in the Inspector

### 1.2 Assign AI Controller References
In the BoatAIModeRouter component:

1. **Legacy AI field**: Drag the existing `AiController` component from the same GameObject
2. **Chase AI field**: Drag the existing `AIChaseController` component (or add one if missing)
3. **Base Home field**: Leave empty to auto-use spawn position, OR drag a Transform to use as base

### 1.3 Verify Default State
- **Current Mode** should show "Legacy" (this preserves existing behavior)
- **Use Spawn Position As Base** should be checked (recommended)

### 1.4 Add AIChaseController (if missing)
If the boat doesn't have an AIChaseController:

1. **Add Component** → Search for "AIChaseController"
2. **Configure chase parameters**:
   - `Desired Speed`: 12 (adjust as needed)
   - `Chase Min Distance`: 12 (standoff distance)
   - `Steer Gain`: 1.0 (steering responsiveness)
   - `Throttle Gain`: 1.0 (throttle responsiveness)
   - `Return Tolerance`: 3.0 (arrival distance)

## Step 2: Scene-Level Setup (Optional)

### 2.1 Add ChaseModeManager
To enable global control over all AI boats:

1. **Create an empty GameObject** in your scene (name it "ChaseModeManager")
2. **Add Component** → Search for "ChaseModeManager"
3. **Configure settings**:
   - `Auto Find Player`: Checked (recommended)
   - `Setup Buttons On Start`: Checked (if you have UI buttons)
   - `Enable On Start`: Unchecked (start manually)

### 2.2 UI Button Setup (Optional)
If you want UI controls:

1. **Create UI buttons** for Start Chase, Stop Chase, Deactivate, Enable Legacy
2. **Assign buttons** to the corresponding fields in ChaseModeManager
3. **Add a Text component** for status display (assign to Status Text field)

### 2.3 Player Reference
- **Player Transform**: Leave blank for auto-detection, OR drag your player boat
- The manager will automatically find boats with `HumanController` components

## Step 3: Play-Mode Testing

### 3.1 Baseline Verification
1. **Enter Play Mode**
2. **Verify AI boats behave normally** (Legacy mode active)
3. **Check Console** for "BoatAIModeRouter: Mode changed from Idle to Legacy" messages

### 3.2 Chase Mode Testing
1. **Use ChaseModeManager** (if added):
   - Click "Start Chase All" button
   - Verify boats start chasing the player
   - Check Console for chase start messages
2. **Or use RaceManager API**:
   - Call `RaceManager.StartChaseMode()` from code
   - Verify boats switch to chase mode

### 3.3 Return to Base Testing
1. **Click "Stop Chase All"** or call `RaceManager.StopChaseMode()`
2. **Verify boats navigate back to spawn/base position**
3. **Check that boats automatically return to Legacy mode** when arriving

### 3.4 Deactivation Testing
1. **Click "Deactivate All"** or call `RaceManager.DeactivateChaseMode()`
2. **Verify boats stop moving** (both controllers disabled)
3. **Check that boats idle** without any AI control

## Step 4: Per-Boat Control (Advanced)

### 4.1 Individual Boat Control
You can control individual boats via their routers:

```csharp
// Get the router component
var router = boat.GetComponent<BoatAIModeRouter>();

// Start chasing a specific player
router.EnableChase(playerTransform);

// Stop and return to base
router.StopChaseAndReturnToBase();

// Deactivate this boat
router.Deactivate();

// Return to legacy racing
router.EnableLegacy();
```

### 4.2 Event Handling
Subscribe to router events for custom behavior:

```csharp
var router = boat.GetComponent<BoatAIModeRouter>();

// Mode change events
router.onModeChanged.AddListener((mode) => {
    Debug.Log($"Boat {boat.name} switched to {mode} mode");
});

// Arrival events
router.onArrivedAtBase.AddListener(() => {
    Debug.Log($"Boat {boat.name} arrived at base");
});
```

## Step 5: Configuration Tuning

### 5.1 Chase Behavior Tuning
In AIChaseController component:

- **Desired Speed**: Target forward speed while chasing
- **Chase Min Distance**: Standoff distance (boat slows when closer)
- **Steer/Throttle Gain**: Responsiveness (higher = more responsive)
- **Slew Rates**: Smoothing (higher = smoother, less oscillation)
- **Return Tolerance**: Distance to base considered "arrived"

### 5.2 Router Behavior Tuning
In BoatAIModeRouter component:

- **Show Debug Info**: Enable for detailed logging
- **Use Spawn Position As Base**: Auto-create base reference
- **Base Home**: Manual base position assignment

## Troubleshooting

### Common Issues

1. **"No Engine component found"**
   - Ensure the boat has an Engine component
   - BoatAIModeRouter requires Engine to function

2. **"Multiple AI controllers active"**
   - This should not happen with the router
   - Check that both AiController and AIChaseController exist
   - Verify router is properly assigned

3. **Boats not chasing**
   - Check that AIChaseController is added
   - Verify player transform is assigned
   - Check Console for error messages

4. **Boats overlapping/conflicting**
   - Ensure BoatAIModeRouter is on each AI boat
   - Verify only one controller is enabled at a time
   - Check router's Current Mode display

### Debug Information

- **Console Logs**: Enable "Show Debug Info" in router components
- **Gizmos**: Select boats to see mode indicators and base positions
- **Inspector**: Monitor Current Mode and controller states

## Performance Notes

- **Zero allocations**: No per-frame memory allocation in Update/FixedUpdate
- **Efficient switching**: Controller enable/disable is lightweight
- **Event-driven**: Uses UnityEvents for minimal overhead
- **Validation**: Runtime checks ensure system integrity

## Integration with Existing Systems

- **RaceManager**: Fully integrated with existing chase mode
- **Boat Setup**: Works with existing boat spawning system
- **Input System**: No changes to human player controls
- **Physics**: No modifications to boat physics or water system

## Next Steps

After completing this setup:

1. **Test thoroughly** in your specific scenes
2. **Adjust parameters** for your game's feel
3. **Add UI controls** if desired
4. **Integrate with game logic** using the provided API

## Support

If you encounter issues:
1. Check the Console for error messages
2. Verify all components are properly assigned
3. Ensure boats have required components (Engine, AiController)
4. Test with minimal setup first, then add complexity

---

**Remember**: Do not save scenes or prefabs during this process. The system is designed to work with your existing setup without requiring permanent changes.
