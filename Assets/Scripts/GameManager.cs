using UnityEngine;
using GangsterMafia.Constants;
using GangsterMafia.Core;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Game State")]
    [SerializeField] private int _currentGameMode = 0; // 0 = None, 1 = Story, 2 = OpenWorld, 3 = Chase
    [SerializeField] private int _currentGameState = 0; // 0 = MainMenu, 1 = LevelSelection, 2 = Loading, 3 = Gameplay, 4 = Paused, 5 = GameOver
    
    [Header("Level Management")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private int _currentRandomLevel = 1;
    [SerializeField] private int _highestUnlockedLevel = 1;
    
    [Header("Player Progress")]
    [SerializeField] private int _currentPlayerIndex = 0;
    [SerializeField] private int _currentGangsterIndex = 0;
    [SerializeField] private int _totalCoins = 0;
    [SerializeField] private int _totalGems = 0;
    [SerializeField] private int _currentKillCount = 0;
    [SerializeField] private int _currentWeaponIndex = 0;
    
    [Header("Gameplay")]
    [SerializeField] private GameObject _policeCar;
    [SerializeField] private int _sceneModeCheck = 0;

    // Public Properties (Clean Architecture)
    public int CurrentGameMode 
    { 
        get => _currentGameMode; 
        set => _currentGameMode = value; 
    }
    public int CurrentGameState => _currentGameState;
    public int CurrentLevel 
    { 
        get => _currentLevel; 
        set => _currentLevel = value; 
    }
    public int CurrentRandomLevel => _currentRandomLevel;
    public int HighestUnlockedLevel => _highestUnlockedLevel;
    public int CurrentPlayer 
    { 
        get => _currentPlayerIndex; 
        set => _currentPlayerIndex = value; 
    }
    public int CurrentGangster 
    { 
        get => _currentGangsterIndex; 
        set => _currentGangsterIndex = value; 
    }
    public int TotalCoins 
    { 
        get => _totalCoins; 
        set => _totalCoins = value; 
    }
    public int TotalGems 
    { 
        get => _totalGems; 
        set => _totalGems = value; 
    }
    public int CurrentKillGangsters 
    { 
        get => _currentKillCount; 
        set => _currentKillCount = value; 
    }
    public int CurrentWeaponIndex => _currentWeaponIndex;
    public GameObject PoliceCar => _policeCar;
    public int CurrentMode 
    { 
        get => _currentGameMode; 
        set => _currentGameMode = value; 
    }
    public int CheckSceneForMode 
    { 
        get => _sceneModeCheck; 
        set => _sceneModeCheck = value; 
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: Initializing...");
            LoadGameData();
            Debug.Log($"GameManager: Initialized with Player: {_currentPlayerIndex}, Gangster: {_currentGangsterIndex}, Level: {_currentLevel}");
        }
        else if (_instance != this)
        {
            Debug.Log("GameManager: Another instance exists, destroying this one");
            Destroy(gameObject);
        }
    }

    private void LoadGameData()
    {
        // Ensure DataManager is initialized
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager not found! Creating one...");
            GameObject dataManagerGO = new GameObject("DataManager");
            dataManagerGO.AddComponent<GangsterMafia.Core.DataManager>();
        }
        
        try
        {
            _totalCoins = DataManager.Instance.GetInt(PlayerPrefsKeys.TOTAL_SCORE, 0);
            _totalGems = DataManager.Instance.GetInt("TotalGems", 0);
            _currentLevel = DataManager.Instance.GetInt(PlayerPrefsKeys.CURRENT_LEVEL, 1);
            _highestUnlockedLevel = DataManager.Instance.GetInt(PlayerPrefsKeys.HIGHEST_LEVEL, 1);
            _currentPlayerIndex = DataManager.Instance.GetInt(PlayerPrefsKeys.SELECTED_GANGSTER_INDEX, 0);
            _currentGangsterIndex = DataManager.Instance.GetInt(PlayerPrefsKeys.SELECTED_GANGSTER_INDEX, 0);
            _currentGameMode = DataManager.Instance.GetInt(PlayerPrefsKeys.GAME_MODE, 0);
            
            // Validate and clamp values to prevent array index out of bounds
            _currentPlayerIndex = Mathf.Clamp(_currentPlayerIndex, 0, 10); // Assuming max 10 cars
            _currentGangsterIndex = Mathf.Clamp(_currentGangsterIndex, 0, 10); // Assuming max 10 gangsters
            _currentLevel = Mathf.Clamp(_currentLevel, 1, 20); // Assuming max 20 levels
            
            Debug.Log($"GameManager: Loaded data - Player: {_currentPlayerIndex}, Gangster: {_currentGangsterIndex}, Level: {_currentLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game data: {e.Message}");
            // Set default values if DataManager fails
            _totalCoins = 0;
            _totalGems = 0;
            _currentLevel = 1;
            _highestUnlockedLevel = 1;
            _currentPlayerIndex = 0;
            _currentGangsterIndex = 0;
            _currentGameMode = 0; // Default to 0 (None)
        }
    }

    public void SaveGameData()
    {
        // Ensure DataManager is initialized
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager not found! Cannot save game data.");
            return;
        }
        
        try
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.TOTAL_SCORE, _totalCoins);
            DataManager.Instance.SetInt("TotalGems", _totalGems);
            DataManager.Instance.SetInt(PlayerPrefsKeys.CURRENT_LEVEL, _currentLevel);
            DataManager.Instance.SetInt(PlayerPrefsKeys.HIGHEST_LEVEL, _highestUnlockedLevel);
            DataManager.Instance.SetInt(PlayerPrefsKeys.SELECTED_GANGSTER_INDEX, _currentGangsterIndex);
            DataManager.Instance.SetInt(PlayerPrefsKeys.GAME_MODE, _currentGameMode);
            DataManager.Instance.SaveAllData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game data: {e.Message}");
        }
    }

    // Game Mode Management
    public void SetGameMode(int mode)
    {
        _currentGameMode = mode;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.GAME_MODE, mode);
        }
    }

    public void SetGameState(int state)
    {
        _currentGameState = state;
    }

    // Level Management
    public void SetCurrentLevel(int level)
    {
        // Validate and clamp the level to prevent array out of bounds
        level = Mathf.Clamp(level, 1, 20); // Assuming max 20 levels
        _currentLevel = level;
        if (level > _highestUnlockedLevel)
        {
            _highestUnlockedLevel = level;
        }
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.CURRENT_LEVEL, _currentLevel);
            DataManager.Instance.SetInt(PlayerPrefsKeys.HIGHEST_LEVEL, _highestUnlockedLevel);
        }
        Debug.Log($"GameManager: Set current level to {level}");
    }

    public void SetRandomLevel(int level)
    {
        _currentRandomLevel = level;
    }

    // Player Progress
    public void SetCurrentPlayer(int playerIndex)
    {
        // Validate and clamp the player index to prevent array out of bounds
        playerIndex = Mathf.Clamp(playerIndex, 0, 10); // Assuming max 10 cars
        _currentPlayerIndex = playerIndex;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.SELECTED_GANGSTER_INDEX, playerIndex);
        }
        Debug.Log($"GameManager: Set current player to {playerIndex}");
    }

    public void SetCurrentGangster(int gangsterIndex)
    {
        // Validate and clamp the gangster index to prevent array out of bounds
        gangsterIndex = Mathf.Clamp(gangsterIndex, 0, 10); // Assuming max 10 gangsters
        _currentGangsterIndex = gangsterIndex;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.SELECTED_GANGSTER_INDEX, gangsterIndex);
        }
        Debug.Log($"GameManager: Set current gangster to {gangsterIndex}");
    }

    public void AddCoins(int amount)
    {
        _totalCoins += amount;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt(PlayerPrefsKeys.TOTAL_SCORE, _totalCoins);
        }
    }

    public void AddGems(int amount)
    {
        _totalGems += amount;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetInt("TotalGems", _totalGems);
        }
    }

    public void SetKillCount(int count)
    {
        _currentKillCount = count;
    }

    public void SetWeaponIndex(int weaponIndex)
    {
        _currentWeaponIndex = weaponIndex;
    }

    // Initial Cut Scene Management
    public bool HasPlayedInitialCutscene()
    {
        if (DataManager.Instance != null)
        {
            return DataManager.Instance.GetBool(PlayerPrefsKeys.INITIAL_CUTSCENE_PLAYED, false);
        }
        return false;
    }

    public void MarkInitialCutsceneAsPlayed()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetBool(PlayerPrefsKeys.INITIAL_CUTSCENE_PLAYED, true);
        }
    }

    public void ResetInitialCutsceneState()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetBool(PlayerPrefsKeys.INITIAL_CUTSCENE_PLAYED, false);
        }
    }

    // Legacy compatibility method
    public void SaveTotalCoins()
    {
        SaveGameData();
    }

    // Cleanup
    private void OnDestroy()
    {
        if (_instance == this)
        {
            SaveGameData();
        }
    }
}
