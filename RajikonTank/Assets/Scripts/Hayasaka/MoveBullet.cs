using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    //Transform Transform;

    //Vector3 PrevPosition;

    Vector3 Direction;
    Rigidbody Rb;
    float Speed = 5.0f;
    int ReflectCount = 0;
    bool Flg = false;
    const int FalseCount = 2;
    // Start is called before the first frame update
    void Start()
    {
        //Transform = transform;

        //PrevPosition = Transform.position;

        Rb = this.transform.GetComponent<Rigidbody>();
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
    void Moving()
    {
        Rb.velocity = new Vector3(0,Speed,0);
    }
    void Reflect(Vector3 WallObj)
    {
        // 反射ベクトル（速度）
        var result = Vector3.Reflect(Direction,WallObj);

        // バウンド後の速度をボールに反映
        Rb.velocity = result;

        Direction = Rb.velocity;

        // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
        var rotation = Quaternion.LookRotation(WallObj.normalized,transform.position + result);

        // オブジェクトの回転に反映
        transform.rotation = rotation;
    }
    void TankDestroy(GameObject TankObj)
    {
        Destroy(TankObj);
    }
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
