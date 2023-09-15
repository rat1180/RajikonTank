using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    Vector3 Direction;
    Rigidbody Rb;
    float Speed = 5.0f;
    bool Flg = false;
    // Start is called before the first frame update
    void Start()
    {
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
    void Reflect(GameObject WallObj)
    {
        // 法線ベクトル
        var inNormal = WallObj.transform.up;
        // 反射ベクトル（速度）
        var result = Vector3.Reflect(Direction, inNormal);

        // バウンド後の速度をボールに反映
        Rb.velocity = result;

        Direction = Rb.velocity;
    }
    void OnCollisionEnter(Collision other)   // 触れている時のやつ
    {
        if (other.gameObject.tag == "Wall")    // 天井に直撃、死亡
        {
            var WallObj = other.gameObject;
            Flg = true;
            Reflect(WallObj);
        }
    }
}
