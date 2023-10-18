using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    Vector2 LeftStickInput;
    Vector2 RightStickInput;

    public KeyList sendkey;  // 押されたキーの情報を送る変数.
    public Vector3 sendtarget;   // 狙っている場所を送る変数

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// タンクが狙う座標
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 TargetPosition()
    {
        return sendtarget;
    }

    /// <summary>
    /// 入力された方向キー
    /// </summary>
    /// <returns></returns>
    public virtual KeyList KeyInput()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        float Threshold = 0.3f;  // 閾値の設定.
        var LeftStickUp = LeftStickInput.y >= Threshold;      // 左スティックを前に倒した時.
        var LeftStickDown = LeftStickInput.y <= -Threshold;   // 左スティックを後ろに倒した時.
        var RightStickUp = RightStickInput.y >= Threshold;    // 右スティックを前に倒した時.
        var RightStickDown = RightStickInput.y <= -Threshold; // 右スティックを後ろに倒した時.

        if (gamepad != null)
        {
            Debug.Log("ゲームパッドが接続されました");
            LeftStickInput = gamepad.leftStick.ReadValue();
            RightStickInput = gamepad.rightStick.ReadValue();

            if (gamepad.rightTrigger.wasPressedThisFrame)
            {
                sendkey = KeyList.FIRE;
            }
            else if(LeftStickUp && RightStickUp)
            {
                sendkey = KeyList.ACCELE;
            }
            else if(LeftStickDown && RightStickDown)
            {
                sendkey = KeyList.BACK;
            }
            else if (LeftStickDown && RightStickUp)
            {
                sendkey = KeyList.LEFTHIGHSPEEDROTATION;
            }
            else if (LeftStickUp && RightStickDown)
            {
                sendkey = KeyList.RIGHTHIGHSPEEDROTATION;
            }
            else if (LeftStickDown || RightStickUp)
            {
                sendkey = KeyList.LEFTROTATION;
            }
            else if (LeftStickUp || RightStickDown)
            {
                sendkey = KeyList.RIGHTROTATION;
            }
            else
            {
                sendkey = KeyList.NONE;
            }
        }
        
        if(keyboard != null && gamepad == null)
        {
            var wkey = keyboard.wKey.isPressed;
            var skey = keyboard.sKey.isPressed;
            var upkey = keyboard.upArrowKey.isPressed;
            var downkey = keyboard.downArrowKey.isPressed;

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                sendkey = KeyList.FIRE;
            }
            else if (wkey && upkey)
            {
                sendkey = KeyList.ACCELE;
            }
            else if (skey && downkey)
            {
                sendkey = KeyList.BACK;
            }
            else if (skey && upkey)
            {
                sendkey = KeyList.LEFTHIGHSPEEDROTATION;
            }
            else if (wkey && downkey)
            {
                sendkey = KeyList.RIGHTHIGHSPEEDROTATION;
            }
            else if (skey || upkey)
            {
                sendkey = KeyList.LEFTROTATION;
            }
            else if (wkey || downkey)
            {
                sendkey = KeyList.RIGHTROTATION;
            }
            else
            {
                sendkey = KeyList.NONE;
            }
        }

        return sendkey;

    }
}
