using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * world 의 stageList 항목을 읽어서 메뉴를 생성한다.
 */
public class StageMenu : MonoBehaviour {
    SlideMenuItem m_focusedMenuItem;
    public GameObject m_stageMenuPref;
    public GameObject m_grid; 
    private World world;
    public UILabel m_focusedBestScore;
    public Transform m_difficultyGrid;
    public GameObject[] m_difficultyIcons;
    public GameObject m_uiCostImage;
    public UILabel m_focusedCost;
    public UILabel m_leftMoneyView;
    public UILabel m_leftCrystalView;
    public Camera m_mainCamera;

    //POP UP
    public GameObject m_yesnoPopUp;
    public GameObject m_messagePopUp;

    //Sound
    private AudioSource m_audioSource;
    public AudioClip m_sfxClick_1;

    void Awake()
    {
        world = (World)FindObjectOfType(typeof(World));
        m_difficultyIcons = new GameObject[m_difficultyGrid.childCount];
        for(int i=0; i < m_difficultyGrid.childCount; i++)
        {
            m_difficultyIcons[i] = m_difficultyGrid.GetChild(i).gameObject;
        }

        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.enabled = world.GetSoundAble();

    }
	// Use this for initialization
	void Start () {
        // TODO: world의 stageList를 읽어서 프리팹 생성
        if(world != null)
        {
            int index = 1;
            foreach (StageParam stage in world.m_stageList)
            {
                //메뉴 프리팹 생성
                GameObject menu = Instantiate(m_stageMenuPref, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                GameObject menu_p = menu.transform.Find("Parameter").gameObject;
                menu_p.GetComponent<StageParam>().CpyParamData(stage);
                UILabel lable = menu.transform.Find("StageNumView").GetComponent<UILabel>();
                lable.text = index+""; //stage number
                menu.transform.SetParent(m_grid.transform, false);
                UIButton menuButton = menu.GetComponent<UIButton>();
                //버튼 이벤트 할당
                EventDelegate eventClick = new EventDelegate(this, "OnMenuClick"); // 오브젝트와 함수명
                eventClick.parameters[0] = MakeParameter(menu_p.GetComponent<StageParam>(), typeof(StageParam));
                EventDelegate.Add(menuButton.onClick, eventClick);
                // 생성한 프리팹 초기화
                menu.SendMessage("Initialize");
                index++;
            }
            m_uiCostImage.SetActive(false);
            m_mainCamera.backgroundColor = world.GetFocuedColor();
            world.PlayBackgroundMusic("common");

            // world 에게 UI 갱신을 요청한다.
            world.UpdateUI();
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
        string stageName = m_focusedMenuItem.m_param.m_name;
        //난이도 출력
        StageParam p = m_focusedMenuItem.transform.Find("Parameter").GetComponent<StageParam>();
        HideAllDifIcons();
        ActiveDifIcons((int)p.Difficulty);
        if (World.GetUserSingleTone().m_edtStage.ContainsKey(stageName) == false)
        {
            m_focusedMenuItem.SetLock(); // 비활성화
            m_focusedBestScore.gameObject.SetActive(false); // 최고점수 비활성화
            m_focusedCost.text = m_focusedMenuItem.m_param.m_price + "";
            m_uiCostImage.SetActive(true);
        }
        else
        {
            m_focusedMenuItem.SetUnlock();
            if(World.GetUserSingleTone().m_edtStage[stageName].m_clearBestScore > 0)
            {
                m_focusedBestScore.gameObject.SetActive(true); // 최고점수 활성
                
                m_focusedBestScore.text = "BEST: " + World.GetUserSingleTone().m_edtStage[stageName].m_clearBestScore;
            }
            else
            {
                m_focusedBestScore.gameObject.SetActive(false); // 최고점수 활성
            }

            m_uiCostImage.SetActive(false);
        }
    }

    public void OnBackClick()
    {
        SceneManager.LoadScene("Theme");
    }

    public void OnMenuClick(StageParam stageParam)
    {
        string stageName = m_focusedMenuItem.m_param.m_name;
        if (World.GetUserSingleTone().m_edtStage.ContainsKey(stageName) == false) //보유하지 않은 스테이지
        {
            m_yesnoPopUp.SetActive(true);
        }
        else
        {
            stageParam.ApplyParam();
            SceneManager.LoadScene("Zone");
        }

    }

    // 이벤트 parameter를 생성하여 리턴.   
    private EventDelegate.Parameter MakeParameter(Object _value, System.Type _type)
    {
        EventDelegate.Parameter param = new EventDelegate.Parameter();
        // 이벤트 parameter 생성.     
        param.obj = _value;
        // 이벤트 함수에 전달하고 싶은 값.     
        param.expectedType = _type;
        // 값의 타입.       
        return param;
    }

    public void PopUpMessage(string msg, Color color)
    {
        // world pop-up message
        if(FindObjectsOfType<ksmPopUp>().Length == 0)
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
        bool isSuccess = world.BuyStage(m_focusedMenuItem.m_param.m_name, m_focusedMenuItem.m_param.m_costType, m_focusedMenuItem.m_param.m_price);
        m_yesnoPopUp.SetActive(false);
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
        m_yesnoPopUp.SetActive(false);
    }

    public void UpdateLeftHeart(int count)
    {
        if (m_leftMoneyView != null)
        {
            m_leftMoneyView.text = count + "";
        }
    }

    public void UpdateLeftCrystal(int count)
    {
        if (m_leftCrystalView != null)
        {
            m_leftCrystalView.text = count + "";
        }
    }

    public void HideAllDifIcons()
    {
        foreach(GameObject icon in m_difficultyIcons)
        {
            icon.SetActive(false);
        }
    }

    public void ActiveDifIcons(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            m_difficultyIcons[i].SetActive(true);
        }
    }

    public void PlayButtonClick1()
    {
        m_audioSource.clip = m_sfxClick_1;
        m_audioSource.Play(0);
    }
}
