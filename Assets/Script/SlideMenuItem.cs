using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlideMenuItem : MonoBehaviour {
    public Param m_param;
    public bool m_isUnlock;
    public UILabel m_menuLable;
    public GameObject m_lockUI;

    void Awake()
    {
        m_lockUI = transform.Find("Lock").gameObject;
        m_param = transform.Find("Parameter").GetComponent<Param>();
    }

	// Use this for initialization
	void Start () {
        if (m_isUnlock)
        {
            m_lockUI.SetActive(false);
            GetComponent<UIButton>().enabled = true;
        }
        /*
        if(m_stageParam != null)
        {
            m_menuLable.text = Enum.GetName(typeof(EDifficulty), m_stageParam.Difficulty);
        }
        */

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableClick(bool able)
    {
        GetComponent<UIButton>().enabled = able;
    }

    public void SetUnlock()
    {
        m_isUnlock = true;
        m_lockUI.SetActive(false);
        //GetComponent<UIButton>().enabled = true;
    }

    public void SetLock()
    {
        m_isUnlock = false;
        m_lockUI.SetActive(true);
        //GetComponent<UIButton>().enabled = false;
    }
    
}
