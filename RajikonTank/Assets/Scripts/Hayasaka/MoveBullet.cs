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

    public GameObject Target;
    
    Vector3 TestTarget;
    // Start is called before the first frame update
    void Start()
    {
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
    void Moving()
    {
        Rb.velocity = Target.transform.forward * Speed;
    }
    void Rotation(Vector3 TargetPos)
    {
        // �I�u�W�F�N�g���ڕW�I�u�W�F�N�g�̈ʒu�������悤�ɂ���
        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        Target.transform.rotation = rotation;
    }
    void Reflect(Vector3 WallObj)
    {
        // ���˃x�N�g���i���x�j
        var result = Vector3.Reflect(Direction,WallObj);

        // �o�E���h��̑��x���{�[���ɔ��f
        Rb.velocity = result;

        Direction = Rb.velocity;

        // �i�s�����i�ړ��ʃx�N�g���j�Ɍ����悤�ȃN�H�[�^�j�I�����擾
        Quaternion rotation = Quaternion.LookRotation(result);

        // �I�u�W�F�N�g�̉�]�ɔ��f
        Target.transform.rotation = rotation;
    }
    void TankDestroy(GameObject TankObj)
    {
        Destroy(TankObj);
    }
    void BulletDestroy()
    {
        //this.gameObject.SetActive(false);
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
