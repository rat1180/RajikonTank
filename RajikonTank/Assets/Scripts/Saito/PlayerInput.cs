using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    Vector2 LeftStickInput;
    Vector2 RightStickInput;

    public KeyList sendkey;    // 押されたキーの情報を送る変数.
    public Vector3 sendtarget; // 狙っている場所を送る変数

    [SerializeField] Controller NowController;

    Keyboard keyboard = Keyboard.current;
    Gamepad gamepad = Gamepad.current;

    float Threshold = 0.8f;  // 閾値の設定.

    bool W_Key;
    bool S_Key;
    bool A_Key;
    bool D_Key;
    bool Up_Key;
    bool Down_Key;

    bool Space_Key;

    bool LeftStickLeft;    // 左スティックを左に倒した時.
    bool LeftStickRight;   // 左スティックを右に倒した時.
    bool LeftStickUp;      // 左スティックを前に倒した時.
    bool LeftStickDown;    // 左スティックを後ろに倒した時.

    bool RightStickUp;     // 右スティックを前に倒した時.
    bool RightStickDown;   // 右スティックを後ろに倒した時.

    void Start()
    {
        gamepad = Gamepad.current;
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
        if (GameManager.instance.NowGameState != GAMESTATUS.INGAME) return new Vector3(0,0,0);

        return sendtarget;
    }

    /// <summary>
    /// 入力された方向キー
    /// </summary>
    /// <returns></returns>
    public virtual KeyList KeyInput()
    {
        if(GameManager.instance.NowGameState != GAMESTATUS.INGAME) return KeyList.NONE;

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
                case Controller.NORMAL:
                    NormalMode();
                    break;
                case Controller.RAJICON:
                    RajikonMode();
                    break;
            }
        }


        if (keyboard != null && gamepad == null)
        {
            W_Key = keyboard.wKey.isPressed;
            S_Key = keyboard.sKey.isPressed;
            A_Key = keyboard.aKey.isPressed;
            D_Key = keyboard.dKey.isPressed;
            Up_Key = keyboard.upArrowKey.isPressed;
            Down_Key = keyboard.downArrowKey.isPressed;

            Space_Key = keyboard.spaceKey.wasPressedThisFrame;

            RookieMode();

            //    if (Space_Key)
            //    {
            //        sendkey = KeyList.FIRE;
            //    }
            //    else if (W_Key && Up_Key)
            //    {
            //        sendkey = KeyList.ACCELE;
            //    }
            //    else if (S_Key && Down_Key)
            //    {
            //        sendkey = KeyList.BACK;
            //    }
            //    else if (S_Key && Up_Key)
            //    {
            //        sendkey = KeyList.LEFTHIGHSPEEDROTATION;
            //    }
            //    else if (W_Key && Down_Key)
            //    {
            //        sendkey = KeyList.RIGHTHIGHSPEEDROTATION;
            //    }
            //    else if (S_Key || Up_Key)
            //    {
            //        sendkey = KeyList.LEFTROTATION;
            //    }
            //    else if (W_Key || Down_Key)
            //    {
            //        sendkey = KeyList.RIGHTROTATION;
            //    }
            //    else
            //    {
            //        sendkey = KeyList.NONE;
            //    }
        }

            return sendkey;
    }
    
    
    void RookieMode()
    {
        if (Space_Key)
        {
            sendkey = KeyList.FIRE;
        }
        else if(W_Key && A_Key)
        {
            sendkey = KeyList.LEFTUP;
        }
        else if (W_Key && D_Key)
        {
            sendkey = KeyList.RIGHTUP;
        }
        else if (S_Key && A_Key)
        {
            sendkey = KeyList.LEFTDOWN;
        }
        else if (S_Key && D_Key)
        {
            sendkey = KeyList.RIGHTDOWN;
        }
        else if (W_Key || Up_Key)
        {
            sendkey = KeyList.W;
        }
        else if (S_Key || Down_Key)
        {
            sendkey = KeyList.S;
        }
        else if (A_Key)
        {
            sendkey = KeyList.A;
        }
        else if (D_Key)
        {
            sendkey = KeyList.D;
        }
        else
        {
            sendkey = KeyList.NONE;
        }
    }

    void NormalMode()
    {
        if (gamepad.rightTrigger.wasPressedThisFrame || gamepad.buttonEast.wasPressedThisFrame)
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
