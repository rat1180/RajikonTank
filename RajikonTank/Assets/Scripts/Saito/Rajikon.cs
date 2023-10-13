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
    [SerializeField] List<GameObject> Bullets;
    private float RotationAngle;              // 累積回転角度.

    public GameObject Tank;
    [SerializeField] GameObject Turret;
    [SerializeField] GameObject ShotPos;
    [SerializeField] GameObject BulletList;
    [SerializeField] MoveBullet MoveBullet;
    [SerializeField] GameObject Target;       // 狙う対象.

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

        for(int i = 0; i < MaxBulletNum; i++)
        {
            Bullet = BulletGenerateClass.BulletInstantiateOne(BulletList, "RealBullet");
            Bullets.Add(Bullet);
        }
    }

    void Start()
    {
        
        Tank = transform.GetChild(0).gameObject;
        Turret = Tank.transform.GetChild(1).gameObject;
        ShotPos = Turret.transform.GetChild(0).gameObject;
        BulletList = transform.GetChild(1).gameObject;
        InitBullet();
    }

    void Update()
    {
        if (PlayerInput == null) return;
        MoveInput(PlayerInput.KeyInput());
        LookTarget();
        TargetUpdate();
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
            case KeyList.FIRE:
                Check();
                break;
            default:

                break;
        }

        // 累積回転角度をもとに回転させる
        Tank.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
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
            if(Bullets[i].activeSelf==false)
            {
                Bullets[i].gameObject.SetActive(true);
                Bullets[i].GetComponent<MoveBullet>().StartRotation(ShotPos.transform.forward, ShotPos.transform.position);

                //テスト
                EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Bullet_Fire, ShotPos.transform.position);

                return;
            }
        }
    }

    //add.h
    public void TankHit()
    {
        if (EventHandler == null) return;

        EventHandler.TankHit();
    }
    
}
