using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;

    //add.h
    [SerializeField, Header("タンクのイベントを通知するスクリプト")] TankEventHandler EventHandler;

    [SerializeField] float MoveSpeed;         // 移動する速さ.
    [SerializeField] float RotationSpeed;     // 回転する速さ.
    [SerializeField] float TurretSpeed;       // タレットの回転速さ.
    [SerializeField] int MaxBulletNum;        // 弾の最大数.
    [SerializeField] List<bool> isFirBullet;  // 弾を発射しているか.
    [SerializeField] int FalseBullet;         // 撃てる弾の数.
    [SerializeField] List<GameObject> Bullets;
    private float RotationAngle;              // 累積回転角度.
    private float TankAngle;                  // タンクの角度.
    private float TurretAngle;                // タレットの角度.
    [SerializeField] float Interpolation;     // タンクの角度回転の補間 ※値が小さいほど滑らかに回転する.

    public bool isFixedTurret;      // タレットを固定するか true:固定 false:解除.
    [SerializeField] bool isTurretAim; // タレットでエイムするか.

    public GameObject Tank;
    [SerializeField] GameObject Turret;
    public GameObject ShotPos;
    [SerializeField] GameObject BulletList;
    [SerializeField] MoveBullet MoveBullet;
    [SerializeField] GameObject Target;       // 狙う対象.
    [SerializeField] BulletPrefabNames NowBulletPrefabNames; // 弾の種類.

    protected AudioSource Audio;

    /// <summary>
    /// 初期化用関数.
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
        if(isFixedTurret == false) LookTarget();
        TargetUpdate();

        // プレイヤーから右スティックの角度を取得
        float rightStickAngle = PlayerInput.SetRightStickAngle();


        // タレットの向きを変更
        if (isTurretAim == true) TurretAim(rightStickAngle);
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
            case KeyList.LEFTROTATION:
                RotationAngle -= rotation;
                break;
            case KeyList.RIGHTROTATION:
                RotationAngle += rotation;
                break;
            case KeyList.LEFTHIGHSPEEDROTATION:
                RotationAngle -= rotation * 1.5f;
                break;
            case KeyList.RIGHTHIGHSPEEDROTATION:
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
            default:

                break;
        }
        var TankRot = Quaternion.Euler(Tank.transform.rotation.x, TankAngle, Tank.transform.rotation.z);
        Tank.transform.rotation = Quaternion.Lerp(Tank.transform.rotation, TankRot, Interpolation);

        // 累積回転角度をもとに回転させる
        //Tank.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }

    private void TurretAim(float angle)
    {
        // タレットを右スティックの角度に向ける
        var TurretRot = Quaternion.Euler(Turret.transform.rotation.x, angle, Turret.transform.rotation.z);
        Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, TurretRot, Interpolation);
    }

    /// <summary>
    /// タンクを指定した向きに進ませる処理.
    /// </summary>
    void ForwardMove(float angle)
    {
        TankAngle = angle;
        Tank.transform.position += Tank.transform.forward * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ターゲットの方向にタレットを向ける処理.
    /// </summary>
    void LookTarget()
    {
        if (Target == null)
        {
            Debug.Log("狙う対象がありません");
        }
        else if (Target != null)
        {
            Vector3 DirectionTarget = Target.transform.position - Turret.transform.position;

            Quaternion TargetRotate = Quaternion.LookRotation(DirectionTarget, Vector3.up);

            // X軸とZ軸の回転を固定する.
            TargetRotate.eulerAngles = new Vector3(0, TargetRotate.eulerAngles.y, 0);

            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, TargetRotate, TurretSpeed * Time.deltaTime);

        }
    }

    /// <summary>
    /// ターゲットの更新.
    /// </summary>
    public void TargetUpdate()
    {
        Target.transform.position = PlayerInput.TargetPosition();
    }

    public void SetTargetObject(GameObject target)
    {
        Target = target;
    }

    void Check()
    {
        for(int i = 0; i < Bullets.Count; i++)
        {
            if(Bullets[i].activeSelf == false)
            {
                Bullets[i].gameObject.SetActive(true);
                Bullets[i].GetComponent<MoveBullet>().StartRotation(ShotPos.transform.forward, ShotPos.transform.position);

                //テスト
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
}
