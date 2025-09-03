using UnityEngine;
using System;
using System.Collections.Generic;
using GangsterMafia.Constants;

namespace GangsterMafia.Core
{
    public class DataManager : Singleton<DataManager>
    {
        [Header("Data Settings")]
        [SerializeField] private bool _autoSave = true;
        [SerializeField] private float _autoSaveInterval = 30f; // seconds
        
        private float _lastAutoSaveTime;
        private Dictionary<string, object> _cachedData = new Dictionary<string, object>();
        
        // Events
        public System.Action OnDataSaved;
        public System.Action OnDataLoaded;
        public System.Action<string> OnDataChanged;
        
        protected override void OnSingletonAwake()
        {
            LoadAllData();
            
            if (_autoSave)
            {
                _lastAutoSaveTime = Time.time;
            }
        }
        
        private void Update()
        {
            if (_autoSave && Time.time - _lastAutoSaveTime >= _autoSaveInterval)
            {
                SaveAllData();
                _lastAutoSaveTime = Time.time;
            }
        }
        
        // Generic data methods
        public void SetData<T>(string key, T value)
        {
            if (value == null)
            {
                Debug.LogWarning($"Attempting to save null value for key: {key}");
                return;
            }
            
            _cachedData[key] = value;
            
            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)(object)value);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)(object)value);
            }
            else if (typeof(T) == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
            }
            else
            {
                Debug.LogWarning($"Unsupported data type for key {key}: {typeof(T)}");
                return;
            }
            
            OnDataChanged?.Invoke(key);
        }
        
        public T GetData<T>(string key, T defaultValue = default(T))
        {
            if (_cachedData.ContainsKey(key))
            {
                return (T)_cachedData[key];
            }
            
            T value = defaultValue;
            
            if (typeof(T) == typeof(int))
            {
                value = (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            }
            else if (typeof(T) == typeof(float))
            {
                value = (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            }
            else if (typeof(T) == typeof(string))
            {
                value = (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                value = (T)(object)(PlayerPrefs.GetInt(key, (bool)(object)defaultValue ? 1 : 0) == 1);
            }
            
            _cachedData[key] = value;
            return value;
        }
        
        // Specific data methods for common types
        public void SetInt(string key, int value)
        {
            SetData(key, value);
        }
        
        public int GetInt(string key, int defaultValue = 0)
        {
            return GetData(key, defaultValue);
        }
        
        public void SetFloat(string key, float value)
        {
            SetData(key, value);
        }
        
        public float GetFloat(string key, float defaultValue = 0f)
        {
            return GetData(key, defaultValue);
        }
        
        public void SetString(string key, string value)
        {
            SetData(key, value);
        }
        
        public string GetString(string key, string defaultValue = "")
        {
            return GetData(key, defaultValue);
        }
        
        public void SetBool(string key, bool value)
        {
            SetData(key, value);
        }
        
        public bool GetBool(string key, bool defaultValue = false)
        {
            return GetData(key, defaultValue);
        }
        
        // Array/List methods
        public void SetIntArray(string key, int[] array)
        {
            string json = JsonUtility.ToJson(new IntArrayWrapper { array = array });
            SetString(key, json);
        }
        
        public int[] GetIntArray(string key, int[] defaultValue = null)
        {
            string json = GetString(key, "");
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue ?? new int[0];
            }
            
            try
            {
                IntArrayWrapper wrapper = JsonUtility.FromJson<IntArrayWrapper>(json);
                return wrapper?.array ?? defaultValue ?? new int[0];
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse int array for key {key}: {e.Message}");
                return defaultValue ?? new int[0];
            }
        }
        
        public void SetStringArray(string key, string[] array)
        {
            string json = JsonUtility.ToJson(new StringArrayWrapper { array = array });
            SetString(key, json);
        }
        
        public string[] GetStringArray(string key, string[] defaultValue = null)
        {
            string json = GetString(key, "");
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue ?? new string[0];
            }
            
            try
            {
                StringArrayWrapper wrapper = JsonUtility.FromJson<StringArrayWrapper>(json);
                return wrapper?.array ?? defaultValue ?? new string[0];
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse string array for key {key}: {e.Message}");
                return defaultValue ?? new string[0];
            }
        }
        
        // Save/Load all data
        public void SaveAllData()
        {
            PlayerPrefs.Save();
            OnDataSaved?.Invoke();
        }
        
        public void LoadAllData()
        {
            _cachedData.Clear();
            OnDataLoaded?.Invoke();
        }
        
        // Delete specific data
        public void DeleteData(string key)
        {
            if (_cachedData.ContainsKey(key))
            {
                _cachedData.Remove(key);
            }
            
            PlayerPrefs.DeleteKey(key);
            OnDataChanged?.Invoke(key);
        }
        
        // Delete all data
        public void DeleteAllData()
        {
            _cachedData.Clear();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        
        // Check if data exists
        public bool HasData(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        
        // Get all keys
        public string[] GetAllKeys()
        {
            // Note: Unity doesn't provide a direct way to get all keys
            // This is a workaround that checks common keys
            List<string> keys = new List<string>();
            
            // Check common keys from PlayerPrefsKeys
            var fields = typeof(PlayerPrefsKeys).GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic && field.FieldType == typeof(string))
                {
                    string key = (string)field.GetValue(null);
                    if (PlayerPrefs.HasKey(key))
                    {
                        keys.Add(key);
                    }
                }
            }
            
            return keys.ToArray();
        }
        
        // Wrapper classes for JSON serialization
        [System.Serializable]
        private class IntArrayWrapper
        {
            public int[] array;
        }
        
        [System.Serializable]
        private class StringArrayWrapper
        {
            public string[] array;
        }
        
        // Cleanup
        protected override void OnDestroy()
        {
            if (_autoSave)
            {
                SaveAllData();
            }
        }
    }
}
