using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class DailyRewardSystem : MonoBehaviour
{
    public static DailyRewardSystem instance;
    private const string LastClaimedTimeKey = "LastClaimedTime";
    private const string CurrentDayKey = "CurrentDay";

    private DateTime lastClaimedTime;
    private int currentDay;

    public Text coins;
    public Text gems;
    public Text rewardAmountText;

    public Text time;
    public GameObject rewardClaim;
    public GameObject[] dailyrewards;
    public GameObject[] lockImage;
    public GameObject[] claimImage;
    public GameObject[] coinImage;
    public GameObject CLAIMBUUTTON;

    private void Start()
    {
        instance = this;
        LoadData();
        CheckRewardAvailability();
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }

    private void Update()
    {
        UpdateTimer();
    }

    void LoadData()
    {
        if (PlayerPrefs.HasKey(LastClaimedTimeKey))
        {
            string lastClaimedTimeString = PlayerPrefs.GetString(LastClaimedTimeKey);
            lastClaimedTime = DateTime.Parse(lastClaimedTimeString);
        }
        else
        {
            lastClaimedTime = DateTime.MinValue;
        }

        currentDay = PlayerPrefs.GetInt(CurrentDayKey, 0);
    }

    void SaveData()
    {
        PlayerPrefs.SetString(LastClaimedTimeKey, lastClaimedTime.ToString());
        PlayerPrefs.SetInt(CurrentDayKey, currentDay);
        PlayerPrefs.Save();
    }

    void CheckRewardAvailability()
    {
        TimeSpan timeSinceLastClaim = DateTime.Now - lastClaimedTime;

        for (int i = 0; i < 7; i++)
        {
            claimImage[i].SetActive(false);
            lockImage[i].SetActive(true);
        }
        lockImage[0].SetActive(false);

        for (int i = 0; i < currentDay; i++)
        {
            lockImage[i].SetActive(false);
            claimImage[i].SetActive(true);
        }

        bool rewardIsReady = timeSinceLastClaim.TotalHours >= 24 || lastClaimedTime == DateTime.MinValue;

        if (rewardIsReady)
        {
            CLAIMBUUTTON.SetActive(true);
            time.text = "00:00:00";

            int todayIndex = currentDay % 7;
        }
        else
        {
            CLAIMBUUTTON.SetActive(false);

            TimeSpan remaining = TimeSpan.FromHours(24) - timeSinceLastClaim;
            time.text = $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }
    }

    void UpdateTimer()
    {
        TimeSpan timeSinceLastClaim = DateTime.Now - lastClaimedTime;
        int dayDisplay = (currentDay % 7) + 1;

        if (timeSinceLastClaim.TotalHours >= 24 || lastClaimedTime == DateTime.MinValue)
        {
            time.text = $"DAY {dayDisplay} NEXT DAY 00:00:00";
        }
        else
        {
            TimeSpan remaining = TimeSpan.FromHours(24) - timeSinceLastClaim;
            time.text = $"DAY {dayDisplay} REWARD IN {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }
    }

    public void ClaimReward()
    {
        TimeSpan timeSinceLastClaim = DateTime.Now - lastClaimedTime;

        if (timeSinceLastClaim.TotalHours >= 24 || lastClaimedTime == DateTime.MinValue)
        {
            ProcessClaim();
        }
    }

    public void OnRewardedNextDayClaimButtonClicked()
    {
        ProcessClaim(true);
    }

 







    private void ProcessClaim(bool isAd = false)
    {
        currentDay = (currentDay % 7) + 1;

        int reward = 500 * currentDay;
        rewardAmountText.text = isAd
            ? $"You claimed {reward} coins (Ad Bonus)!"
            : $"You claimed {reward} coins!";

        GameManager.Instance.TotalCoins += reward;
        coins.text = GameManager.Instance.TotalCoins.ToString();
        GameManager.Instance.SaveTotalCoins();

        lastClaimedTime = DateTime.Now;

        int todayIndex = currentDay - 1;

        CLAIMBUUTTON.SetActive(false);
        lockImage[todayIndex].SetActive(false);
        claimImage[todayIndex].SetActive(true);

        SaveData();

        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.dailyrewardClaim);
        StartCoroutine(ShowRewardUI(rewardClaim));

        if (currentDay == 7)
        {
            StartCoroutine(ResetAfterDelay(3));
        }
    }

    private IEnumerator ShowRewardUI(GameObject rewardUI)
    {
        rewardUI.SetActive(true);
        yield return new WaitForSeconds(3);
        rewardUI.SetActive(false);
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentDay = 0;
        SaveData();

        for (int i = 0; i < 7; i++)
        {
            claimImage[i].SetActive(false);
            lockImage[i].SetActive(true);
        }

        lockImage[0].SetActive(false);

        CLAIMBUUTTON.SetActive(true);
    }

    public void OnClaimButtonClicked()
    {
        ClaimReward();
    }

    public void OnClickExitButton()
    {
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BackBtn);
        MainMenuHandler.instance.dailyRewardPanel.SetActive(false);
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        MainMenuHandler.instance.gems.text = GameManager.Instance.TotalGems.ToString();
    }
}