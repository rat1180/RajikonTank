using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Rajikon : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] GameObject Target;    // ìÆÇ©Ç∑ëŒè€.
    [SerializeField] float MoveSpeed;      // à⁄ìÆÇ∑ÇÈë¨Ç≥.
    [SerializeField] float RotationSpeed;  // âÒì]Ç∑ÇÈë¨Ç≥.
    private float RotationAngle;           // ó›êœâÒì]äpìx
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

        // ó›êœâÒì]äpìxÇÇ‡Ç∆Ç…âÒì]Ç≥ÇπÇÈ
        Target.transform.rotation = Quaternion.AngleAxis(RotationAngle, transform.up);
    }
}
