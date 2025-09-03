using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GangsterMafia.Core;
using GangsterMafia.Constants;

namespace GangsterMafia.Sounds
{
	public class SoundManager : Singleton<SoundManager> {

		[Header("Audio Sources")]
		[SerializeField] public AudioSource _BGAudioSource;
		[SerializeField] public AudioSource _FGAudioSource;
		
		[Header("Audio Clips")]
		[SerializeField] private AudioClip _defaultBGClip = null;
		
		[Header("Audio Settings")]
		[SerializeField] private bool _soundEnabled = true;
		[SerializeField] private bool _musicEnabled = true;
		[SerializeField] private bool _effectEnabled = true;
		[SerializeField] private bool _vibrationStatus = true;
		
		private float _effectsVolume = 0.7f;
		private float _musicVolume = 0.5f;


	    protected override void OnSingletonAwake()
	    {
	        // Add required components if not already assigned
	        if (_FGAudioSource == null)
	        {
	            _FGAudioSource = gameObject.AddComponent<AudioSource>();
	            _FGAudioSource.name = "(AudioSource)FG";
	        }
	        
	        if (_BGAudioSource == null)
	        {
	            _BGAudioSource = gameObject.AddComponent<AudioSource>();
	            _BGAudioSource.name = "(AudioSource)BG";
	        }
	        
	        // Load persistent settings
	        LoadAudioSettings();
	        
	        // Set default volumes
	        _BGAudioSource.volume = _musicVolume;
	        _FGAudioSource.volume = _effectsVolume;
	    }
	    
	    private void LoadAudioSettings()
	    {
	        _soundEnabled = PlayerPrefs.GetInt(PlayerPrefsKeys.SFX_MUTED, 0) == 0;
	        _musicEnabled = PlayerPrefs.GetInt(PlayerPrefsKeys.MUSIC_MUTED, 0) == 0;
	        _effectsVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFX_VOLUME, GameConstants.DEFAULT_SFX_VOLUME);
	        _musicVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MUSIC_VOLUME, GameConstants.DEFAULT_MUSIC_VOLUME);
	    }

	    public bool IsEffectsPlaying()
	    {
	        if (_FGAudioSource.isPlaying)
	        {
	            return true;
	        }
	        else
	        {
	            return false;
	        }
	    }

    		public bool EnableSounds {
			set { 
				_soundEnabled = value;
				_FGAudioSource.enabled = _soundEnabled;
				PlayerPrefs.SetInt(PlayerPrefsKeys.SFX_MUTED, _soundEnabled ? 0 : 1);
				PlayerPrefs.Save();
			}
			get { 
				return _soundEnabled;
			}
		}

		public bool MusicEnabled {
			set {
				_musicEnabled = value;
				_BGAudioSource.enabled = _musicEnabled;
				PlayerPrefs.SetInt(PlayerPrefsKeys.MUSIC_MUTED, _musicEnabled ? 0 : 1);
				PlayerPrefs.Save();
			}
			get { 
				return _musicEnabled;
			}
		}


		
		public float EffectsVolume {
			set { 
				_effectsVolume = Mathf.Clamp01(value);
				_FGAudioSource.volume = _effectsVolume;
				PlayerPrefs.SetFloat(PlayerPrefsKeys.SFX_VOLUME, _effectsVolume);
				PlayerPrefs.Save();
			}
			get {
				return _effectsVolume;
			}
		}
		
		public float MusicVolume {
			set { 
				_musicVolume = Mathf.Clamp01(value);
				_BGAudioSource.volume = _musicVolume;
				PlayerPrefs.SetFloat(PlayerPrefsKeys.MUSIC_VOLUME, _musicVolume);
				PlayerPrefs.Save();
			}
			get {
				return _musicVolume;
			}
		}

		public AudioClip DefaultBGClip {
			set { 
				_defaultBGClip = value;
			}
			get { 
				return _defaultBGClip;
			}
		}
		

		public void Play ()
		{
			if (_defaultBGClip ==null) {
				Debug.LogWarning ("Default Audio Clip is required for SoundManager ");
				return;
			}
			PlayBackgroundMusic (_defaultBGClip);
		}


		public void PlayEffect (AudioClip _clip)
		{
			if (_soundEnabled & _clip != null) {
				_FGAudioSource.PlayOneShot (_clip);
			}
		}

		public void PlayVocal (AudioClip _clip)
		{
			if (_soundEnabled & _clip != null) {
				_FGAudioSource.Stop ();
				_FGAudioSource.clip = _clip;
				_FGAudioSource.Play ();
			}

		}
		public void PlayEffect(AudioClip _clip , bool status){
			_FGAudioSource.clip = _clip;
			_FGAudioSource.loop = status;
			_FGAudioSource.Play ();
		}
		public void PlayVibration(bool status)
		{
	        if (status)
			{
				Handheld.Vibrate();
			}
		}
		public void StopEffect(AudioClip _clip){
			_FGAudioSource.Stop ();

		}
		public void StopMusic(AudioClip _clip){
			_BGAudioSource.Stop ();

		}

		public void PlayBackgroundMusic (AudioClip _clip)
		{
			if (_soundEnabled && _clip != null) {
				_BGAudioSource.clip = _clip;
				_BGAudioSource.loop = true;
				_BGAudioSource.Play ();
			}
		}
	}
}
