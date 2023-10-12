using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] InputAction InputLeftCaterpillarMove;
    [SerializeField] InputAction InputRightCaterpillarMove;
    Vector2 LeftStickInput;
    Vector2 RightStickInput;

    public KeyList sendkey;  // 押されたキーの情報を送る変数.
    public Vector3 sendtarget;   // 狙っている場所を送る変数

    private void OnDisable()
    {
        InputLeftCaterpillarMove.Disable();
    }

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

        if(gamepad != null)
        {
            LeftStickInput = gamepad.leftStick.ReadValue();
            RightStickInput = gamepad.rightStick.ReadValue();
        }

        if (keyboard.aKey.isPressed)
        {
            sendkey = KeyList.A;
        }
        else if (keyboard.dKey.isPressed)
        {
            sendkey = KeyList.D;
        }
        else if (keyboard.sKey.isPressed || LeftStickInput.magnitude <= -1.0f && gamepad != null)
        {
            sendkey = KeyList.S;
        }
        else if (keyboard.wKey.isPressed || LeftStickInput.magnitude >= 1.0f && gamepad != null)
        {
            sendkey = KeyList.W;
        }
        else if (keyboard.upArrowKey.isPressed || RightStickInput.magnitude >= 1.0f && gamepad != null)
        {
            sendkey = KeyList.UPARROW;
        }
        else if (keyboard.rightArrowKey.isPressed)
        {
            sendkey = KeyList.RIGHTARROW;
        }
        else if (keyboard.leftArrowKey.isPressed)
        {
            sendkey = KeyList.LEFTARROW;
        }
        else if (keyboard.downArrowKey.isPressed || RightStickInput.magnitude <= -1.0f && gamepad != null)
        {
            sendkey = KeyList.DOWNARROW;
        }
        else if (keyboard.spaceKey.wasPressedThisFrame || gamepad.rightTrigger.wasPressedThisFrame)
        {
            sendkey = KeyList.FIRE;
        }
        else
        {
            sendkey = KeyList.NONE;
        }

        return sendkey;

    }
}
