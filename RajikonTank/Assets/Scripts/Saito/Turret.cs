using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] float RotateSpeed;
    [SerializeField] GameObject Target;

    void FixedUpdate()
    {
        LookTarget();
    }

    /// <summary>
    /// �^�[�Q�b�g�̕����Ƀ^���b�g�������鏈��.
    /// </summary>
    void LookTarget()
    {
        if (Target == null)
        {
            Debug.Log("�_���Ώۂ�����܂���");
        }
        else if (Target != null)
        {
            Vector3 DirectionTarget = Target.transform.position - transform.position;

            Quaternion TargetRotate = Quaternion.LookRotation(DirectionTarget, Vector3.up);

            // X����Z���̉�]���Œ肷��.
            TargetRotate.eulerAngles = new Vector3(0, TargetRotate.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotate, RotateSpeed * Time.deltaTime);

        }
    }

    /// <summary>
    /// �^�[�Q�b�g�̍X�V.
    /// </summary>
    public void TargetUpdate(GameObject target)
    {
        Target = target;
    }
}
