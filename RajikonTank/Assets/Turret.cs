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
        // カメラの視点からマウスの現在の位置に向かうRayを作成.
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // プレイヤーの高さにPlaneObjを作成して、カメラの情報を元に地面判定して距離を取得.
        Plane plane = new Plane(Vector3.up, transform.position);
        
        // Rayの交点距離を格納するための変数.
        float distance;

        // planeがRayと交差する場合.
        if (plane.Raycast(ray, out distance))
        {
            // 距離を元に交点を算出して、交点の方を向く.
            var LookPoint = ray.GetPoint(distance);

            // オブジェクトの上方向をY軸に設定して回転する処理.
            Vector3 UpDirection = Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(LookPoint - transform.position, UpDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }
    }
}
