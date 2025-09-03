using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GangsterMafia.Constants;

namespace GangsterMafia.Core
{
    public class SceneManager : Singleton<SceneManager>
    {
        [Header("Loading Settings")]
        [SerializeField] private float _minimumLoadingTime = 1.5f;
        [SerializeField] private string _loadingSceneName = GameConstants.LOADING_SCENE;
        
        private bool _isLoading = false;
        private string _targetSceneName = "";
        
        // Events
        public System.Action<string> OnSceneLoadStarted;
        public System.Action<string> OnSceneLoadCompleted;
        public System.Action<float> OnLoadingProgressChanged;
        
        protected override void OnSingletonAwake()
        {
            // Subscribe to scene load events
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            // if (UnityEngine.SceneManagement.SceneManager.sceneLoaded != null)
            // {
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            // }
        }
        
        // Load scene with loading screen
        public void LoadScene(string sceneName, bool showLoadingScreen = true)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"Scene loading already in progress. Cannot load {sceneName}");
                return;
            }
            
            if (showLoadingScreen)
            {
                StartCoroutine(LoadSceneWithLoading(sceneName));
            }
            else
            {
                LoadSceneDirect(sceneName);
            }
        }
        
        // Load scene directly without loading screen
        public void LoadSceneDirect(string sceneName)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"Scene loading already in progress. Cannot load {sceneName}");
                return;
            }
            
            _isLoading = true;
            _targetSceneName = sceneName;
            
            OnSceneLoadStarted?.Invoke(sceneName);
            
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene {sceneName}: {e.Message}");
                _isLoading = false;
            }
        }
        
        // Load scene with loading screen
        private IEnumerator LoadSceneWithLoading(string sceneName)
        {
            _isLoading = true;
            _targetSceneName = sceneName;
            
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // Load loading scene first
            UnityEngine.SceneManagement.SceneManager.LoadScene(_loadingSceneName);
            
            yield return new WaitForSeconds(0.1f); // Wait for loading scene to initialize
            
            // Start loading the target scene
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            
            float startTime = Time.time;
            float progress = 0f;
            
            // Update loading progress
            while (asyncLoad.progress < 0.9f)
            {
                progress = asyncLoad.progress;
                OnLoadingProgressChanged?.Invoke(progress);
                yield return null;
            }
            
            // Ensure minimum loading time
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < _minimumLoadingTime)
            {
                yield return new WaitForSeconds(_minimumLoadingTime - elapsedTime);
            }
            
            // Complete loading
            asyncLoad.allowSceneActivation = true;
            progress = 1f;
            OnLoadingProgressChanged?.Invoke(progress);
            
            // Wait for scene to fully load
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        // Reload current scene
        public void ReloadCurrentScene()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }
        
        // Load next scene in build index
        public void LoadNextScene()
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;
            
            if (nextIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                string nextSceneName = GetSceneNameByBuildIndex(nextIndex);
                LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("No next scene available. This is the last scene in build settings.");
            }
        }
        
        // Load previous scene in build index
        public void LoadPreviousScene()
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int previousIndex = currentIndex - 1;
            
            if (previousIndex >= 0)
            {
                string previousSceneName = GetSceneNameByBuildIndex(previousIndex);
                LoadScene(previousSceneName);
            }
            else
            {
                Debug.LogWarning("No previous scene available. This is the first scene in build settings.");
            }
        }
        
        // Get scene name by build index
        private string GetSceneNameByBuildIndex(int buildIndex)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(buildIndex);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            return sceneName;
        }
        
        // Check if scene exists
        public bool SceneExists(string sceneName)
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).path;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
        
        // Get current scene name
        public string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        
        // Get current scene build index
        public int GetCurrentSceneBuildIndex()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }
        
        // Check if currently loading
        public bool IsLoading => _isLoading;
        
        // Get target scene name
        public string GetTargetSceneName => _targetSceneName;
        
        // Scene loaded callback
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isLoading = false;
            OnSceneLoadCompleted?.Invoke(scene.name);
            
            // Trigger event for other systems
            EventManager.Instance.TriggerEvent(Events.LEVEL_LOADED, scene.name);
        }
        
        // Quit application
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
