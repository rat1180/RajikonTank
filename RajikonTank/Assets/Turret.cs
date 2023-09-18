using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] float RotateSpeed;

    void FixedUpdate()
    {
        LookMouse();
    }

    void LookMouse()
    {
        // カメラとマウスの位置を元にRayを準備.
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // プレイヤーの高さにPlaneを更新して、カメラの情報を元に地面判定して距離を取得.
        Plane plane = new Plane(Vector3.up, transform.position);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            // 距離を元に交点を算出して、交点の方を向く.
            var LookPoint = ray.GetPoint(distance);

            // オブジェクトの上方向をY軸に設定して回転を計算.
            Vector3 upDirection = Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(LookPoint - transform.position, upDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }
    }
}
