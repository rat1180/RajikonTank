using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{ 
    Vector3 Direction;
    Rigidbody Rb;
    float Speed = 5.0f;
    int ReflectCount = 0;
    bool Flg = false;
    const int FalseCount = 2;

    public GameObject BulletHead;
    
    Vector3 TestTarget;
    // Start is called before the first frame update
    void Start()
    {
        BulletDestroy();

        //Transform = transform;

        //PrevPosition = Transform.position;

        Rb = this.transform.GetComponent<Rigidbody>();
        TestTarget = new Vector3(10,5, 0);
        Rotation(TestTarget);
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
    //直進させる
    void Moving()
    {
        Rb.velocity = BulletHead.transform.forward * Speed;
    }
    //送られた方向に向く
    void Rotation(Vector3 TargetPos)
    {
        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        BulletHead.transform.rotation = rotation;
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
            BulletDestroy();
            var TankObj = other.gameObject;
            TankDestroy(TankObj);
        }
    }
}
