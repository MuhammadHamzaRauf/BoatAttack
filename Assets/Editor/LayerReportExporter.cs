#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayerReportExporter : EditorWindow
{
    [Serializable]
    public class LayerEntry
    {
        public string context;            // "scene" | "prefab"
        public string scenePath;
        public string sceneName;
        public string prefabAssetPath;
        public string prefabGuid;
        public string objectPath;         // hierarchy path inside scene/prefab
        public string objectName;
        public int    layerIndex;
        public string layerName;
        public string tag;
    }

    [Serializable]
    public class ErrorEntry
    {
        public string prefabAssetPath;
        public string prefabGuid;
        public string error;
    }

    [Serializable]
    public class LayerReport
    {
        public string exportedAt;
        public string unityVersion;
        public string projectPath;
        public bool includeInactive;
        public bool includeScenesOpen;
        public bool includeAllPrefabs;

        public List<LayerEntry> items = new();
        public List<ErrorEntry> errors = new();
    }

    // UI state
    bool includeInactive = true;
    bool includeScenesOpen = false;   // off by default; you can toggle it in UI
    bool includeAllPrefabs = true;
    string exportFileName = "LayerReport.json";

    [MenuItem("Tools/Layers/Export Layers to JSON (Hardened)")]
    static void Open() => GetWindow<LayerReportExporter>("Layer Report");

    void OnGUI()
    {
        GUILayout.Label("Layer Report Exporter (Hardened)", EditorStyles.boldLabel);
        includeInactive   = EditorGUILayout.ToggleLeft("Include Inactive Scene Objects", includeInactive);
        includeScenesOpen = EditorGUILayout.ToggleLeft("Scan All Open Scenes", includeScenesOpen);
        includeAllPrefabs = EditorGUILayout.ToggleLeft("Scan All Prefab Assets", includeAllPrefabs);

        EditorGUILayout.Space(8);
        exportFileName = EditorGUILayout.TextField("Output File Name", exportFileName);

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Export JSON")) Export();
        if (GUILayout.Button("Apply JSON (Tags & Layers)")) ApplyFromJson();

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Exporter skips broken prefabs (missing nested/multi-root) and records them under 'errors' so the export completes. Importer assigns layers by NAME (creating them if needed) and tags as required.",
            MessageType.Info);
    }

    void Export()
    {
        try
        {
            var report = new LayerReport
            {
                exportedAt        = DateTime.UtcNow.ToString("o"),
                unityVersion      = Application.unityVersion,
                projectPath       = Directory.GetParent(Application.dataPath)?.FullName ?? "",
                includeInactive   = includeInactive,
                includeScenesOpen = includeScenesOpen,
                includeAllPrefabs = includeAllPrefabs,
            };

            if (includeScenesOpen)  ScanOpenScenes(report);
            if (includeAllPrefabs)  ScanAllPrefabs(report);

            string savePath = EditorUtility.SaveFilePanelInProject(
                "Save Layer JSON",
                string.IsNullOrEmpty(exportFileName) ? "LayerReport.json" : exportFileName,
                "json",
                "Choose where to save the layer report JSON."
            );

            if (!string.IsNullOrEmpty(savePath))
            {
                File.WriteAllText(savePath, JsonUtility.ToJson(report, true));
                AssetDatabase.Refresh();
                Debug.Log($"[LayerReportExporter] Exported {report.items.Count} entries, {report.errors.Count} errors → {savePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LayerReportExporter] Export failed: {ex}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    // ---------- SCAN OPEN SCENES ----------
    void ScanOpenScenes(LayerReport report)
    {
        int sceneCount = SceneManager.sceneCount;
        for (int si = 0; si < sceneCount; si++)
        {
            var scene = SceneManager.GetSceneAt(si);
            if (!scene.isLoaded) continue;

            string scenePath = scene.path;
            string sceneName = scene.name;

            var roots = scene.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++)
            {
                var root = roots[r];
                EditorUtility.DisplayProgressBar("Scanning Scenes", $"{sceneName} ({r + 1}/{roots.Length})", (float)r / Mathf.Max(1, roots.Length));

                TraverseHierarchy(
                    root.transform,
                    (t, objectPath) =>
                    {
                        if (!includeInactive && !t.gameObject.activeInHierarchy) return;
                        report.items.Add(MakeEntry("scene", scenePath, sceneName, null, null, t, objectPath));
                    });
            }
        }
        EditorUtility.ClearProgressBar();
    }

    // ---------- SCAN ALL PREFABS ----------
    void ScanAllPrefabs(LayerReport report)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            EditorUtility.DisplayProgressBar("Scanning Prefabs", $"{assetPath}  ({i + 1}/{guids.Length})", (float)i / Mathf.Max(1, guids.Length));

            GameObject root;
            Action cleanup;
            string error;

            bool ok = TryLoadPrefabForTraversal(assetPath, out root, out cleanup, out error);
            if (!ok || root == null)
            {
                report.errors.Add(new ErrorEntry { prefabAssetPath = assetPath, prefabGuid = guid, error = error ?? "Unknown error loading prefab." });
                continue;
            }

            try
            {
                var localRoot = root; // avoid capturing an out variable in the lambda
                TraverseHierarchy(localRoot.transform, (t, objectPath) =>
                {
                    report.items.Add(MakeEntry("prefab", null, null, assetPath, guid, t, objectPath));
                });
            }
            finally
            {
                cleanup?.Invoke();
            }
        }
        EditorUtility.ClearProgressBar();
    }

    // Try LoadPrefabContents; if it fails, fallback to instantiating the main asset in a preview scene.
    static bool TryLoadPrefabForTraversal(string assetPath, out GameObject root, out Action cleanup, out string error)
    {
        // Strategy 1: LoadPrefabContents (fastest, gives an isolated scene)
        try
        {
            root = PrefabUtility.LoadPrefabContents(assetPath);
            var capturedRoot = root; // capture into local to avoid lambda capturing 'out' param
            cleanup = () => PrefabUtility.UnloadPrefabContents(capturedRoot);
            error = null;
            if (root != null) return true;
        }
        catch (Exception ex)
        {
            // keep going with fallback
            error = $"LoadPrefabContents failed: {ex.Message}";
        }

        // Strategy 2: Load main asset and instantiate in a Preview Scene
        try
        {
            var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefabAsset == null)
            {
                root = null; cleanup = null;
                error = string.IsNullOrEmpty(error) ? "LoadAssetAtPath returned null." : error + " | Main asset null.";
                return false;
            }

            var preview = EditorSceneManager.NewPreviewScene();
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset, preview);
            if (instance == null)
            {
                root = null; cleanup = null;
                error = string.IsNullOrEmpty(error) ? "InstantiatePrefab returned null." : error + " | Instantiate null.";
                EditorSceneManager.ClosePreviewScene(preview);
                return false;
            }

            root = instance;
            cleanup = () => EditorSceneManager.ClosePreviewScene(preview);
            return true;
        }
        catch (Exception ex)
        {
            root = null; cleanup = null;
            error = (string.IsNullOrEmpty(error) ? "" : error + " | ") + $"Fallback instantiate failed: {ex.Message}";
            return false;
        }
    }

    // ---------- TRAVERSAL (iterative, no local function; avoids CS1628 capture issues) ----------
    static void TraverseHierarchy(Transform root, Action<Transform, string> visitor)
    {
        if (root == null || visitor == null) return;

        var tStack = new Stack<Transform>();
        var pStack = new Stack<string>();

        tStack.Push(root);
        pStack.Push(root.name);

        while (tStack.Count > 0)
        {
            var t = tStack.Pop();
            var path = pStack.Pop();
            visitor(t, path);

            for (int i = t.childCount - 1; i >= 0; i--)
            {
                var ch = t.GetChild(i);
                var childPath = string.IsNullOrEmpty(path) ? ch.name : path + "/" + ch.name;
                tStack.Push(ch);
                pStack.Push(childPath);
            }
        }
    }

    static LayerEntry MakeEntry(string context, string scenePath, string sceneName, string prefabAssetPath, string prefabGuid, Transform t, string objectPath)
    {
        int idx = t.gameObject.layer;
        string nm = LayerMask.LayerToName(idx);
        return new LayerEntry
        {
            context         = context,
            scenePath       = scenePath ?? "",
            sceneName       = sceneName ?? "",
            prefabAssetPath = prefabAssetPath ?? "",
            prefabGuid      = prefabGuid ?? "",
            objectPath      = objectPath,
            objectName      = t.name,
            layerIndex      = idx,
            layerName       = string.IsNullOrEmpty(nm) ? "" : nm,
            tag             = t.gameObject.tag
        };
    }

    // ---------- IMPORT / APPLY ----------
    void ApplyFromJson()
    {
        try
        {
            string path = EditorUtility.OpenFilePanel("Choose Layer JSON to Apply", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            var report = JsonUtility.FromJson<LayerReport>(json);
            if (report == null || report.items == null || report.items.Count == 0)
            {
                Debug.LogError("[LayerReportExporter] No items found in JSON. Abort.");
                return;
            }

            // Group by target asset/scene to minimize loads
            var prefabBuckets = new Dictionary<string, List<LayerEntry>>();
            var sceneBuckets  = new Dictionary<string, List<LayerEntry>>();

            for (int i = 0; i < report.items.Count; i++)
            {
                var item = report.items[i];
                if (item == null) continue;

                if (item.context == "prefab")
                {
                    string prefabPath = !string.IsNullOrEmpty(item.prefabAssetPath)
                        ? item.prefabAssetPath
                        : (!string.IsNullOrEmpty(item.prefabGuid) ? AssetDatabase.GUIDToAssetPath(item.prefabGuid) : null);

                    if (string.IsNullOrEmpty(prefabPath)) continue;

                    if (!prefabBuckets.TryGetValue(prefabPath, out var list))
                    {
                        list = new List<LayerEntry>();
                        prefabBuckets[prefabPath] = list;
                    }
                    list.Add(item);
                }
                else if (item.context == "scene")
                {
                    if (string.IsNullOrEmpty(item.scenePath)) continue;
                    if (!sceneBuckets.TryGetValue(item.scenePath, out var list))
                    {
                        list = new List<LayerEntry>();
                        sceneBuckets[item.scenePath] = list;
                    }
                    list.Add(item);
                }
            }

            // Apply to prefabs
            int prefabIdx = 0;
            foreach (var kv in prefabBuckets)
            {
                string assetPath = kv.Key;
                var entries = kv.Value;

                EditorUtility.DisplayProgressBar("Apply Prefab Layers/Tags", assetPath, (float)prefabIdx / Math.Max(1, prefabBuckets.Count));
                prefabIdx++;

                GameObject root;
                Action cleanup;
                string loadErr;
                if (!TryLoadPrefabForTraversal(assetPath, out root, out cleanup, out loadErr) || root == null)
                {
                    Debug.LogWarning($"[LayerReportExporter] Skip prefab '{assetPath}': {loadErr}");
                    continue;
                }

                bool changed = false;
                try
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        var e = entries[i];
                        var t = FindByObjectPath(root.transform, e.objectPath);
                        if (t == null)
                        {
                            Debug.LogWarning($"[LayerReportExporter] Path not found in prefab '{assetPath}': {e.objectPath}");
                            continue;
                        }
                        if (TrySetLayerAndTag(t.gameObject, e.layerName, e.tag))
                            changed = true;
                    }

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LayerReportExporter] Error applying to prefab '{assetPath}': {ex.Message}");
                }
                finally
                {
                    cleanup?.Invoke();
                }
            }

            // Apply to scenes
            int sceneIdx = 0;
            foreach (var kv in sceneBuckets)
            {
                string scenePath = kv.Key;
                var entries = kv.Value;

                EditorUtility.DisplayProgressBar("Apply Scene Layers/Tags", scenePath, (float)sceneIdx / Math.Max(1, sceneBuckets.Count));
                sceneIdx++;

                bool openedHere = false;
                Scene scene = default;
                try
                {
                    // Try to get loaded scene first
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        var s = SceneManager.GetSceneAt(i);
                        if (s.path == scenePath) { scene = s; break; }
                    }
                    if (!scene.IsValid())
                    {
                        scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                        openedHere = true;
                    }

                    bool changed = false;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        var e = entries[i];
                        var t = FindInSceneByObjectPath(scene, e.objectPath);
                        if (t == null)
                        {
                            Debug.LogWarning($"[LayerReportExporter] Path not found in scene '{scenePath}': {e.objectPath}");
                            continue;
                        }
                        if (TrySetLayerAndTag(t.gameObject, e.layerName, e.tag))
                            changed = true;
                    }

                    if (changed)
                    {
                        EditorSceneManager.MarkSceneDirty(scene);
                        EditorSceneManager.SaveScene(scene);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LayerReportExporter] Error applying to scene '{scenePath}': {ex.Message}");
                }
                finally
                {
                    if (openedHere && scene.IsValid())
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[LayerReportExporter] Apply JSON complete.");
        }
        catch (Exception ex)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"[LayerReportExporter] ApplyFromJson failed: {ex}");
        }
    }

    // ---------- PATH HELPERS ----------
    static Transform FindByObjectPath(Transform root, string objectPath)
    {
        if (root == null || string.IsNullOrEmpty(objectPath)) return null;
        // objectPath includes root name, e.g. "_BoatBase/BoatHull/Engine"
        string[] parts = objectPath.Split('/');
        int idx = 0;

        Transform current = root;
        if (!string.Equals(current.name, parts[0], StringComparison.Ordinal))
        {
            // attempt to find a direct child matching the first part
            current = FindChildByName(current, parts[0]);
            if (current == null) return null;
        }
        idx = 1;

        while (idx < parts.Length)
        {
            current = FindChildByName(current, parts[idx]);
            if (current == null) return null;
            idx++;
        }
        return current;
    }

    static Transform FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i);
            if (ch.name == name) return ch;
        }
        return null;
    }

    static Transform FindInSceneByObjectPath(Scene scene, string objectPath)
    {
        if (!scene.IsValid() || string.IsNullOrEmpty(objectPath)) return null;
        string[] parts = objectPath.Split('/');
        if (parts.Length == 0) return null;

        var roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            var root = roots[i].transform;
            if (root.name != parts[0]) continue;

            Transform current = root;
            int idx = 1;
            while (idx < parts.Length)
            {
                current = FindChildByName(current, parts[idx]);
                if (current == null) break;
                idx++;
            }
            if (current != null && idx == parts.Length) return current;
        }
        return null;
    }

    // ---------- APPLY HELPERS (assign by NAME, create if needed) ----------
    static bool TrySetLayerAndTag(GameObject go, string layerName, string tagName)
    {
        bool changed = false;

        // Layer by NAME (not index). Create if missing.
        if (!string.IsNullOrEmpty(layerName))
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
            {
                if (EnsureLayerExists(layerName, out layer))
                {
                    // ok
                }
                else
                {
                    Debug.LogWarning($"[LayerReportExporter] Could not ensure/create layer '{layerName}'. Skipping layer set on '{go.name}'.");
                    layer = -1;
                }
            }
            if (layer >= 0 && go.layer != layer)
            {
                go.layer = layer;
                changed = true;
            }
        }

        // Tag (create if missing; skip if empty or Untagged)
        if (!string.IsNullOrEmpty(tagName) && tagName != "Untagged")
        {
            if (!UnityEditorInternal.InternalEditorUtility.tags.Contains(tagName))
            {
                if (!EnsureTagExists(tagName))
                    Debug.LogWarning($"[LayerReportExporter] Could not ensure/create tag '{tagName}'. Skipping tag set on '{go.name}'.");
            }
            if (go.tag != tagName && UnityEditorInternal.InternalEditorUtility.tags.Contains(tagName))
            {
                go.tag = tagName;
                changed = true;
            }
        }

        return changed;
    }

    static bool EnsureLayerExists(string layerName, out int index)
    {
        index = LayerMask.NameToLayer(layerName);
        if (index >= 0) return true;

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layersProp = tagManager.FindProperty("layers");
        if (layersProp == null || !layersProp.isArray) { index = -1; return false; }

        // Already present?
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            var sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && sp.stringValue == layerName)
            {
                tagManager.ApplyModifiedProperties();
                index = i;
                return true;
            }
        }

        // Find an empty slot in user layer range (8..31)
        for (int i = 8; i < layersProp.arraySize; i++)
        {
            var sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                index = i;
                return true;
            }
        }

        index = -1;
        return false; // no free user layer slots
    }

    static bool EnsureTagExists(string tagName)
    {
        if (string.IsNullOrEmpty(tagName)) return false;
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tagsProp = tagManager.FindProperty("tags");
        if (tagsProp == null || !tagsProp.isArray) return false;

        // Already exists?
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            var sp = tagsProp.GetArrayElementAtIndex(i);
            if (sp != null && sp.stringValue == tagName)
            {
                return true;
            }
        }

        // Append new tag
        int newIndex = tagsProp.arraySize;
        tagsProp.InsertArrayElementAtIndex(newIndex);
        var newTag = tagsProp.GetArrayElementAtIndex(newIndex);
        newTag.stringValue = tagName;
        tagManager.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return true;
    }
}
#endif