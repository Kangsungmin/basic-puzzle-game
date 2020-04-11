using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewradIconController : MonoBehaviour
{
    public float m_rotateSpeed;
    public float m_moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        m_rotateSpeed = Random.Range(10, 300);
        m_moveSpeed = Random.Range(0.4f, 1f);
        Destroy(gameObject, 3.0f);
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * m_rotateSpeed * Time.deltaTime);
        transform.position = transform.position + new Vector3(0, -1 * m_moveSpeed * Time.deltaTime, 0);
    }
}
