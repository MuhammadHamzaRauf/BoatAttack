using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class SettingHandler : MonoBehaviour
{
    public static SettingHandler instance;

    public GameObject lowGraphics, mediumGraphics, highGraphics;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider vibrationSlider;

    public enum GraphicsQuality { Low = 0, Medium = 1, High = 2 }
    private GraphicsQuality currentQuality;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        vibrationSlider.value = PlayerPrefs.GetFloat("Vibration", 1f);

        SoundManager.Instance._BGAudioSource.volume = musicSlider.value;
        SoundManager.Instance._FGAudioSource.volume = soundSlider.value;

        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", (int)GraphicsQuality.Medium);
        ApplyQualitySetting((GraphicsQuality)savedQuality);
    }

    #region Sound & Volume
    public void OnMusicVolumeChanged()
    {
        SoundManager.Instance._BGAudioSource.volume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void OnSoundVolumeChanged()
    {
        SoundManager.Instance._FGAudioSource.volume = soundSlider.value;
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();
    }

    public void OnVibrationChanged()
    {
        PlayerPrefs.SetFloat("Vibration", vibrationSlider.value);
        PlayerPrefs.Save();
    }
    #endregion

    public void OnClickBackButton()
    {
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.setting.SetActive(false);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    #region Graphics Settings
    public void OnClickLowGraphics()
    {
        ApplyQualitySetting(GraphicsQuality.Low);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void OnClickMediumGraphics()
    {
        ApplyQualitySetting(GraphicsQuality.Medium);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void OnClickHighGraphics()
    {
        ApplyQualitySetting(GraphicsQuality.High);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    private void ApplyQualitySetting(GraphicsQuality qualitySetting)
    {
        currentQuality = qualitySetting;
        int unityQualityIndex = GetUnityQualityLevel(qualitySetting);
        QualitySettings.SetQualityLevel(unityQualityIndex, true);

        lowGraphics.GetComponent<Image>().color = Color.white;
        mediumGraphics.GetComponent<Image>().color = Color.white;
        highGraphics.GetComponent<Image>().color = Color.white;

        switch (qualitySetting)
        {
            case GraphicsQuality.Low:
                lowGraphics.GetComponent<Image>().color = Color.yellow;
                break;
            case GraphicsQuality.Medium:
                mediumGraphics.GetComponent<Image>().color = Color.yellow;
                break;
            case GraphicsQuality.High:
                highGraphics.GetComponent<Image>().color = Color.yellow;
                break;
        }

        PlayerPrefs.SetInt("GraphicsQuality", (int)qualitySetting);
        PlayerPrefs.Save();
    }

    private int GetUnityQualityLevel(GraphicsQuality quality)
    {
        switch (quality)
        {
            case GraphicsQuality.Low: return 2;
            case GraphicsQuality.Medium: return 3;
            case GraphicsQuality.High: return 5;
            default: return 3;
        }
    }
    #endregion
}
