using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Vector2 LeftStickVector;
    private Vector2 RightStickVector;
    [SerializeField] InputActionReference RightStickInput;
    private Vector2 RightStickValue;

    public KeyList sendkey;        // 押されたキーの情報を送る変数.
    public RightStickList RightStickSend; // 右スティックの情報を送る変数.
    public Vector3 sendtarget;     // 狙っている場所を送る変数.

    [SerializeField] ControllerMode NowController;

    private Keyboard keyboard = Keyboard.current;
    private Gamepad gamepad = Gamepad.current;

    private float Threshold = 0.1f;  // 閾値の設定.
    protected float angle;

    private bool W_Key;
    private bool S_Key;
    private bool A_Key;
    private bool D_Key;
    private bool Up_Key;
    private bool Down_Key;

    private bool Space_Key;

    private bool LeftStickLeft;    // 左スティックを左に倒した時.
    private bool LeftStickRight;   // 左スティックを右に倒した時.
    private bool LeftStickUp;      // 左スティックを前に倒した時.
    private bool LeftStickDown;    // 左スティックを後ろに倒した時.

    private bool RightStickLeft;   // 右スティックを左に倒した時.
    private bool RightStickRight;  // 右スティックを右に倒した時.
    private bool RightStickUp;     // 右スティックを前に倒した時.
    private bool RightStickDown;   // 右スティックを後ろに倒した時.

    private bool LeftTrigger2;     // 左トリガーの下. 
    private bool RightTrigger2;    // 右トリガーの下.
    private bool ButtonEast;       // 東(○)のボタンを押した時.
    private bool ButtonSouth;      // 南(×)のボタンを押した時.

    private void OnEnable()
    {
        if (gamepad == null) return;
        RightStickInput.action.Enable();
    }

    private void OnDisable()
    {
        if (gamepad == null) return;
        RightStickInput.action.Disable();
    }

    void Start()
    {
        angle = 0;
    }

    void Update()
    {
        if (gamepad == null) return;
        gamepad = Gamepad.current;
        RightStickValue = Gamepad.current.rightStick.ReadValue();
        InformationStick();
    }

    /// <summary>
    /// スティックの倒している向き情報.
    /// </summary>
    private void InformationStick()
    {
        LeftStickLeft = LeftStickVector.x <= -Threshold;   // 左スティックを左に倒した時.
        LeftStickRight = LeftStickVector.x >= Threshold;   // 左スティックを右に倒した時.
        LeftStickUp = LeftStickVector.y >= Threshold;      // 左スティックを前に倒した時.
        LeftStickDown = LeftStickVector.y <= -Threshold;   // 左スティックを後ろに倒した時.

        RightStickLeft = RightStickVector.x <= -Threshold; // 右スティックを左に倒した時.
        RightStickRight = RightStickVector.x >= Threshold; // 右スティックを右に倒した時.
        RightStickUp = RightStickVector.y >= Threshold;    // 右スティックを前に倒した時.
        RightStickDown = RightStickVector.y <= -Threshold; // 右スティックを後ろに倒した時.

        LeftTrigger2 = gamepad.leftTrigger.wasPressedThisFrame;
        RightTrigger2 = gamepad.rightTrigger.wasPressedThisFrame;
        ButtonEast = gamepad.buttonEast.wasPressedThisFrame;
        ButtonSouth = gamepad.buttonSouth.wasPressedThisFrame;
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
            LeftStickVector = gamepad.leftStick.ReadValue();
            RightStickVector = gamepad.rightStick.ReadValue();

            switch (NowController)
            {
                case ControllerMode.ROOKIE:
                    Gamepad_RookieMode();
                    break;
                default:

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

            KeyBode_RookieMode();
        }

            return sendkey;
    }

    /// <summary>
    /// 右スティックの倒している方向を度数に変える処理.
    /// </summary>
    /// <returns></returns>
    public float SetRightStickAngle()
    {
        // Debug.Log(RightStickValue.magnitude);
        if (RightStickValue != Vector2.zero && RightStickValue.magnitude > 0.8f)
        {
            angle = Mathf.Atan2(RightStickValue.x, RightStickValue.y) * Mathf.Rad2Deg;
        }
        return angle;
        
    }

    /// <summary>
    /// 右スティックが入力されているか.
    /// </summary>
    /// <returns></returns>
    public bool isRightStickAngle()
    {
        if(RightStickValue.magnitude > 0.8f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// キーボードの押されたキーによって対応した値を入れる関数.
    /// </summary>
    private void KeyBode_RookieMode()
    {
        if (Space_Key)
        {
            sendkey = KeyList.FIRE;
        }
        else if(W_Key && A_Key)
        {
            sendkey = KeyList.WA;
        }
        else if (W_Key && D_Key)
        {
            sendkey = KeyList.WD;
        }
        else if (S_Key && A_Key)
        {
            sendkey = KeyList.SA;
        }
        else if (S_Key && D_Key)
        {
            sendkey = KeyList.SD;
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

    /// <summary>
    /// ゲームパッドの押されたボタンによって対応した値を入れる関数.
    /// </summary>
    private void Gamepad_RookieMode()
    {
        if (RightTrigger2 || ButtonEast)
        {
            sendkey = KeyList.FIRE;
        }
        else if (LeftTrigger2 || ButtonSouth)
        {
            sendkey = KeyList.PLANT;
        }
        else if (LeftStickUp && LeftStickLeft)
        {
            sendkey = KeyList.WA;
        }
        else if (LeftStickUp && LeftStickRight)
        {
            sendkey = KeyList.WD;
        }
        else if (LeftStickDown && LeftStickLeft)
        {
            sendkey = KeyList.SA;
        }
        else if (LeftStickDown && LeftStickRight)
        {
            sendkey = KeyList.SD;
        }
        else if (LeftStickUp)
        {
            sendkey = KeyList.W;
        }
        else if (LeftStickDown)
        {
            sendkey = KeyList.S;
        }
        else if (LeftStickLeft)
        {
            sendkey = KeyList.A;
        }
        else if (LeftStickRight)
        {
            sendkey = KeyList.D;
        }
        else
        {
            sendkey = KeyList.NONE;
        }

        if (RightStickUp)
        {
            RightStickSend = RightStickList.UP;
        }
        else if (RightStickDown)
        {
            RightStickSend = RightStickList.DOWN;
        }
        else if (RightStickLeft)
        {
            RightStickSend = RightStickList.LEFT;
        }
        else if (RightStickRight)
        {
            RightStickSend = RightStickList.RIGHT;
        }
        else
        {
            RightStickSend = RightStickList.NONE;
        }
    }
}
