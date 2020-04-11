using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeParam : Param
{
    private World world;

    public StageParam[] m_stageList; // 현재 테마의 스테이지 리스트
	// Use this for initialization
    void Awake()
    {
        // 자식들(StageParam)의 객체를 m_stageList에 가져온다.
        int childSize = transform.childCount;
        m_stageList = new StageParam[childSize];
        for(int c = 0; c < childSize; c++)
        {
            m_stageList[c] = transform.GetChild(c).GetComponent<StageParam>(); 
        }
    }

    void Start () {
        world = (World)FindObjectOfType(typeof(World));
    }

    public void ApplyParam()
    {
        world.SetTheme(m_name);
        // 현재 테마의 스테이지 리스트를 world 하위에 복사한다.
        world.RestStageList();
        foreach (StageParam stage in m_stageList)
        {
            GameObject child = Instantiate(stage.gameObject, stage.transform.position, stage.transform.rotation) as GameObject;
            child.transform.SetParent(world.transform);
        }
  
        world.LoadStageList(); 
    }
}
