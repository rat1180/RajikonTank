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
    void Reflect(Vector3 WallObj)
    {
        // ���˃x�N�g���i���x�j
        var result = Vector3.Reflect(Direction,WallObj);

        // �o�E���h��̑��x���{�[���ɔ��f
        Rb.velocity = result;

        Direction = Rb.velocity;
    }
    void OnCollisionEnter(Collision other)   // �G��Ă��鎞�̂��
    {
        if (other.gameObject.tag == "Wall")    // �V��ɒ����A���S
        {
            var WallObj = other.contacts[0].normal;
            Flg = true;
            Reflect(WallObj);
        }
    }
}
