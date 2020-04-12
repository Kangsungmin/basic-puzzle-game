using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState
{
    eGameOver = -1,
    eInitGame = 0,
    eOnGame = 1,
    ePause = 2,
    eShuffle = 3,
    eComplete = 4,
    eBlock = 5 //사용자 입력을 제한한다.
}

public class PuzzleZone : MonoBehaviour {

    //정적 변수
    private static PuzzleZone instance;
    public static PuzzleZone GetInstance()
    {
        if (!instance)
        {
            instance = (PuzzleZone) GameObject.FindObjectOfType(typeof(PuzzleZone));
            if (!instance)
            {
                Debug.LogError("PuzzleZone 게임 오브젝트 찾을 수 없음.");
            }
        }

        return instance;
    }
    public World world;
    
    //멤버 변수
    public float m_shuffleTime = 5.0f;
    public float m_avgSolveTime = 0.0f;
    public bool m_bRotatable = false;
    private int m_resultScore = 0;
    private float m_useTime = 0.0f;
    public int m_moveCount = 0;
    private int m_rewardMoney = 1; //게임 종료 후 획득하는 재화
    private int m_rewardMultiplyValue = 1;
    public EGameState m_gameState;
    public Camera m_mainCamera;
    public GameObject m_nguiCamera;
    public GameObject m_confetti;

    public ST_PuzzleDisplay m_puzzleDisplay;
    // Use this for initialization

    //UI
    public UISlider m_uISlider;
    public GameObject m_previewImage;
    public GameObject m_previewCostView;
    public GameObject m_playBtn;
    public UILabel m_moveCountText;
    public UILabel m_moneyText;
    public UILabel m_crystalText;
    public GameObject EndGamePopScreen;
    public UILabel m_ScoreView;
    public UILabel m_usedTimeView;
    public UILabel m_movedCountView;
    public UILabel m_bestScoreView;
    public UILabel m_bestMoveCountView;
    public UILabel m_getMoneyAmountView; // 획득 재화량
    public GameObject m_rewardVideoButton;
    public UILabel m_rewardMultiplyValueView; // 광고 곱하기 V

    //Sound
    private AudioSource m_audioSource;
    public AudioClip m_sfxPuzzleMove;
    public AudioClip m_sfxPuzzleSpin;
    public AudioClip m_sfxClear;

    public EGameState GetState() { return m_gameState; }
    public void SetState(EGameState state) {
        Debug.Log(state);
        m_gameState = state;
    }


    public void IncreaseMoveCount() {
        m_moveCount += 1;
        GameUIUpdate();
    }

    void Awake()
    {
        world = (World)FindObjectOfType(typeof(World));
        m_gameState = EGameState.eInitGame;
        m_audioSource = GetComponent<AudioSource>();
        m_previewImage.SetActive(false);
        m_previewCostView.SetActive(false);
        if (world.GetSoundAble() == false) m_audioSource.enabled = false;
    }
    
    void Start ()
    {
        m_shuffleTime = world.GetShuffleTime();
        m_avgSolveTime = world.GetAvgTime();
        m_bRotatable = world.GetRotatable();
        m_moveCountText.text = m_moveCount + "";
        m_moneyText.text = World.GetUserSingleTone().m_heartCount + "";
        m_crystalText.text = World.GetUserSingleTone().m_crystalCount + "";
        StartCoroutine(TickTok());

        //========Play Background music=========
        world.PlayBackgroundMusic(world.GetTheme());
        //========Set Background color=========
        m_mainCamera.backgroundColor = world.GetFocuedColor();
    }

    // Update is called once per frame
    void Update() {
        switch (m_gameState)
        {
            case EGameState.eGameOver:
                if (m_playBtn.activeSelf)
                {
                    m_playBtn.SetActive(false);
                }
                break;
            case EGameState.eInitGame:
                break;
            case EGameState.eOnGame:
                if(m_previewImage.activeSelf == false)
                {
                    m_previewImage.SetActive(true);
                    m_previewCostView.SetActive(true);
                }
                break;
            case EGameState.ePause:
                break;
            
            case EGameState.eComplete:
                if(EndGamePopScreen.activeSelf == false)
                {
                    m_gameState = EGameState.eInitGame;
                    Invoke("ProcessGameResult", 1.0f);
                }
                break;
            case EGameState.eBlock:
                break;


        }
	}

    public void Play()
    {
        //게임의 첫 시작이면 퍼즐을 섞는다.
        if(m_gameState == EGameState.eInitGame)
        {
            m_puzzleDisplay.ShufflePuzzle();
            StartCoroutine(Shuffle());
        }
        else if(m_gameState == EGameState.eShuffle)
        {
            return;
        }
        else
        {
            m_gameState = EGameState.eOnGame;
            //퍼즐 선택을 다시 활성화한다.
            m_playBtn.SetActive(false);
            
        }
    }

    IEnumerator Shuffle()
    {
        // 퍼즐을 섞는 동안 퍼즐 이동 금지.
        m_gameState = EGameState.eShuffle;
        //퍼즐 선택을 다시 활성화한다.
        m_playBtn.SetActive(false);
        
        yield return new WaitForSeconds(m_shuffleTime); // 퍼즐을 섞는 상수시간 만큼 대기한다.

        m_gameState = EGameState.eOnGame;
    }

    IEnumerator TickTok() {
        while(true)
        {
            if(m_gameState == EGameState.eOnGame)
            {
                m_useTime += 1.0f;
                Param_Game_Result result = new Param_Game_Result();
                result.puzzleCount = world.GetWidth() * world.GetHeight();
                result.useTime = m_useTime;
                result.moveCount = m_moveCount;
                result.avgSolveTime = m_avgSolveTime;
                result.lastScore = m_resultScore;
                m_resultScore = world.UpdateScore(result);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void GameUIUpdate()
    {
        m_moveCountText.text = m_moveCount + "";
        m_moneyText.text = World.GetUserSingleTone().m_heartCount + "";
    }

    public void OnClickBack()
    {
        world.StopBackgroundMusic();
        SceneManager.LoadScene("Stage");
    }

    public void UpdatePlayerData()
    {
        World.GetUserSingleTone().m_heartCount += m_rewardMoney;
        World.GetUserSingleTone().m_moveCount += m_moveCount;
        World.GetUserSingleTone().m_clearCount += 1;
        World.GetUserSingleTone().m_playTime += (long) m_useTime;
        // 최고 점수 갱신
        if(World.GetUserSingleTone().m_edtStage.ContainsKey(world.GetStageName()) == true)
        {
            int bestScore = World.GetUserSingleTone().m_edtStage[world.GetStageName()].m_clearBestScore;
            World.GetUserSingleTone().m_edtStage[world.GetStageName()].m_clearBestScore = bestScore > m_resultScore ? bestScore : m_resultScore;
        }
    }

    public void SavePlayerData()
    {
        world.PrefSave();
    }

    public void AfterRewardUIUpdate()
    {
        m_rewardMoney = m_rewardMoney * m_rewardMultiplyValue;
        EndGamePopScreenUpdate();
    }

    public void EndGamePopScreenUpdate()
    {
        
        m_ScoreView.text = m_resultScore + "";
        TimeSpan t = TimeSpan.FromSeconds(m_useTime);
        string str = t.ToString(@"mm\:ss");
        m_rewardMultiplyValue =  UnityEngine.Random.Range(1,4);

        m_usedTimeView.text = "Time " + str;//minutes + ":" + seconds;
        m_movedCountView.text = m_moveCount + " Move";
        m_getMoneyAmountView.text = m_rewardMoney + "";
        if(m_rewardMultiplyValue == 1)
        {
            m_rewardVideoButton.SetActive(false);
        }
        m_rewardMultiplyValueView.text = "X" + m_rewardMultiplyValue;
        if (World.GetUserSingleTone().m_edtStage.ContainsKey(world.GetStageName()) == true)
        {
            m_bestScoreView.text = "Best score: " + World.GetUserSingleTone().m_edtStage[world.GetStageName()].m_clearBestScore;
        }
    }

    public void ShowRewardVideo()
    {
        m_rewardVideoButton.SetActive(false);
        world.GetComponent<UnityAdsManager>().ShowRewardedAD_Heart(m_rewardMoney * (m_rewardMultiplyValue-1));
    }

    public void ProcessGameResult()
    {
        //게임 결과를 world에 반영한다.
        UpdatePlayerData();
        
        GenConfetti();
        EndGamePopScreenUpdate();
        EndGamePopScreen.SetActive(true);
        AudioPlayClear();
        SavePlayerData();
    }

    public void AudioPlayPuzzleMove()
    {
        m_audioSource.clip = m_sfxPuzzleMove;
        m_audioSource.Play(0);
    }

    public void AudioPlayPuzzleSpin()
    {
        m_audioSource.clip = m_sfxPuzzleSpin;
        m_audioSource.Play(0);
    }

    public void AudioPlayClear()
    {
        m_audioSource.clip = m_sfxClear;
        m_audioSource.Play(0);
    }

    public void GenConfetti()
    {
        Vector3 randomPosition;
        int screenWidth = (int)(Screen.width * 0.5f);
        int screenHeight = (int)(Screen.height);
        Debug.Log(screenWidth + "/" + screenHeight);
        for(int i = 0; i < 50; i++)
        {
            randomPosition = new Vector3(UnityEngine.Random.Range(-1 * screenWidth, screenWidth), UnityEngine.Random.Range(0, screenHeight), 0);
            GameObject genConfetti = Instantiate(m_confetti, Vector3.zero, Quaternion.identity);
            genConfetti.transform.SetParent(m_nguiCamera.transform);
            genConfetti.transform.localScale = new Vector3(1,1,1);
            genConfetti.transform.localPosition = randomPosition;
        }
    }
}
