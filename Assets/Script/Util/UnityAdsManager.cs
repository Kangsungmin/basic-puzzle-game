using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.Advertisements;
using System;

public class UnityAdsManager : MonoBehaviour
{
    //////////////////////////////////
    public enum MoneyType
    {
        HEART,
        CRYSTAL
    }
    //////////////////////////////////


    public string m_gameID;
    private string placementId_video = "video";
    private string placementId_rewardedVideo = "rewardedVideo";
    private string bannerPlacement = "banner";
    private World world;
    private MoneyType m_moneyType = MoneyType.HEART;
    private int m_rewardAmount = 1;

    public bool m_testMode = true;

    //리워드 광고시청 충전 수
    public int m_fixedRefillNormalRewardLeft = 5;
    public int m_fixedRefillCrystalRewardLeft = 4;
    //리워드 광고시청 딜레이 시간(분)
    public int m_fixedNormalRewardDelaySec = 300;
    public int m_fixedCrystalRewardDelaySec = 600;

    public UIButton m_normalRewardButton;
    public UILabel m_normalDisableLabel;
    public UIButton m_crystalRewardButton;
    public UILabel m_crystalDisableLabel;

    public int m_refillNormalRewardLeft = 0;
    public int m_refillCrystalRewardLeft = 0;
    public int m_normalRewardDelaySec = 0;
    public int m_crystalRewardDelaySec = 0;

    public GameObject[] m_rewardObject;

    public AudioClip m_rewardAudioClip;
    public AudioSource m_adsRewardSource;

    private void Awake()
    {
        world = GetComponent<World>();
        //m_adsRewardSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_gameID = "3487812";
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            m_gameID = "3487813";
        }

        Monetization.Initialize(m_gameID, m_testMode);
        ShowBannerAd();

        //==========Load Pref==========
        if (world.RESET) PlayerPrefs.DeleteAll();

        m_refillNormalRewardLeft = PlayerPrefs.GetInt("UnityAdsManager.m_refillNormalRewardLeft", m_fixedRefillNormalRewardLeft);
        m_refillCrystalRewardLeft = PlayerPrefs.GetInt("UnityAdsManager.m_refillCrystalRewardLeft", m_fixedRefillCrystalRewardLeft);

        InitRewardButton();
        if (m_refillNormalRewardLeft == 0) StartCoroutine(NormalMinuteCounter());
        if (m_refillCrystalRewardLeft == 0) StartCoroutine(CrystalMinuteCounter());
         

    }

    public void InitRewardButton()
    {
        if(m_normalRewardButton != null) m_normalRewardButton.isEnabled = m_refillNormalRewardLeft > 0 ? true : false;
        if(m_crystalRewardButton != null) m_crystalRewardButton.isEnabled = m_refillCrystalRewardLeft > 0 ? true : false;
        if (m_normalDisableLabel != null && m_refillNormalRewardLeft != 0) m_normalDisableLabel.text = "( " + m_refillNormalRewardLeft + " )";
        if (m_crystalDisableLabel != null && m_refillCrystalRewardLeft != 0) m_crystalDisableLabel.text = "( " + m_refillCrystalRewardLeft + " )";
    }

    IEnumerator NormalMinuteCounter()
    {
        m_normalRewardDelaySec = PlayerPrefs.GetInt("UnityAdsManager.m_normalRewardDelaySec", 0);
        if (m_normalRewardDelaySec == 0) m_normalRewardDelaySec = m_fixedNormalRewardDelaySec;
        YieldInstruction instruction = new WaitForSeconds(1);
        bool isOver = false;
        while(isOver != true)
        {
            m_normalRewardDelaySec--;
            if(m_normalDisableLabel != null)
            {
                TimeSpan t = TimeSpan.FromSeconds((float)m_normalRewardDelaySec);
                string str = t.ToString(@"mm\:ss");
                m_normalDisableLabel.text = str;
            }

            if (m_normalRewardDelaySec == 0)
            {
                isOver = true;
                m_normalRewardDelaySec    = m_fixedNormalRewardDelaySec;
                m_refillNormalRewardLeft  = m_fixedRefillNormalRewardLeft;          // 남은 광고 시청 횟수를 초기화
                m_normalRewardButton.isEnabled = true;                              // 광고시청 버튼 재 활성화
                m_normalDisableLabel.text = "(" + m_refillNormalRewardLeft + ")";
            }
            if(m_normalRewardDelaySec % 60 == 0)                                    // 1분마다 현재 타임스탬프 저장한다.
            {
                SaveNormalTimeStamp();
            }
            yield return instruction;
        }
    }

    IEnumerator CrystalMinuteCounter()
    {
        m_crystalRewardDelaySec = PlayerPrefs.GetInt("UnityAdsManager.m_crystalRewardDelaySec", 0);
        if (m_crystalRewardDelaySec == 0) m_crystalRewardDelaySec = m_fixedCrystalRewardDelaySec; 
        YieldInstruction instruction = new WaitForSeconds(1);
        bool isOver = false;
        while (isOver != true)
        {
            if(m_crystalDisableLabel != null)
            {
                m_crystalRewardDelaySec--;
                TimeSpan t = TimeSpan.FromSeconds((float)m_crystalRewardDelaySec);
                string str = t.ToString(@"mm\:ss");
                m_crystalDisableLabel.text = str;
            }

            if (m_crystalRewardDelaySec == 0)
            {
                isOver = true;
                m_crystalRewardDelaySec = m_fixedCrystalRewardDelaySec;
                m_refillCrystalRewardLeft = m_fixedRefillCrystalRewardLeft;          // 남은 광고 시청 횟수를 초기화
                m_crystalRewardButton.isEnabled = true;                              // 광고시청 버튼 재 활성화
                m_crystalDisableLabel.text = "(" + m_refillCrystalRewardLeft + ")";
            }
            if (m_crystalRewardDelaySec % 60 == 0)                                    // 1분마다 현재 타임스탬프 저장한다.
            {
                SaveCrystalTimeStamp();
            }
            yield return instruction;
        }
    }

    private void SaveNormalTimeStamp()
    {
        PlayerPrefs.SetInt("UnityAdsManager.m_refillNormalRewardLeft", m_refillNormalRewardLeft);
        PlayerPrefs.SetInt("UnityAdsManager.m_normalRewardDelaySec", m_normalRewardDelaySec);
    }

    private void SaveCrystalTimeStamp()
    {
        PlayerPrefs.SetInt("UnityAdsManager.m_refillCrystalRewardLeft", m_refillCrystalRewardLeft);
        PlayerPrefs.SetInt("UnityAdsManager.m_crystalRewardDelaySec", m_crystalRewardDelaySec);
    }


    public void ShowAD()
    {
        StartCoroutine(WaitForAd());
    }

    public void ShowRewardedAD_Heart(int amount)
    {
        if(m_refillNormalRewardLeft > 0)
        {
            m_refillNormalRewardLeft--;
            m_normalDisableLabel.text = "( " + m_refillNormalRewardLeft + " )";
            SaveNormalTimeStamp();
            if (m_refillNormalRewardLeft == 0)
            {
                m_normalRewardButton.isEnabled = false;
                // 충전 시간 카운팅 시작.
                StartCoroutine(NormalMinuteCounter());      
            }
            //////////////////////////////////
            m_moneyType = MoneyType.HEART;
            m_rewardAmount = amount;
            //////////////////////////////////
            StartCoroutine(WaitForAd(true));
        }

    }

    public void ShowRewardedAD_Crystal(int amount)
    {
        if(m_refillCrystalRewardLeft > 0)
        {
            m_refillCrystalRewardLeft--;
            m_crystalDisableLabel.text = "( " + m_refillCrystalRewardLeft + " )";
            SaveCrystalTimeStamp();
            if(m_refillCrystalRewardLeft == 0)
            {
                m_crystalRewardButton.isEnabled = false;
                //충전 시간 카운팅 시작
                StartCoroutine(CrystalMinuteCounter());
            }
            //////////////////////////////////
            m_moneyType = MoneyType.CRYSTAL;
            m_rewardAmount = amount;
            //////////////////////////////////
            StartCoroutine(WaitForAd(true));
        }

    }

    IEnumerator WaitForAd(bool isReward = false, int amount = 1)
    {
        string placementId = isReward ? placementId_rewardedVideo : placementId_video;
        while(!Monetization.IsReady(placementId))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        if(ad != null)
        {
            if(isReward)
                ad.Show(AdFinished);
            else
                ad.Show();
        }
    }

    void AdFinished(UnityEngine.Monetization.ShowResult result)
    {
        if(result == UnityEngine.Monetization.ShowResult.Finished)
        {
            //Reward the player.
            switch(m_moneyType)
            {
                case MoneyType.HEART:
                    ShowRewardAnimation();
                    world.AddHeart(m_rewardAmount);
                    break;
                case MoneyType.CRYSTAL:
                    ShowRewardAnimation();
                    world.AddCrystal(m_rewardAmount);
                    break;
                default:
                    break;
            }

        }
    }

    public void ShowRewardAnimation() // 리워드 애니메이션 실행
    {
        Vector3 randomPosition;
        int screenWidth = (int)(Screen.width * 0.5f);
        int screenHeight = (int)(Screen.height * 0.5f);
        for (int i=0; i < 20; i++)
        {
            randomPosition = new Vector3(UnityEngine.Random.Range(-1 * screenWidth, screenWidth), UnityEngine.Random.Range(-1 * screenHeight, screenHeight), 0);
            GameObject pref = Instantiate(m_rewardObject[i % m_rewardObject.Length], Vector3.zero, Quaternion.identity);
            Transform camera = GameObject.FindGameObjectWithTag("NGUI_CAMERA").transform;
            pref.transform.SetParent(camera);
            pref.transform.localScale = new Vector3(1, 1, 1);
            pref.transform.localPosition = randomPosition;
            pref.GetComponent<UISprite>().depth = 10;
        }
        m_adsRewardSource.clip = m_rewardAudioClip;
        m_adsRewardSource.Play();
    }

    public void ShowBannerAd()
    {
        Advertisement.Initialize(m_gameID, m_testMode);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerPlacement))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerPlacement);

    }
}
