using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Param : MonoBehaviour {
    public enum CostType
    {
        heart,
        crystal
    }
    public string m_name;
    public int m_price;
    public CostType m_costType;
    public Color32 m_tintColor;
}
