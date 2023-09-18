using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    Vector3 Direction;
    Rigidbody Rb;
    float Speed = 5.0f;
    int ReflectCount;
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
    void Reflect(Vector3 WallObj)
    {
        // 反射ベクトル（速度）
        var result = Vector3.Reflect(Direction,WallObj);

        // バウンド後の速度をボールに反映
        Rb.velocity = result;

        Direction = Rb.velocity;
    }
    //void ReflectCount()
    //{

    //}
    void ObjDestroy(GameObject TankObj)
    {
        Destroy(TankObj);
        this.gameObject.SetActive(false);
    }
    void OnCollisionEnter(Collision other)  
    {
        if (other.gameObject.tag == "Wall")
        {
            var WallObj = other.contacts[0].normal;
            Flg = true;
            Reflect(WallObj);
        }
        if (other.gameObject.tag == "Tank")
        {
            var TankObj = other.gameObject;
            ObjDestroy(TankObj);
        }
    }
}
