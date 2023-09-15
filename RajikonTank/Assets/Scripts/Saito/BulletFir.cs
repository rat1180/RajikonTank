using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFir : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] GameObject Tank;

    [SerializeField] float BulletSpeed;               // �e�̈ړ��X�s�[�h.
    [SerializeField, Range(0, 2)] int MaxReflect;     // �ő唽�ˉ�.
    [SerializeField] float CntReflect;                // ���˂�����.

    private void OnDisable()
    {
        transform.position = Tank.transform.position;
        CntReflect = 0;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        CntReflect = 0;
        BulletActive(true);
        BulletMove();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �e��ON�EOFF.
    /// </summary>
    void BulletActive(bool isswitch)
    {
        if (isswitch)
        {
            gameObject.SetActive(true);
        }
        else if (!isswitch)
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// �e�̈ړ��֐�.
    /// </summary>
    private void BulletMove()
    {
        rb.velocity = transform.forward * BulletSpeed * Time.deltaTime;
    }
}
