using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace BoatAttack.Editor
{
    /// <summary>
    /// Automatically fixes EventSystem components to use the new Input System
    /// </summary>
    [InitializeOnLoad]
    public static class AutoFixEventSystem
    {
        static AutoFixEventSystem()
        {
            // Run after the domain reload
            EditorApplication.delayCall += CheckAndFixEventSystems;
        }

        private static void CheckAndFixEventSystems()
        {
            // Only run once when the project is opened
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var eventSystems = Object.FindObjectsOfType<EventSystem>();
            int fixedCount = 0;
            
            foreach (var eventSystem in eventSystems)
            {
                if (FixEventSystem(eventSystem))
                {
                    fixedCount++;
                }
            }
            
            if (fixedCount > 0)
            {
                Debug.Log($"[AutoFixEventSystem] Fixed {fixedCount} EventSystem(s) to use new Input System");
            }
        }

        private static bool FixEventSystem(EventSystem eventSystem)
        {
            var standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneModule == null)
            {
                return false; // Already fixed or no input module
            }

            // Remove the old StandaloneInputModule
            Object.DestroyImmediate(standaloneModule);
            
            // Add the new InputSystemUIInputModule
            var newModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            
            // Configure the new module with default settings
            // Note: InputSystemUIInputModule uses default settings automatically
            // No additional configuration needed for basic functionality
            
            return true;
        }
    }
}
