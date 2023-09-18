using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFir : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] GameObject Tank;

    [SerializeField] float BulletSpeed;               // 弾の移動スピード.
    [SerializeField, Range(0, 2)] int MaxReflect;     // 最大反射回数.
    [SerializeField] float CntReflect;                // 反射した回数.

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
    /// 弾のON・OFF.
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
    /// 弾の移動関数.
    /// </summary>
    private void BulletMove()
    {
        rb.velocity = transform.forward * BulletSpeed * Time.deltaTime;
    }
}
