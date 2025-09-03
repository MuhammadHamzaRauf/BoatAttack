using UnityEngine;


public class AudioClipsSource : MonoBehaviour
{

    [Header("Music Clips")]
    public AudioClip MainMenuClip;
    public AudioClip GamePlayClip;
    [Header("Button Click Sounds")]
    public AudioClip PlayBtn;
    public AudioClip BtnClick;
    public AudioClip BackBtn;
    public AudioClip CantPurchase;
    public AudioClip Purchased;
    public AudioClip LevelWin;
    public AudioClip LevelLose;
    public AudioClip checkPointForCoins;
    public AudioClip dailyrewardClaim;
    public AudioClip Coins;
    public static AudioClipsSource Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
