using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class World : MonoBehaviour {
    //************************************
    // 게임 업데이트시 확인해야 하는 중요 변수들
    //************************************


    //************************************
    public bool RESET = false;

    private static bool m_isPrefLoaded = false;
    private static UserData g_userData;
    private string g_theme = "animal"; // 테마
    private string g_stageName = "animal0";
    public Color32 g_focusedColor;
    private bool g_soundAble = true;
    private int g_stageNumber = 1;
    private EDifficulty g_difficulty;
    private int g_width = 3;
    private int g_height = 2;
    private float g_shuffleTime;
    public int g_shuffleComplexity = 1;
    private float g_avgTime = 0;
    private bool g_rotatable;
    private bool g_reversible;

    private ThemeMenu m_themeMenu;
    private StageMenu m_stageMenu;
    private PuzzleZone m_puzzleZone;

    public StageParam[] m_stageList; // 현재 테마의 스테이지 리스트
    public AudioSource m_backgroundAudioSource;
    public AudioClip[] m_backgroundMusic;

    // 유저 데이터 반환 싱글톤 패턴
    public static UserData GetUserSingleTone()
    {
        if (g_userData == null) g_userData = new UserData();
        
        return g_userData;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) // 이미 다른 world 존재
        {
            Destroy(gameObject);
        }

        // 프리팹 로드: THEME_MAP, STAGE_MAP 갱신(클리어 여부 등등)
        if(m_isPrefLoaded == false)
        {
            PrefLoad();
        }
        //m_backgroundAudioSource = GetComponent<AudioSource>();
    }


    public void PrefSave()
    {
        /*
         * 플레이어 정보를 로컬에 저장하는 함수
         * 현재 클리어한 스테이지(StageClass)의 정보를 키에 대응하여 저장한다.
         */

        
        PlayerPrefs.SetInt("OWN_THEME_COUNT", GetUserSingleTone().m_ownTheme.Count);

        int num = 1;
        foreach(string theme in GetUserSingleTone().m_ownTheme)
        {
            PlayerPrefs.SetString("OWN_THEME" + num, theme);
            num++;
        }

        PlayerPrefs.SetInt("MODIFIED_STAGE_COUNT", GetUserSingleTone().m_edtStage.Count);
        num = 1;
        foreach (KeyValuePair<string,Stage> item in GetUserSingleTone().m_edtStage)
        {
            string stageName = item.Key;
            PlayerPrefs.SetString("MODIFIED_STAGE" + num, stageName);
            PlayerPrefs.SetInt("STAGE[" + stageName + "]." + "IS_CLEAR", item.Value.m_isClear ? 1 : 0);
            PlayerPrefs.SetFloat("STAGE[" + stageName + "]." + "CLEAR_MOVE_REC", item.Value.m_clearMoveRecord);
            PlayerPrefs.SetFloat("STAGE[" + stageName + "]." + "CLEAR_USED_TIME", item.Value.m_clearUsedTime);
            PlayerPrefs.SetInt("STAGE[" + stageName + "]." + "CLEAR_BEST_SCORE", item.Value.m_clearBestScore);
            num++;
        }

        //보유 아이템 및 재화 저장
        PlayerPrefs.SetInt("HEART_COUNT", GetUserSingleTone().m_heartCount);
        PlayerPrefs.SetInt("CRYSTAL_COUNT", GetUserSingleTone().m_crystalCount);
        PlayerPrefs.SetInt("TOTAL_CLEAR_COUNT", GetUserSingleTone().m_clearCount);
        PlayerPrefs.SetString("TOTAL_PLAY_TIME", GetUserSingleTone().m_playTime.ToString());
        PlayerPrefs.SetString("TOTAL_MOVE_COUNT", GetUserSingleTone().m_moveCount.ToString());
    }

    public void PrefLoad()
    {
        /*
         * 플레이어의 정보를 로컬에서 로드하는 함수
         */
         if(RESET) PlayerPrefs.DeleteAll();

        GetUserSingleTone().ResetAll(); // 초기화


        SetSoundAble(PlayerPrefs.GetInt("SOUND", 1) == 1);
        int ownThemeCount = PlayerPrefs.GetInt("OWN_THEME_COUNT", 0); // 보유한 테마 개수
        for (int num = 1; num <= ownThemeCount; num++)
        {
            //Prefeb에서 읽어들인 data를 Stage class로 변환한다.
            string data = PlayerPrefs.GetString("OWN_THEME" + num);
            GetUserSingleTone().m_ownTheme.Add(data); // 보유한 테마에 추가
        }

        int modifiedStageCount = PlayerPrefs.GetInt("MODIFIED_STAGE_COUNT", 0); // 사용자가 수정한(1회 이상 접근한) 스테이지 개수
        for (int num = 1; num <= modifiedStageCount; num++)
        {
            string stageName = PlayerPrefs.GetString("MODIFIED_STAGE" + num, "");
            bool stageClear = PlayerPrefs.GetInt("STAGE[" + stageName +"]." + "IS_CLEAR") == 1 ? true : false;
            float stageClearMoveRec = PlayerPrefs.GetFloat("STAGE[" + stageName + "]." + "CLEAR_MOVE_REC", Mathf.Infinity);
            float stageClearUsedTime = PlayerPrefs.GetFloat("STAGE[" + stageName + "]." + "CLEAR_USED_TIME", 0);
            int stageClearBestScore = PlayerPrefs.GetInt("STAGE[" + stageName + "]." + "CLEAR_BEST_SCORE", 0);
            Stage stage = new Stage(stageName, stageClear, stageClearMoveRec, stageClearUsedTime);
            stage.m_clearBestScore = stageClearBestScore;
            GetUserSingleTone().m_edtStage.Add(stageName, stage);
        }

        // 보유 아이템 및 재화 로드
        int heartCount = PlayerPrefs.GetInt("HEART_COUNT", 15);
        int crystalCount = PlayerPrefs.GetInt("CRYSTAL_COUNT", 15);
        int clearCount = PlayerPrefs.GetInt("TOTAL_CLEAR_COUNT", 0);
        long playTime = long.Parse(PlayerPrefs.GetString("TOTAL_PLAY_TIME", "0"));
        long moveCount = long.Parse(PlayerPrefs.GetString("TOTAL_MOVE_COUNT", "0"));
        
        GetUserSingleTone().m_heartCount = heartCount;
        GetUserSingleTone().m_crystalCount = crystalCount;
        GetUserSingleTone().m_playTime = playTime;
        GetUserSingleTone().m_clearCount = clearCount;

        m_isPrefLoaded = true;
    }

    public Stage StringToStage(string data)
    {
        Stage stage = new Stage();
        return stage;
    }

    public bool GetSoundAble()
    {
        return g_soundAble;
    }

    public void SetSoundAble(bool able)
    {
        g_soundAble = able;
        PlayerPrefs.SetInt("SOUND", g_soundAble == true ? 1 : 0);
        if(m_backgroundAudioSource != null) m_backgroundAudioSource.enabled = able;
    }

    public string GetTheme()
    {
        return g_theme;
    }

    public void SetTheme(string theme)
    {
        g_theme = theme;
    }

    public int GetStageNum()
    {
        return g_stageNumber;
    }

    public void SetStageNum(int number)
    {
        this.g_stageNumber = number;
    }

    public string GetStageName()
    {
        return g_stageName;
    }

    public void SetStageName(string name)
    {
        this.g_stageName = name;
    }


    public void SetWidth(int width)
    {
        g_width = width;
    }

    public int GetWidth()
    {
        return g_width;
    }

    public void SetHeight(int height)
    {
        g_height = height;
    }

    public int GetHeight()
    {
        return g_height;
    }

    public void SetShuffleTime(float time)
    {
        g_shuffleTime = time;
    }

    public float GetShuffleTime()
    {
        return g_shuffleTime;
    }

    public void SetShuffleComplexity(int complexity)
    {
        g_shuffleComplexity = complexity;
    }

    public int GetShuffleComplexity()
    {
        return g_shuffleComplexity;
    }

    public void SetAvgTime(float avgTime)
    {
        g_avgTime = avgTime;
    }

    public float GetAvgTime()
    {
        return g_avgTime;
    }

    public bool GetRotatable()
    {
        return g_rotatable;
    }

    public void SetFocuedColor(Color32 color)
    {
        g_focusedColor = color;
    }

    public Color32 GetFocuedColor()
    {
        return g_focusedColor;
    }

    public void SetRotatable(bool rotatable)
    {
        g_rotatable = rotatable;
    }

    public void SetReversible(bool reversible)
    {
        g_reversible = reversible;
    }

    public EDifficulty GetDifficulty()
    {
        return g_difficulty;
    }

    public void SetDifficulty(EDifficulty difficulty)
    {
        g_difficulty = difficulty;
    }

    public void RestStageList()
    {
        int childSize = transform.childCount;
        for (int c = 0; c < childSize; c++)
        {
            Destroy(transform.GetChild(c).gameObject);
        }
    }

    public void LoadStageList()
    {
        int childSize = transform.childCount;
        m_stageList = new StageParam[childSize];
        for (int c = 0; c < childSize; c++)
        {
            m_stageList[c] = transform.GetChild(c).GetComponent<StageParam>();
        }
    }

    public void SelectStageInList(int pos)
    {
        if(m_stageList.Length > pos)
        {
            g_stageNumber = m_stageList[pos].StageNumber;
            g_difficulty = m_stageList[pos].Difficulty;
            g_rotatable = m_stageList[pos].Rotatable;
            g_reversible = m_stageList[pos].Reversible;
        }
    }

    public bool BuyTheme(string name, Param.CostType cType, int cost) // 테마 구매 프로세스
    {
        bool buy = false;
        if(GetUserSingleTone().m_ownTheme.Contains(name) == false)// 보유하지 않은 테마라면
        {
            if(cType == Param.CostType.crystal)
            {
                if (GetUserSingleTone().m_crystalCount >= cost)
                {
                    GetUserSingleTone().m_crystalCount -= cost;
                    GetThemeMenu().UpdateLeftCrystal(GetUserSingleTone().m_crystalCount); // UI Update
                    buy = true;
                }
                else
                {
                    //잔고 부족 충전제안.
                    Debug.Log("크리스탈 부족");
                }
            }
            else if(cType == Param.CostType.heart)
            {
                if(GetUserSingleTone().m_heartCount >= cost)
                {
                    GetUserSingleTone().m_heartCount -= cost;
                    GetThemeMenu().UpdateLeftHeart(GetUserSingleTone().m_heartCount); // UI Update
                    buy = true;
                }
                else
                {
                    //잔고 부족 충전제안.
                    Debug.Log("재화 부족");
                }
            }
            
        }

        if(buy)
        {
            GetUserSingleTone().m_ownTheme.Add(name); // 테마 추가
            GetThemeMenu().UpdateFocusedMenuItem();
            PrefSave();
        }
        return buy;
    }

    public bool BuyStage(string name, Param.CostType cType, int cost) // 스테이지 구매 프로세스
    {
        bool buy = false;
        if(GetUserSingleTone().m_edtStage.ContainsKey(name) == false)
        {
            if (cType == Param.CostType.crystal)
            {
                if (GetUserSingleTone().m_crystalCount >= cost)
                {
                    GetUserSingleTone().m_crystalCount -= cost;
                    GetStageMenu().UpdateLeftCrystal(GetUserSingleTone().m_crystalCount); // UI Update
                    buy = true;
                }
            }
            else if (cType == Param.CostType.heart)
            {
                if (GetUserSingleTone().m_heartCount >= cost)
                {
                    GetUserSingleTone().m_heartCount -= cost;
                    GetStageMenu().UpdateLeftHeart(GetUserSingleTone().m_heartCount); // UI Update
                    buy = true;
                }
                else
                {
                    //잔고 부족 충전제안.
                    Debug.Log("하트 부족");
                }
            }
        }
        // 위 로직 공통으로 묶기
        if(buy)
        {
            GetUserSingleTone().m_edtStage.Add(name,new Stage(name,false,0,0)); // 스테이  추가
            GetStageMenu().UpdateFocusedMenuItem();
            PrefSave();
        }
        return buy;
    }

    public ThemeMenu GetThemeMenu()
    {
        if(m_themeMenu == null)
        {
            m_themeMenu = (ThemeMenu)FindObjectOfType(typeof(ThemeMenu));
        }
        return m_themeMenu;
    }

    public StageMenu GetStageMenu()
    {
        if (m_stageMenu == null)
        {
            m_stageMenu = (StageMenu)FindObjectOfType(typeof(StageMenu));
        }
        return m_stageMenu;
    }

    public PuzzleZone GetPuzzleZone()
    {
        if (m_puzzleZone == null)
        {
            m_puzzleZone = (PuzzleZone)FindObjectOfType(typeof(PuzzleZone));
        }
        return m_puzzleZone;
    }

    public void UpdateUI()
    {
        if(GetThemeMenu() != null)
        {
            GetThemeMenu().UpdateLeftHeart(GetUserSingleTone().m_heartCount); // Heart-view Update
            GetThemeMenu().UpdateLeftCrystal(GetUserSingleTone().m_crystalCount); // Heart-view Update
        }
        if(GetStageMenu() != null)
        {
            GetStageMenu().UpdateLeftHeart(GetUserSingleTone().m_heartCount); // Heart-view Update
            GetStageMenu().UpdateLeftCrystal(GetUserSingleTone().m_crystalCount); // Heart-view Update
        }
        if(GetPuzzleZone() != null)
        {
            GetPuzzleZone().AfterRewardUIUpdate();
        }
    }

    public void AddHeart(int amount)
    {
        GetUserSingleTone().m_heartCount += amount;
        PrefSave();
        UpdateUI();
    }

    public bool DiscountHeart(int amount)
    {
        if (GetUserSingleTone().m_heartCount >= amount)
        {
            GetUserSingleTone().m_heartCount -= amount;
            return true;
        }
        else return false;
    }

    public void AddCrystal(int amount)
    {
        GetUserSingleTone().m_crystalCount += amount;
        PrefSave();
        UpdateUI();
    }

    public bool DiscountCrystal(int amount)
    {
        if (GetUserSingleTone().m_crystalCount >= amount)
        {
            GetUserSingleTone().m_crystalCount -= amount;
            return true;
        }
        else return false;
    }

    public void OnPurchaseFailure(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of product" + product.definition.id + " fail due to " + reason);
    }

    public int GameScoreResult(Param_Game_Result p)
    {
        // 게임 결과를 zone에서 전달받아 현재 테마, 스테이지 정보를 반영하여 결과 스코어를 계산하여 결과를 전달한다.
        /*
         * 기준 시간 초과시 초당 2점 감점
         * 1초 당겨질 시 +2
         */
        int deltaSec = (int)(p.avgSolveTime - p.useTime);   //변동 점수
        int interestScore = p.puzzleCount * 10;             //기준 점수
        int minimumScore = p.puzzleCount;                   //기본 점수
        int score = interestScore - (deltaSec * 2);
        return score > minimumScore ? score: minimumScore;
    }

    public void PlayBackgroundMusic(string themeName)
    {
        for(int i=0; i < m_backgroundMusic.Length; i++)
        {
            if(m_backgroundAudioSource.clip != null && m_backgroundAudioSource.clip.name == "common" && themeName == "common") // 기본 배경 이미 재생중
            {
                return;
            }
            else if (m_backgroundMusic[i].name == themeName)
            {
                m_backgroundAudioSource.clip = m_backgroundMusic[i];
                m_backgroundAudioSource.Play();
                return;
            }
        }
        //m_backgroundAudioSource.clip = m_backgroundMusic[m_backgroundMusic.Length];
        //m_backgroundAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        m_backgroundAudioSource.Stop();
    }
}

