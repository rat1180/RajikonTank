using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] float MoveSpeed;      // 移動する速さ.
    [SerializeField] float RotationSpeed;  // 回転する速さ.
    [SerializeField] float TurretSpeed;    // タレットの回転速さ.
    [SerializeField] int Num;
    [SerializeField] int MaxBulletNum;     // 弾の最大数.
    private float RotationAngle;           // 累積回転角度.

    [SerializeField] GameObject Tank;
    [SerializeField] GameObject Turret;
    [SerializeField] GameObject ShotPos;
    [SerializeField] GameObject BulletList;
    [SerializeField] MoveBullet MoveBullet;
    [SerializeField] GameObject Target;    // 狙う対象.

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

    private void InitBullet()
    {
        BulletGenerateClass.BulletInstantiate(gameObject, BulletList.gameObject, "RealBullet", MaxBulletNum);

        for (int num = 0; num < BulletList.transform.childCount; num++)
        {
            MoveBullet = BulletList.transform.GetChild(num).gameObject.GetComponent<MoveBullet>();
            MoveBullet.StartRotation(Turret.transform.forward, ShotPos.transform.position);
        }
    }

    void Start()
    {
        
        Tank = transform.GetChild(0).gameObject;
        BulletList = transform.GetChild(2).gameObject;
        Turret = Tank.transform.GetChild(1).gameObject;
        ShotPos = Turret.transform.GetChild(0).gameObject;
        InitBullet();
    }

    void Update()
    {
        MoveInput(PlayerInput.KeyInput());
        LookTarget();
        TargetUpdate();
    }

    public void MoveInput(KeyList inputkey)
    {
        // Debug.Log(inputkey);
        Move(PlayerInput.KeyInput());
    }

    private void Move(KeyList keylist)
    {
       // Debug.Log(keylist);

        var rotation = RotationSpeed * Time.deltaTime;

        switch (keylist)
        {

            case KeyList.A:
                RotationAngle -= rotation;
                break;
            case KeyList.D:
                RotationAngle += rotation;
                break;
            case KeyList.S:
                Tank.transform.position -= Tank.transform.forward * MoveSpeed / 1.5f * Time.deltaTime;
                break;
            case KeyList.W:
                Tank.transform.position += Tank.transform.forward * MoveSpeed * Time.deltaTime;
                break;
            case KeyList.SPACE:
                if (Num < BulletList.transform.childCount)
                {
                    MoveBullet = BulletList.transform.GetChild(Num).gameObject.GetComponent<MoveBullet>();
                    MoveBullet.gameObject.SetActive(true);
                    MoveBullet.StartRotation(Turret.transform.forward, ShotPos.transform.position);
                    Num++;
                    Debug.Log(Num);
                    if(Num >= BulletList.transform.childCount)
                    {
                        Num = 0;
                    }
                }
               

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
}
