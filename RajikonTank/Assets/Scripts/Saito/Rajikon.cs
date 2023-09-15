using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] GameObject Target;    // �������Ώ�.
    [SerializeField] float MoveSpeed;      // �ړ����鑬��.
    [SerializeField] float RotationSpeed;  // ��]���鑬��.
    private float RotationAngle;           // �ݐω�]�p�x
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
                BulletGenerateClass.BulletInstantiate(Bullet, "Bullet", 3);
                break;
            default:

                break;
        }

        // �ݐω�]�p�x�����Ƃɉ�]������
        Target.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }
}
