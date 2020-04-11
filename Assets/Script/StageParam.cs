using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELockCondition
{
    NONE,
    PLAYCOUNT, // 플레이 수
    PUZZLEMOVECOUNT, // 퍼즐 총 이동 수
    THEMECLEAR, // 특정 테마 클리어
    TOTALPLAYTIME // 총 플레이시간
}

public enum EDifficulty
{
    NONE = 0,
    EASY = 1,
    NORMAL = 2,
    HARD = 3,
    SUPER_HARD = 4,
    LEGENDARY = 5
}

public class StageParam : Param
{
    public int StageNumber;
    public EDifficulty Difficulty;      // 난이도
    public int Width;
    public int Height;
    public int ShuffleComplexity = 20;       // 섞는 복잡도
    public float ShuffleTime;
    public float AvgTime;       // 해당 스테이지 평균 완료 시간(초)
    public bool Rotatable;      // 퍼즐 회전 가능 여부
    public bool Reversible;     // 퍼즐 반전 가능 여부

    public ELockCondition LockCondition;
    public int param1;
    public int param2;

    private World world;
    public void Start()
    {
        world = (World)FindObjectOfType(typeof(World));
    }

    public void CpyParamData(StageParam rhs)
    {
        this.m_name = rhs.m_name;
        this.StageNumber = rhs.StageNumber;
        //this.StageName = rhs.StageName; name exist
        this.m_price = rhs.m_price;
        this.Difficulty = rhs.Difficulty;
        this.Width = rhs.Width;
        this.Height = rhs.Height;
        this.AvgTime = rhs.AvgTime;
        this.Rotatable = rhs.Rotatable;
        this.Reversible = rhs.Reversible;
        this.ShuffleComplexity = rhs.ShuffleComplexity;
        this.ShuffleTime = rhs.ShuffleTime;
    }

    public void ApplyParam()
    {
        // 파라미터를 World에 적용한다.
        world.SetStageNum(StageNumber);
        world.SetStageName(m_name);
        world.SetWidth(Width);
        world.SetHeight(Height);
        world.SetAvgTime(AvgTime);
        world.SetRotatable(Rotatable);
        world.SetReversible(Reversible);
        world.SetShuffleComplexity(ShuffleComplexity);
        world.SetShuffleTime(ShuffleTime);
    }
}
