using System.Collections.Generic;
/*
 * 유저 업적등 게임 활동 클래스
 */
public class UserData {
    public int m_heartCount;    // 하트 수
    public int m_crystalCount;  // 유료 크리스탈 수
    public HashSet<string> m_ownTheme; //구매한 테마
    public Dictionary<string, Stage> m_edtStage;
    public long m_playTime; // 총 플레이시간
    public int m_clearCount; // 클리어 횟수
    public long m_moveCount; // 퍼즐 이동 횟수

    public UserData()
    {
        m_heartCount = 0;
        m_crystalCount = 0;
        m_ownTheme = new HashSet<string>();
        m_edtStage = new Dictionary<string, Stage>();
        m_playTime = 0;
        m_clearCount = 0;
        m_moveCount = 0;
    }

    public void ResetAll()
    {
        m_heartCount = 0;
        m_crystalCount = 0;
        m_ownTheme = new HashSet<string>();
        m_edtStage = new Dictionary<string, Stage>();
        m_playTime = 0;
        m_clearCount = 0;
        m_moveCount = 0;
    }
}
