using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confetti : MonoBehaviour
{
    public Color[] m_randomColorArr = new Color[5];
    private int m_rotateSpeed = 0;
    private float m_moveSpeed = 0.1f;

    private void Awake()
    {
        int randomPick = Random.Range(0, 5);
        gameObject.GetComponent<UIWidget>().color = m_randomColorArr[randomPick];
        m_rotateSpeed = Random.Range(10,100);
        m_moveSpeed = Random.Range(0.2f, 0.4f);
        Destroy(gameObject, 6.0f);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * m_rotateSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * m_rotateSpeed * Time.deltaTime);
        transform.position = transform.position + new Vector3(0, -1 * m_moveSpeed * Time.deltaTime, 0);
    }
}
