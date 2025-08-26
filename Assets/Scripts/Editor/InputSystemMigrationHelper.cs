using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace BoatAttack.Editor
{
    /// <summary>
    /// Helper script to migrate EventSystem components from old input system to new Input System
    /// </summary>
    public class InputSystemMigrationHelper : EditorWindow
    {
        [MenuItem("Tools/Input System Migration Helper")]
        public static void ShowWindow()
        {
            GetWindow<InputSystemMigrationHelper>("Input System Migration");
        }

        private void OnGUI()
        {
            GUILayout.Label("Input System Migration Helper", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This tool helps migrate EventSystem components from the old input system to the new Input System.");
            GUILayout.Space(10);
            
            if (GUILayout.Button("Migrate All EventSystems to New Input System"))
            {
                MigrateAllEventSystems();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Find EventSystems Using Old Input System"))
            {
                FindEventSystemsUsingOldInput();
            }
        }

        private void MigrateAllEventSystems()
        {
            var eventSystems = Object.FindObjectsOfType<EventSystem>();
            int migratedCount = 0;
            
            foreach (var eventSystem in eventSystems)
            {
                if (MigrateEventSystem(eventSystem))
                {
                    migratedCount++;
                }
            }
            
            EditorUtility.DisplayDialog("Migration Complete", 
                $"Successfully migrated {migratedCount} EventSystem(s) to the new Input System.", "OK");
        }

        private void FindEventSystemsUsingOldInput()
        {
            var eventSystems = Object.FindObjectsOfType<EventSystem>();
            var oldInputSystems = new System.Collections.Generic.List<EventSystem>();
            
            foreach (var eventSystem in eventSystems)
            {
                var standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (standaloneModule != null)
                {
                    oldInputSystems.Add(eventSystem);
                }
            }
            
            if (oldInputSystems.Count > 0)
            {
                string message = $"Found {oldInputSystems.Count} EventSystem(s) still using the old input system:\n\n";
                foreach (var es in oldInputSystems)
                {
                    message += $"- {es.name} in {es.gameObject.scene.name}\n";
                }
                message += "\nUse 'Migrate All EventSystems' to fix this.";
                
                EditorUtility.DisplayDialog("Old Input System Found", message, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No Issues Found", "All EventSystems are using the new Input System.", "OK");
            }
        }

        private bool MigrateEventSystem(EventSystem eventSystem)
        {
            var standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneModule == null)
            {
                return false; // Already migrated or no input module
            }

            // Remove the old StandaloneInputModule
            Object.DestroyImmediate(standaloneModule);
            
            // Add the new InputSystemUIInputModule
            var newModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            
            // Configure the new module with default settings
            // Note: InputSystemUIInputModule uses default settings automatically
            // No additional configuration needed for basic functionality
            
            Debug.Log($"Migrated EventSystem '{eventSystem.name}' to use new Input System");
            return true;
        }
    }
}
