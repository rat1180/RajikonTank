using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveQuickBullet : MoveBullet
{
    private void OnDisable()
    {
        InitBullet();
    }
    void Start()
    {
        Trail = GetComponent<TrailRenderer>();
        InitBullet();
    }
    void Update()
    {
        if (GameManager.instance.NowGameState != ConstList.GAMESTATUS.INGAME) BulletDestroy();

        if (!Flg)
        {
            Moving();
        }
        Direction = Rb.velocity;
    }
    public override void InitBullet()
    {
        Rb = this.transform.GetComponent<Rigidbody>();
        TestTarget = new Vector3(10, 0, 5);
        TestStartPos = new Vector3(1, 0, 0);
        gameObject.SetActive(false);
        ReflectCount = 1;
        Speed = 15.0f;
        StartRotation(TestTarget, TestStartPos);
    }
    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Wall")
    //    {
    //        var WallObj = other.contacts[0].normal;
    //        Flg = true;
    //        ReflectCount++;
    //        if (ReflectCount == FalseCount)
    //        {
    //            BulletDestroy();
    //        }
    //        else
    //        {
    //            Reflect(WallObj);
    //        }
    //    }
    //    if (other.gameObject.tag == "Tank")
    //    {
    //        BulletDestroy();
    //    }

    //    //add.h
    //    if (other.gameObject.tag == "Bullet")
    //    {
    //        BulletDestroy();
    //    }
    //}
}

