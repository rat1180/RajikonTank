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

    [SerializeField] Controller NowController;

    Keyboard keyboard = Keyboard.current;
    Gamepad gamepad = Gamepad.current;
    float Threshold = 0.8f;  // 閾値の設定.

    bool LeftStickLeft;    // 左スティックを左に倒した時.
    bool LeftStickRight;   // 左スティックを右に倒した時.
    bool LeftStickUp;      // 左スティックを前に倒した時.
    bool LeftStickDown;    // 左スティックを後ろに倒した時.

    bool RightStickUp;     // 右スティックを前に倒した時.
    bool RightStickDown;   // 右スティックを後ろに倒した時.

    void Start()
    {
        
    }

    void Update()
    {
        InformationStick();
    }

    /// <summary>
    /// スティックの倒している向き情報.
    /// </summary>
    void InformationStick()
    {
        LeftStickLeft = LeftStickInput.x <= -Threshold;   // 左スティックを左に倒した時.
        LeftStickRight = LeftStickInput.x >= Threshold;   // 左スティックを右に倒した時.
        LeftStickUp = LeftStickInput.y >= Threshold;      // 左スティックを前に倒した時.
        LeftStickDown = LeftStickInput.y <= -Threshold;   // 左スティックを後ろに倒した時.

        RightStickUp = RightStickInput.y >= Threshold;    // 右スティックを前に倒した時.
        RightStickDown = RightStickInput.y <= -Threshold; // 右スティックを後ろに倒した時.
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
        if (gamepad != null)
        {
            Debug.Log("ゲームパッドが接続されました");
            LeftStickInput = gamepad.leftStick.ReadValue();
            RightStickInput = gamepad.rightStick.ReadValue();

            switch (NowController)
            {
                case Controller.ROOKIE:
                    RookieMode();
                    break;
                case Controller.RAJICON:
                    RajikonMode();
                    break;
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

    void RookieMode()
    {
        if (gamepad.rightTrigger.wasPressedThisFrame)
        {
            sendkey = KeyList.FIRE;
        }
        else if (LeftStickUp)
        {
            sendkey = KeyList.ACCELE;
        }
        else if (LeftStickDown)
        {
            sendkey = KeyList.BACK;
        }
        else if (LeftStickLeft)
        {
            sendkey = KeyList.LEFTHIGHSPEEDROTATION;
        }
        else if (LeftStickRight)
        {
            sendkey = KeyList.RIGHTHIGHSPEEDROTATION;
        }
        else
        {
            sendkey = KeyList.NONE;
        }
    }

    void RajikonMode()
    {
        if (gamepad.rightTrigger.wasPressedThisFrame)
        {
            sendkey = KeyList.FIRE;
        }
        else if (LeftStickUp && RightStickUp)
        {
            sendkey = KeyList.ACCELE;
        }
        else if (LeftStickDown && RightStickDown)
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
}
