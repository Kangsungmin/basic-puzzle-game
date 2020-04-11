using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEyeView : MonoBehaviour {
    Transform m_transform;
    UIPanel m_panel;
    UIWidget m_widget;
    public GameObject m_menuObject;
    public float m_cellWidth = 200;
    public float m_downScale = 0.4f;
    public float m_focusRange = 0.0f;

    float m_pos;
    float m_dist;
    // Use this for initialization
    void Awake () {
        // 자동 초기화
        m_transform = transform;
        if(m_transform.parent != null && m_transform.parent.parent != null)
        {
            m_panel = m_transform.parent.parent.GetComponent<UIPanel>();
        }
        m_widget = GetComponent<UIWidget>();
        m_menuObject = GameObject.FindGameObjectWithTag("MENU");
    }

    public void Initialize()
    {
        // 현재 클래스 초기화와 함께 멤버변수 할당한다.
        m_transform = transform;
        if (m_transform.parent != null && m_transform.parent.parent != null)
        {
            m_panel = m_transform.parent.parent.GetComponent<UIPanel>();
        }
        m_widget = GetComponent<UIWidget>();
        m_menuObject = GameObject.FindGameObjectWithTag("MENU");
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void FixedUpdate()
    {
        if(m_pos < m_focusRange && m_pos > (-1 * m_focusRange))
        {
            m_menuObject.SendMessage("SetFocusedMenuItem", GetComponent<SlideMenuItem>()); // 중앙에 위치한 오브젝트를 갱신한다.
        }
        m_pos = m_transform.localPosition.x - m_panel.clipOffset.x;
        m_dist = Mathf.Clamp(Mathf.Abs(m_pos), 0f, m_cellWidth);
        m_widget.width = System.Convert.ToInt32(((m_cellWidth - m_dist * m_downScale)/m_cellWidth)*m_cellWidth);
    }
}
