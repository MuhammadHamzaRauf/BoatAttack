# Input System Migration Guide

## Problem Description

The error you encountered:
```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings.
UnityEngine.Input.get_mousePosition () (at <be81f3ae31184ce39450911afbb8439b>:0)
UnityEngine.EventSystems.BaseInput.get_mousePosition () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/InputModules/BaseInput.cs:75)
UnityEngine.EventSystems.StandaloneInputModule.UpdateModule () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/InputModules/StandaloneInputModule.cs:180)
UnityEngine.EventSystems.EventSystem.TickModules () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:481)
UnityEngine.EventSystems.EventSystem.Update () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:496)
```

This error occurs because:
1. Your project is configured to use the new Input System package (`activeInputHandler: 1` in ProjectSettings)
2. Some scripts are still using the old `UnityEngine.Input` class
3. The EventSystem is still using the old `StandaloneInputModule` instead of the new `InputSystemUIInputModule`

## What I've Fixed

### 1. Updated Scripts to Use New Input System

#### CameraManager.cs
- Replaced `Input.GetKeyDown()` calls with new Input System actions
- Added proper input binding for camera controls
- Integrated with existing `InputControls` asset

#### RaceManager.cs
- Removed old `Input.touchSupported` usage
- Simplified platform detection logic

### 2. Enhanced InputControls Asset
- Added new actions for camera controls:
  - `CameraToggle` (Space key)
  - `NextCamera` (Right arrow)
  - `PrevCamera` (Left arrow)
  - `ToggleUI` (H key + double tap)

### 3. Created Migration Tools
- `InputSystemMigrationHelper.cs` - Manual migration tool
- `AutoFixEventSystem.cs` - Automatic EventSystem fixer

## How to Complete the Fix

### Option 1: Automatic Fix (Recommended)
The `AutoFixEventSystem.cs` script will automatically run when you open the project and fix all EventSystem components.

### Option 2: Manual Fix
1. Open Unity Editor
2. Go to `Tools > Input System Migration Helper`
3. Click "Migrate All EventSystems to New Input System"

### Option 3: Manual Scene Fix
For each scene with an EventSystem:
1. Select the EventSystem GameObject
2. Remove the `StandaloneInputModule` component
3. Add the `InputSystemUIInputModule` component
4. Configure settings:
   - The `InputSystemUIInputModule` uses default settings automatically
   - No additional configuration needed for basic functionality

## Regenerate InputControls.cs

After updating the InputControls.inputactions file, Unity should automatically regenerate the `InputControls.cs` file. If it doesn't:

1. Select the `InputControls.inputactions` file in the Project window
2. In the Inspector, click "Generate C# Class"
3. Or restart Unity Editor

## Testing the Fix

1. Open any scene with an EventSystem
2. Verify the EventSystem has `InputSystemUIInputModule` instead of `StandaloneInputModule`
3. Test UI interactions (buttons, sliders, etc.)
4. Test camera controls (Space, arrow keys, H key)

## Additional Notes

- The new Input System provides better performance and more flexible input handling
- Touch input will work automatically with the new system
- All existing input bindings are preserved
- The migration is backward compatible

## Troubleshooting

If you still encounter issues:

1. Check that `activeInputHandler` is set to `1` in ProjectSettings
2. Ensure all EventSystem components use `InputSystemUIInputModule`
3. Verify the `InputControls.cs` file is up to date
4. Check the Console for any remaining old Input usage warnings

## Files Modified

- `Assets/Scripts/Camera/CameraManager.cs` - Updated to use new Input System
- `Assets/Scripts/GameSystem/RaceManager.cs` - Removed old Input usage
- `Assets/Data/InputControls.inputactions` - Added new camera control actions
- `Assets/Scripts/Editor/InputSystemMigrationHelper.cs` - Migration tool
- `Assets/Scripts/Editor/AutoFixEventSystem.cs` - Automatic fixer

The error should now be resolved, and your project will fully use the new Input System package.
