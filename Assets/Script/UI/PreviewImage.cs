using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewImage : MonoBehaviour
{
    private PuzzleZone zone;
    private bool isPop = false;
    private Animator m_animator;
    private GameObject Background;
    
    // Start is called before the first frame update
    void Awake()
    {
        zone = FindObjectOfType<PuzzleZone>();
        m_animator = GetComponent<Animator>();
        Background = transform.Find("BlurBG").gameObject;
        m_animator.enabled = false;
    }


    public void PopTogle()
    {
        if(isPop)
        {
            zone.SetState(EGameState.eOnGame);
            isPop = false;
            Background.SetActive(false);
            m_animator.Play("PreviewIdle", -1, 0f);
            zone.GameUIUpdate();

            if(World.GetUserSingleTone().m_heartCount == 0)
            {
                GetComponent<UIButton>().SetState(UIButton.State.Disabled, true);
                GetComponent<UIButton>().isEnabled = false;
            }
        }
        else
        {
            bool able = zone.world.DiscountHeart(1);
            if (able == true)
            {
                isPop = true;
                zone.SetState(EGameState.eBlock);
                Background.SetActive(true);
                m_animator.enabled = true;
                m_animator.Play("PreviewPop", -1, 0f);
            }
        }

    }
}
