using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] float MoveSpeed;      // ˆÚ“®‚·‚é‘¬‚³.
    [SerializeField] float RotationSpeed;  // ‰ñ“]‚·‚é‘¬‚³.
    [SerializeField] float TurretSpeed;    // ƒ^ƒŒƒbƒg‚Ì‰ñ“]‘¬‚³.
    [SerializeField] int Num;
    [SerializeField] int MaxBulletNum;     // ’e‚ÌÅ‘å”.
    private float RotationAngle;           // —İÏ‰ñ“]Šp“x.

    [SerializeField] GameObject Tank;
    [SerializeField] GameObject Turret;
    [SerializeField] GameObject ShotPos;
    [SerializeField] GameObject BulletList;
    [SerializeField] MoveBullet MoveBullet;
    [SerializeField] GameObject Target;    // ‘_‚¤‘ÎÛ.

    /// <summary>
    /// ‰Šú‰»—pŠÖ”.
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

        // —İÏ‰ñ“]Šp“x‚ğ‚à‚Æ‚É‰ñ“]‚³‚¹‚é
        Tank.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }

    /// <summary>
    /// ƒ^[ƒQƒbƒg‚Ì•ûŒü‚Éƒ^ƒŒƒbƒg‚ğŒü‚¯‚éˆ—.
    /// </summary>
    void LookTarget()
    {
        if (Target == null)
        {
            Debug.Log("‘_‚¤‘ÎÛ‚ª‚ ‚è‚Ü‚¹‚ñ");
        }
        else if (Target != null)
        {
            Vector3 DirectionTarget = Target.transform.position - Turret.transform.position;

            Quaternion TargetRotate = Quaternion.LookRotation(DirectionTarget, Vector3.up);

            // X²‚ÆZ²‚Ì‰ñ“]‚ğŒÅ’è‚·‚é.
            TargetRotate.eulerAngles = new Vector3(0, TargetRotate.eulerAngles.y, 0);

            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, TargetRotate, TurretSpeed * Time.deltaTime);

        }
    }

    /// <summary>
    /// ƒ^[ƒQƒbƒg‚ÌXV.
    /// </summary>
    public void TargetUpdate(Vector3 target)
    {
        Target.transform.position = target;
    }
}
