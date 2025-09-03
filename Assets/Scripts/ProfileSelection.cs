using System.Collections;
using System.Collections.Generic;
using GangsterMafia.Core;
using UnityEngine;
using UnityEngine.UI;
using GangsterMafia.Sounds;

public class ProfileSelection : MonoBehaviour
{
    public static ProfileSelection instance;
    public InputField nameInputField;

    public string playerName;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        LoadProfileData();
    }

    public void SaveProfileData()
    {
        playerName = nameInputField.text;

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            PlayerPrefs.SetString("PlayerName", playerName);
        }
        else
        {
            playerName = "New_User";
            PlayerPrefs.SetString("PlayerName", playerName);
        }

        PlayerPrefs.SetInt("Profile", 1);
        PlayerPrefs.Save();

        MainMenuHandler.instance.Name.text = playerName;
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.Profile.SetActive(false);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

   public void LoadProfileData()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            playerName = "New_User";
            PlayerPrefs.SetString("PlayerName", playerName);
        }

        nameInputField.text = playerName;
    }
}
