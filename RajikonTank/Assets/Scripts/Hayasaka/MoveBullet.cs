using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    const int FalseCount = 2;
    int   ReflectCount = 0;
    float Speed = 5.0f;
    bool  Flg = false;

    public GameObject BulletHead;

    Rigidbody Rb;
    Vector3 Direction;
    Vector3 TestStartPos;
    Vector3 TestTarget;
    // Start is called before the first frame update
    void Start()
    {
        InitBullet();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Flg)
        {
            Moving();
        }
        Direction = Rb.velocity;
    }
    void InitBullet()
    {
        Rb = this.transform.GetComponent<Rigidbody>();
        TestTarget = new Vector3(10, 0, 0);
        TestStartPos = new Vector3(1, 0, 0);
        StartRotation(TestTarget, TestStartPos);
    }
    //直進させる
    void Moving()
    {
        Rb.velocity = BulletHead.transform.forward * Speed;
    }
    //送られた方向に向く,スタート位置に配置される
    void StartRotation(Vector3 TargetPos,Vector3 StartPos)
    {
        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        BulletHead.transform.rotation = rotation;

        this.transform.position = StartPos;
    }
    //反射させる関数
    void Reflect(Vector3 WallObj)
    {
        // 反射ベクトル（速度）
        var result = Vector3.Reflect(Direction,WallObj);

        // バウンド後の速度をボールに反映
        Rb.velocity = result;

        Direction = Rb.velocity;

        // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
        Quaternion rotation = Quaternion.LookRotation(result);

        // オブジェクトの回転に反映
        BulletHead.transform.rotation = rotation;
    }
   
    void TankDestroy(GameObject TankObj)
    {
        Destroy(TankObj);
    }
    //弾の削除
    void BulletDestroy()
    {
        this.gameObject.SetActive(false);
        ReflectCount = 0;
        Flg = false;
    }
    void OnCollisionEnter(Collision other)  
    {
        if (other.gameObject.tag == "Wall")
        {
            var WallObj = other.contacts[0].normal;
            Flg = true;
            ReflectCount++;
            if (ReflectCount == FalseCount)
            {
                BulletDestroy();
            }
            else
            {
                Reflect(WallObj);
            }
        }
        if (other.gameObject.tag == "Tank")
        {
            var TankObj = other.gameObject;
            BulletDestroy();
            TankDestroy(TankObj);
        }
    }
}
