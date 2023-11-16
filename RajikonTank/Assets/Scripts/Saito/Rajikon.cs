using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;

    //add.h
    [SerializeField, Header("�^���N�̃C�x���g��ʒm����X�N���v�g")] TankEventHandler EventHandler;

    [SerializeField] float MoveSpeed;         // �ړ����x.
    [SerializeField] float RotationSpeed;     // �^���N�̉�]���x.
    [SerializeField] float TurretSpeed;       // �^���b�g�̉�]���x.
    [SerializeField] int MaxBulletNum;        // �e�̍ő吔.
    [SerializeField] List<bool> isFirBullet;  // �e�𔭎˂��Ă��邩.
    [SerializeField] int FalseBullet;         // ���Ă�e�̐�.
    [SerializeField] List<GameObject> Bullets;
    [SerializeField] int MaxBombNum;          // ���e�̍ő吔.
    [SerializeField] int HaveBombNum;         // ���ݎ����Ă��锚�e�̐�.
    private float RotationAngle;              // �ݐω�]�p�x.
    private float TankAngle;                  // �^���N�̊p�x.
    private float TurretAngle;                // �^���b�g�̊p�x.
    [SerializeField] float Interpolation;     // �^���N�̊p�x��]�̕�� ���l���������قǊ��炩�ɉ�]����.

    public bool isFixedTurret;      // �^���b�g���Œ肷�邩 true:�Œ� false:����.
    [SerializeField] bool isTurretAim; // �^���b�g�ŃG�C�����邩.

    public GameObject Tank;
    [SerializeField] GameObject Turret;
    public GameObject ShotPos;
    [SerializeField] GameObject BulletList;
    [SerializeField] MoveBullet MoveBullet;
    [SerializeField] GameObject Target;       // �_���Ώ�.
    [SerializeField] BulletPrefabNames NowBulletPrefabNames; // �e�̎��.

    protected AudioSource Audio;

    private float RightStickAngle;

    /// <summary>
    /// �������p�֐�.
    /// </summary>
    /// <param name="movespeed"></param>
    /// <param name="rotationspeed"></param>
    /// <param name="rotationangle"></param>
    public void InitRajikon(float movespeed, float rotationspeed, float rotationangle)
    {
        MoveSpeed = movespeed;
        RotationSpeed = rotationspeed;
        RotationAngle = rotationangle;
    }

    public void SetPlayerInput(PlayerInput input)
    {
        PlayerInput = input;
    }

    //add.h
    public void SetEventHandler(TankEventHandler eventhandler)
    {
        EventHandler = eventhandler;
    }

    private void InitBullet()
    {
        GameObject Bullet;

        for (int i = 0; i < MaxBulletNum; i++)
        {
            Bullet = BulletGenerateClass.BulletInstantiateOne(BulletList, NowBulletPrefabNames);
            Bullets.Add(Bullet);
        }
    }

    void Start()
    {

        Tank = transform.Find("Tank").gameObject;
        Turret = Tank.transform.Find("Turret").gameObject;
        ShotPos = Turret.transform.Find("ShotPosition").gameObject;
        BulletList = transform.Find("BulletList").gameObject;
        InitBullet();

        Audio = gameObject.AddComponent<AudioSource>();
        Audio.clip = Resources.Load<AudioClip>("Sounds/Move");
        Audio.loop = true;
        Audio.playOnAwake = false;
        Interpolation = 0.02f;
    }

    void Update()
    {
        if (PlayerInput == null) return;
        MoveInput(PlayerInput.KeyInput());
        if (isFixedTurret == false) LookTarget();
        TargetUpdate();

        // �v���C���[����E�X�e�B�b�N�̊p�x���擾.
        RightStickAngle = PlayerInput.SetRightStickAngle();

        // �^���b�g�̌�����ύX
        if (isTurretAim == true) TurretAim(RightStickAngle);

        DebugMode();
    }

    public void MoveInput(KeyList inputkey)
    {
        // Debug.Log(inputkey);

        //add.h
        if (PlayerInput == null) return;

        Move(PlayerInput.KeyInput());
    }

    private void Move(KeyList keylist)
    {
        // Debug.Log(keylist);

        var rotation = RotationSpeed * Time.deltaTime;

        if (keylist == KeyList.FIRE || keylist == KeyList.NONE)
        {

            Audio.Stop();
        }
        else
        {
            if (!Audio.isPlaying)
            {
                Audio.Play();
            }
        }

        switch (keylist)
        {
            case KeyList.LEFT_ROTATION:
                RotationAngle -= rotation;
                break;
            case KeyList.RIGHT_ROTATION:
                RotationAngle += rotation;
                break;
            case KeyList.LEFT_HIGHSPEED_ROTATION:
                RotationAngle -= rotation * 1.5f;
                break;
            case KeyList.RIGHT_HIGHSPEED_ROTATION:
                RotationAngle += rotation * 1.5f;
                break;
            case KeyList.BACK:
                Tank.transform.position -= Tank.transform.forward * MoveSpeed / 1.5f * Time.deltaTime;
                break;
            case KeyList.ACCELE:
                Tank.transform.position += Tank.transform.forward * MoveSpeed * Time.deltaTime;
                break;
            case KeyList.W:
                ForwardMove(0f);
                break;
            case KeyList.S:
                ForwardMove(180f);
                break;
            case KeyList.A:
                ForwardMove(270f);
                break;
            case KeyList.D:
                ForwardMove(90f);
                break;
            case KeyList.WA:
                ForwardMove(315f);
                break;
            case KeyList.WD:
                ForwardMove(45f);
                break;
            case KeyList.SA:
                ForwardMove(225f);
                break;
            case KeyList.SD:
                ForwardMove(125f);
                break;
            case KeyList.FIRE:
                Check();
                break;
            case KeyList.PLANT:
                GenerateBomb();
                break;
            default:

                break;
        }
        var TankRot = Quaternion.Euler(Tank.transform.rotation.x, TankAngle, Tank.transform.rotation.z);
        Tank.transform.rotation = Quaternion.Lerp(Tank.transform.rotation, TankRot, Interpolation);

        // �ݐω�]�p�x�����Ƃɉ�]������
        //Tank.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }

    /// <summary>
    /// �^���N�̃^���b�g���E�X�e�B�b�N�ő_���鏈��.
    /// </summary>
    /// <param name="angle"></param>
    private void TurretAim(float angle)
    {
        if(PlayerInput.isRightStickAngle() == true)
        {
            // �^���b�g���E�X�e�B�b�N�̊p�x�Ɍ�����.
            var TurretRot = Quaternion.Euler(Turret.transform.rotation.x, angle, Turret.transform.rotation.z);
            Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, TurretRot, TurretSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// �^���N���w�肵�������ɐi�܂��鏈��.
    /// </summary>
    private void ForwardMove(float angle)
    {
        TankAngle = angle;
        Tank.transform.position += Tank.transform.forward * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �^�[�Q�b�g�̕����Ƀ^���b�g�������鏈��.
    /// </summary>
    private void LookTarget()
    {
        if (Target == null)
        {
            Debug.Log("�_���Ώۂ�����܂���");
        }
        else if (Target != null)
        {
            Vector3 DirectionTarget = Target.transform.position - Turret.transform.position;

            Quaternion TargetRotate = Quaternion.LookRotation(DirectionTarget, Vector3.up);

            // X����Z���̉�]���Œ肷��.
            TargetRotate.eulerAngles = new Vector3(0, TargetRotate.eulerAngles.y, 0);

            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, TargetRotate, TurretSpeed * Time.deltaTime);

        }
    }

    /// <summary>
    /// �^�[�Q�b�g�̍X�V.
    /// </summary>
    public void TargetUpdate()
    {
        Target.transform.position = PlayerInput.TargetPosition();
    }

    public void SetTargetObject(GameObject target)
    {
        Target = target;
    }

    private void Check()
    {
        for (int i = 0; i < Bullets.Count; i++)
        {
            if (Bullets[i].activeSelf == false)
            {
                Bullets[i].gameObject.SetActive(true);
                Bullets[i].GetComponent<MoveBullet>().StartRotation(ShotPos.transform.forward, ShotPos.transform.position);

                //�e�X�g
                EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Bullet_Fire, ShotPos.transform.position);

                return;
            }

        }
    }

    public int GetRestBullet()
    {
        FalseBullet = 0;

        for (int i = 0; i < Bullets.Count; i++)
        {
            if (Bullets[i].activeSelf == false)
            {
                FalseBullet++;
            }
        }

        return FalseBullet;
    }

    //add.h
    public void TankHit()
    {
        if (EventHandler == null) return;

        EventHandler.TankHit();
    }

    public void SetPlayTrail(bool isPlay)
    {
        Tank.GetComponent<Tank>().SetPlayTrail(isPlay);
    }

    /// <summary>
    /// ���e�̐���.
    /// </summary>
    private void GenerateBomb()
    {
        if (HaveBombNum != 0)
        {
            // ���e�̐���.
            var bomb = ResorceManager.Instance.GetOtherResorce(OtherPrefabNames.Bomb);
            Instantiate(bomb, Tank.transform.position, Quaternion.identity);
            AddBomb(-1);
        }
        else
        {
            Debug.Log("���e�𐶐��ł��܂���");
        }
    }

    // ���e�����炵���葝�₵���肷�鏈��.
    public void AddBomb(int addnum)
    {
        HaveBombNum += addnum;

        if(HaveBombNum > MaxBombNum)
        {
            HaveBombNum = MaxBombNum;
        }
    }

    /// <summary>
    /// ���ݎ����Ă���e�̐�.
    /// </summary>
    /// <returns></returns>
    public int GetBomb()
    {
        return HaveBombNum;
    }

    void DebugMode()
    {
        if (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.A))
        {
            MaxBombNum = 1000;
            HaveBombNum = 1000;
        }
    }

    protected int SetMaxBombNum(int num)
    {
        return MaxBombNum = num;
    }
}
