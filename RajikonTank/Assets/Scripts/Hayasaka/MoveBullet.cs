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
    void InitBullet()
    {
        Rb = this.transform.GetComponent<Rigidbody>();
        TestTarget = new Vector3(10, 0, 5);
        TestStartPos = new Vector3(1, 0, 0);
        StartRotation(TestTarget, TestStartPos);
        gameObject.SetActive(false);
    }
    //���i������
    void Moving()
    {
        Rb.velocity = BulletHead.transform.forward * Speed;
    }
    //����ꂽ�����Ɍ���
    public void StartRotation(Vector3 TargetPos,Vector3 StartPos)
    {
        //add.h
        this.transform.position = StartPos;

        Quaternion rotation = Quaternion.LookRotation(TargetPos);
        BulletHead.transform.rotation = rotation;

        //this.transform.position = StartPos;

        ReflectCount = 0;
        Rb.velocity = new Vector3(0, 0, 0);
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
            //�e�X�g
            EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Bullet_Hit, transform.position);

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
