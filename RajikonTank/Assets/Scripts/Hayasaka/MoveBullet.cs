using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    protected const int FalseCount = 2;
    protected int   ReflectCount;
    protected float Speed;
    protected bool  Flg = false;

    public GameObject BulletHead;

    protected Rigidbody Rb;
    protected Vector3 Direction;
    protected Vector3 TestStartPos;
    protected Vector3 TestTarget;
    private void OnDisable()
    {
        InitBullet();
    }

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
    public virtual void InitBullet()
    {
        Rb = this.transform.GetComponent<Rigidbody>();
        TestTarget = new Vector3(10, 0, 5);
        TestStartPos = new Vector3(1, 0, 0);
        gameObject.SetActive(false);
        ReflectCount = 0;
        Speed = 5.0f;
        StartRotation(TestTarget, TestStartPos);
    }
    //直進させる
    protected void Moving()
    {
        Rb.velocity = BulletHead.transform.forward * Speed;
    }
    //送られた方向に向く
    public void StartRotation(Vector3 TargetPos,Vector3 StartPos)
    {
        //add.h
        this.transform.position = StartPos;

        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        BulletHead.transform.rotation = rotation;

        //this.transform.position = StartPos;
        Rb.velocity = new Vector3(0, 0, 0);
    }
    //反射させる関数
    protected void Reflect(Vector3 WallObj)
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
    //弾の削除
    protected void BulletDestroy()
    {
        this.gameObject.SetActive(false);
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
        }

        //add.h
        if(other.gameObject.tag == "Bullet")
        {
            BulletDestroy();
        }
    }
}
