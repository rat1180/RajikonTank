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

    protected TrailRenderer Trail;
    private void OnDisable()
    {
        InitBullet();
    }

    private void Awake()
    {
        Trail = GetComponent<TrailRenderer>();
        Rb = this.transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBullet();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.NowGameState != ConstList.GAMESTATUS.INGAME) gameObject.SetActive(false);

        if (!Flg)
        {
            Moving();
        }
        Direction = Rb.velocity;
    }
    public virtual void InitBullet()
    {
        TestTarget = new Vector3(10, 0, 5);
        TestStartPos = new Vector3(1, 0, 0);
        gameObject.SetActive(false);
        ReflectCount = 0;
        Speed = 5.0f;
        StartRotation(TestTarget, TestStartPos);
    }
    //���i������
    protected void Moving()
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

        Rb.velocity = new Vector3(0, 0, 0);

        SetPlayTrail(true);
    }
    //���˂�����֐�
    protected void Reflect(Vector3 WallObj)
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


        GameManager.instance.PlaySE(ConstList.SE_ID.Reflect);

    }
    //�e�̍폜
    protected void BulletDestroy()
    {
        GameManager.instance.PlaySE(ConstList.SE_ID.BulletDestroy);
        SetPlayTrail(false);
        this.gameObject.SetActive(false);
        Flg = false;
    }
    void OnCollisionEnter(Collision other)
    {
        if (GameManager.instance.NowGameState != ConstList.GAMESTATUS.INGAME) return;
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "BreakWall")
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
        if (other.gameObject.tag == "Bullet")
        {
            EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Bullet_Hit, transform.position);
            BulletDestroy();
        }
    }

    protected void SetPlayTrail(bool isPlay)
    {
        Trail.emitting = isPlay;
        Trail.Clear();
    }
}
