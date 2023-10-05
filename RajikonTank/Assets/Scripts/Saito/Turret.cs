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
            Vector3 DirectionTarget = Target.transform.position - transform.position;

            Quaternion TargetRotate = Quaternion.LookRotation(DirectionTarget, Vector3.up);

            // X軸とZ軸の回転を固定する.
            TargetRotate.eulerAngles = new Vector3(0, TargetRotate.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotate, RotateSpeed * Time.deltaTime);

        }
    }

    /// <summary>
    /// ターゲットの更新.
    /// </summary>
    public void TargetUpdate(GameObject target)
    {
        Target = target;
    }
}
