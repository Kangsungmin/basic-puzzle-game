using UnityEngine;

// 저장 로드용 스테이지 클래스
// 스테이지 관련 유지되어야 할 데이터를 포함한다.
public class Stage {
    public string m_name;
    public bool m_isClear;        // 클리어 여부
    public float m_clearMoveRecord; // 클리어를 위해 이동한 퍼즐(최소)
    public float m_clearUsedTime; // 클리어를 위해 사용한 가장 짧은 시간(최소)
    public int m_clearBestScore = 0;

    public Stage()
    {
        m_isClear = false;
        m_clearMoveRecord = Mathf.Infinity;
        m_clearUsedTime = Mathf.Infinity;

    }
    public Stage(string name, bool clear, float moveRec, float usedTime)
    {
        m_name = name;
        m_isClear = clear;
        m_clearMoveRecord = moveRec;
        m_clearUsedTime = usedTime;
    }
}
