using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] GameObject Target;    // 動かす対象.
    [SerializeField] string TargetName;
    [SerializeField] float MoveSpeed;      // 移動する速さ.
    [SerializeField] float RotationSpeed;  // 回転する速さ.
    private float RotationAngle;           // 累積回転角度
    [SerializeField] GameObject Tank;
    [SerializeField] GameObject Turret;
    [SerializeField] GameObject ShotPos;
    [SerializeField] MoveBullet MoveBullet;

    [SerializeField] int num;

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
        BulletGenerateClass.BulletInstantiate(gameObject, gameObject, "RealBullet", 1);

        for (int num = 0; num < transform.childCount; num++)
        {
            MoveBullet = transform.GetChild(num).gameObject.GetComponent<MoveBullet>();
            MoveBullet.StartRotation(Turret.transform.forward, ShotPos.transform.position);
        }
    }

    void Start()
    {
        
        Tank = transform.GetChild(0).gameObject;
        Turret = Tank.transform.GetChild(1).gameObject;
        ShotPos = Turret.transform.GetChild(0).gameObject;
        InitBullet();
    }

    void Update()
    {
        MoveInput(PlayerInput.KeyInput());
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
                Target.transform.position -= Target.transform.forward * MoveSpeed / 1.5f * Time.deltaTime;
                break;
            case KeyList.W:
                Target.transform.position += Target.transform.forward * MoveSpeed * Time.deltaTime;
                break;
            case KeyList.SPACE:
                if (num < transform.childCount)
                {
                    MoveBullet = transform.GetChild(num).gameObject.GetComponent<MoveBullet>();
                    MoveBullet.gameObject.SetActive(true);
                    MoveBullet.StartRotation(Turret.transform.forward, ShotPos.transform.position);
                    num++;
                    Debug.Log(num);
                    if(num >= transform.childCount)
                    {
                        num = 2;
                    }
                }
               

                break;
            default:

                break;
        }

        // 累積回転角度をもとに回転させる
        Target.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }
}
