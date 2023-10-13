using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveMouse();
    }

    /// <summary>
    /// マウスの位置にオブジェクトを移動.
    /// </summary>
    void MoveMouse()
    {
        Mouse mouse = Mouse.current;

        // カーソルの位置を取得.
        Vector3 MousePos = mouse.position.ReadValue();

        // カーソル位置のZ座標を変更 ※0だとカメラのレンズに張り付いている感じになり上手くワールド座標に変換できない為.
        MousePos.z = 10;

        Vector3 Target = Camera.main.ScreenToWorldPoint(MousePos);

        transform.position = Target;
    }
}
