using System;
using System.Collections.Generic;
using UnityEngine;

namespace GangsterMafia.Core
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<string, Action<object>> _eventDictionary = new Dictionary<string, Action<object>>();
        private Dictionary<string, Action> _eventDictionaryNoParams = new Dictionary<string, Action>();

        // Generic event with parameters
        public void AddListener<T>(string eventName, Action<T> listener)
        {
            if (!_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName] = null;
            }
            _eventDictionary[eventName] += (obj) => listener((T)obj);
        }

        public void RemoveListener<T>(string eventName, Action<T> listener)
        {
            if (_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName] -= (obj) => listener((T)obj);
            }
        }

        public void TriggerEvent<T>(string eventName, T eventData)
        {
            if (_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName]?.Invoke(eventData);
            }
        }

        // Events without parameters
        public void AddListener(string eventName, Action listener)
        {
            if (!_eventDictionaryNoParams.ContainsKey(eventName))
            {
                _eventDictionaryNoParams[eventName] = null;
            }
            _eventDictionaryNoParams[eventName] += listener;
        }

        public void RemoveListener(string eventName, Action listener)
        {
            if (_eventDictionaryNoParams.ContainsKey(eventName))
            {
                _eventDictionaryNoParams[eventName] -= listener;
            }
        }

        public void TriggerEvent(string eventName)
        {
            if (_eventDictionaryNoParams.ContainsKey(eventName))
            {
                _eventDictionaryNoParams[eventName]?.Invoke();
            }
        }

        // Clear all events (useful for scene transitions)
        public void ClearAllEvents()
        {
            _eventDictionary.Clear();
            _eventDictionaryNoParams.Clear();
        }

        // Clear specific event
        public void ClearEvent(string eventName)
        {
            if (_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary.Remove(eventName);
            }
            if (_eventDictionaryNoParams.ContainsKey(eventName))
            {
                _eventDictionaryNoParams.Remove(eventName);
            }
        }
    }

    // Static helper class for easier access
    public static class Events
    {
        // Game State Events
        public const string GAME_STATE_CHANGED = "GameStateChanged";
        public const string GAME_MODE_CHANGED = "GameModeChanged";
        public const string LEVEL_LOADED = "LevelLoaded";
        public const string LEVEL_COMPLETED = "LevelCompleted";
        public const string GAME_PAUSED = "GamePaused";
        public const string GAME_RESUMED = "GameResumed";
        
        // Player Events
        public const string PLAYER_SPAWNED = "PlayerSpawned";
        public const string PLAYER_DIED = "PlayerDied";
        public const string PLAYER_HEALTH_CHANGED = "PlayerHealthChanged";
        public const string PLAYER_WEAPON_CHANGED = "PlayerWeaponChanged";
        
        // Car Events
        public const string CAR_SELECTED = "CarSelected";
        public const string CAR_PURCHASED = "CarPurchased";
        public const string CAR_DESTROYED = "CarDestroyed";
        
        // UI Events
        public const string UI_PANEL_OPENED = "UIPanelOpened";
        public const string UI_PANEL_CLOSED = "UIPanelClosed";
        public const string BUTTON_CLICKED = "ButtonClicked";
        
        // Audio Events
        public const string MUSIC_VOLUME_CHANGED = "MusicVolumeChanged";
        public const string SFX_VOLUME_CHANGED = "SFXVolumeChanged";
        public const string AUDIO_MUTED = "AudioMuted";
        
        // Mission Events
        public const string MISSION_STARTED = "MissionStarted";
        public const string MISSION_COMPLETED = "MissionCompleted";
        public const string MISSION_FAILED = "MissionFailed";
        public const string OBJECTIVE_UPDATED = "ObjectiveUpdated";
        
        // Economy Events
        public const string COINS_EARNED = "CoinsEarned";
        public const string COINS_SPENT = "CoinsSpent";
        public const string GEMS_EARNED = "GemsEarned";
        public const string GEMS_SPENT = "GemsSpent";
    }
}
