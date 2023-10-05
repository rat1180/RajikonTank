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
    //���i������
    void Moving()
    {
        Rb.velocity = BulletHead.transform.forward * Speed;
    }
    //����ꂽ�����Ɍ���,�X�^�[�g�ʒu�ɔz�u�����
    void StartRotation(Vector3 TargetPos,Vector3 StartPos)
    {
        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        BulletHead.transform.rotation = rotation;

        this.transform.position = StartPos;
    }
    //���˂�����֐�
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
        BulletHead.transform.rotation = rotation;
    }
   
    void TankDestroy(GameObject TankObj)
    {
        Destroy(TankObj);
    }
    //�e�̍폜
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
