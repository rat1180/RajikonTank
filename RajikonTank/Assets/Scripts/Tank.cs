using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Tank : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] GameObject Target;    // 動かす対象.
    [SerializeField] float MoveSpeed;      // 移動する速さ.
    [SerializeField] float RotationSpeed;  // 回転する速さ.
    private float RotationAngle;           // 累積回転角度
    [SerializeField]GameObject Bullet;

    void Start()
    {

    }

    void Update()
    {
        MoveInput(PlayerInput.KeyInput());
    }

    public void MoveInput(KeyList inputkey)
    {
        Debug.Log(inputkey);
        Move(PlayerInput.KeyInput());
    }

    private void Move(KeyList keylist)
    {
        Debug.Log(keylist);

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
                BulletGenerateClass.BulletInstantiate(Bullet, "Bullet", 3);
                break;
            default:

                break;
        }

        // 累積回転角度をもとに回転させる
        Target.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }
}
