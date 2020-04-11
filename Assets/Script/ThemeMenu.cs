using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class ThemeMenu : MonoBehaviour {
    SlideMenuItem m_focusedMenuItem;

    public GameObject m_popUpBuy;
    public GameObject m_popUpMenu;
    public GameObject m_uiCostImage;
    public UILabel m_focusedCost;
    public UILabel m_leftMoneyView;
    public UILabel m_leftCrystalView;
    public UILabel m_themeLabel;
    public GameObject m_messagePopUp;
    public GameObject m_shopMenu;
    public GameObject m_soundDisalbeLable;
    public Camera m_mainCamera;

    public UIButton m_normalRewardButton;
    public UILabel m_normalDisableLabel;
    public UIButton m_crystalRewardButton;
    public UILabel m_crystalDisableLabel;



    private World world;
    private UnityAdsManager adManager;
    private Color32 m_bacgroundColor;

    //Sound
    private AudioSource m_audioSource;
    public AudioClip m_sfxClick_1;

    void Awake()
    {
        world = (World)FindObjectOfType(typeof(World));
        adManager = (UnityAdsManager)FindObjectOfType(typeof(UnityAdsManager));
        //연결된 다른 게임오브젝트에 world를 할당해준다.
        m_audioSource = GetComponent<AudioSource>();
    }
	// Use this for initialization
	void Start ()
    {
        // world 의 param을 초기화한다.
        world.RestStageList();
        // world 에게 UI 갱신을 요청한다.
        world.UpdateUI();

        if (world.GetSoundAble() == true) m_soundDisalbeLable.SetActive(false);
        else m_soundDisalbeLable.SetActive(true);

        world.PlayBackgroundMusic("common");

        adManager.m_normalDisableLabel = m_normalDisableLabel;
        adManager.m_crystalDisableLabel = m_crystalDisableLabel;
        adManager.m_normalRewardButton = m_normalRewardButton;
        adManager.m_crystalRewardButton = m_crystalRewardButton;
        adManager.InitRewardButton();
    }

    public void OnMenuClick(ThemeParam themeParam)
    {
        if (World.GetUserSingleTone().m_ownTheme.Contains(themeParam.m_name) == false)
        {
            m_popUpBuy.SetActive(true);
        }
        else
        {
            themeParam.ApplyParam();
            SceneManager.LoadScene("Stage");
        }
    }

    public void SetFocusedMenuItem(SlideMenuItem item)
    {
        if (m_focusedMenuItem != item)
        {
            PlayButtonClick1();
            if (m_focusedMenuItem != null) m_focusedMenuItem.EnableClick(false);

            m_focusedMenuItem = item;
            m_focusedMenuItem.EnableClick(true);
            // 보유한 테마인지 확인
            UpdateFocusedMenuItem();
        }
    }

    public void UpdateFocusedMenuItem()
    {
        string themeName = m_focusedMenuItem.m_param.m_name;
        m_themeLabel.text = "#"+themeName;
        m_bacgroundColor = m_focusedMenuItem.m_param.m_tintColor;
        m_mainCamera.backgroundColor = m_bacgroundColor;
        world.SetFocuedColor(m_bacgroundColor);
        //Debug.Log(m_bacgroundColor);
        if (World.GetUserSingleTone().m_ownTheme.Contains(themeName) == false)
        {
            m_focusedMenuItem.SetLock(); // 비활성화
            m_focusedCost.text = m_focusedMenuItem.m_param.m_price + "";
            m_uiCostImage.SetActive(true);
        }
        else
        {
            m_focusedMenuItem.SetUnlock();
            m_uiCostImage.SetActive(false);
        }
    }

    public void OnPopUpMenu()
    {
        if(m_popUpMenu.activeSelf == false)
        {
            m_popUpMenu.SetActive(true);
        }
        else
        {
            m_popUpMenu.SetActive(false);
        }
    }

    public void PopUpMessage(string msg, Color color)
    {
        // world pop-up message
        if (FindObjectsOfType<ksmPopUp>().Length == 0)
        {
            GameObject pop = Instantiate(m_messagePopUp, new Vector3(0, 0, 0), Quaternion.identity);
            pop.GetComponent<ksmPopUp>().SetMessageText(msg);
            pop.GetComponent<ksmPopUp>().SetMsgColor(color);
        }
    }

    public void OnBuyYes()
    {
        PlayButtonClick1();
        //구매 확정.
        //구매 프로세스 함수 호출.
        bool isSuccess = world.BuyTheme(m_focusedMenuItem.m_param.m_name, m_focusedMenuItem.m_param.m_costType, m_focusedMenuItem.m_param.m_price);
        m_popUpBuy.SetActive(false);
        if (isSuccess)
        {
            PopUpMessage("PURCHASE COMPLETE !", Color.green);
        }
        else
        {
            PopUpMessage("PURCHASE FAIL !", Color.red);
        }

    }

    public void OnBuyNo()
    {
        PlayButtonClick1();
        m_popUpBuy.SetActive(false);
    }

    public void UpdateLeftHeart(int count)
    {
        if(m_leftMoneyView != null)
        {
            m_leftMoneyView.text = count + "";
        }
    }

    public void UpdateLeftCrystal(int count)
    {
        if(m_leftCrystalView != null)
        {
            m_leftCrystalView.text = count + "";
        }
    }

    public void OnPopUpShop()
    {
        m_popUpMenu.SetActive(false);
        if(m_shopMenu.activeSelf == false)
        {
            m_shopMenu.SetActive(true);
        }
        else
        {
            m_shopMenu.SetActive(false);
        }
    }

    public void AddHeart(int amount)
    {
        world.AddHeart(amount);
    }

    public void AddCrystal(int amount)
    {
        world.AddCrystal(amount);
    }

    public void ShowRewardedAD_Heart(int amount)
    {
        m_popUpMenu.SetActive(false);
        adManager.ShowRewardedAD_Heart(amount);
    }

    public void ShowRewardedAD_Crystal(int amount)
    {
        m_popUpMenu.SetActive(false);
        adManager.ShowRewardedAD_Crystal(amount);
    }

    public void RemoveAD()
    {

    }

    public void OnPurchaseFailure(Product product, PurchaseFailureReason reason)
    {
        world.OnPurchaseFailure(product, reason);
    }

    public void OnSoundToggle()
    {
        if(world.GetSoundAble())
        {
            world.SetSoundAble(false);
            m_soundDisalbeLable.SetActive(true);
        }
        else
        {
            world.SetSoundAble(true);
            m_soundDisalbeLable.SetActive(false);
        }
    }

    public void PlayButtonClick1()
    {
        if(world.GetSoundAble() == true)
        {
            m_audioSource.clip = m_sfxClick_1;
            m_audioSource.Play(0);
        }

    }
}
