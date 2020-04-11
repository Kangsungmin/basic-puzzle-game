using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksmPopUp : MonoBehaviour
{
    public bool m_disable = false;
    public float m_lifeTime = 3.0f;
    private UILabel m_message;
    private UISprite m_msgSprite;
    // Start is called before the first frame update
    void Start()
    {
         GetMessageLabel();
        if(m_disable)
        {
            StartCoroutine("Disable");
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(m_lifeTime);
        gameObject.SetActive(false);
    }

    public UILabel GetMessageLabel()
    {
        if(m_message == null)
        {
            m_message = transform.GetChild(0).Find("Label").GetComponent<UILabel>();
        }
        return m_message;
    }

    public UISprite GetMessageSprite()
    {
        if(m_msgSprite == null)
        {
            m_msgSprite = transform.GetChild(0).GetComponent<UISprite>();
        }
        return m_msgSprite;
    }

    public void SetMessageText(string text)
    {
        GetMessageLabel().text = text;
    }

    public void SetMsgColor(Color c)
    {
        GetMessageSprite().color = c;
    }
}
