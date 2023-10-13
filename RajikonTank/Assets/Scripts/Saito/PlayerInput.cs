using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    Vector2 LeftStickInput;
    Vector2 RightStickInput;

    public KeyList sendkey;  // �����ꂽ�L�[�̏��𑗂�ϐ�.
    public Vector3 sendtarget;   // �_���Ă���ꏊ�𑗂�ϐ�

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �^���N���_�����W
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 TargetPosition()
    {
        return sendtarget;
    }

    /// <summary>
    /// ���͂��ꂽ�����L�[
    /// </summary>
    /// <returns></returns>
    public virtual KeyList KeyInput()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        if(gamepad != null)
        {
            Debug.Log("�Q�[���p�b�h���ڑ�����܂���");
            LeftStickInput = gamepad.leftStick.ReadValue();
            RightStickInput = gamepad.rightStick.ReadValue();

            if (gamepad.rightTrigger.wasPressedThisFrame)
            {
                sendkey = KeyList.FIRE;
            }
            else if (LeftStickInput.magnitude <= -1.0f || RightStickInput.magnitude >= 1.0f)
            {
                sendkey = KeyList.LEFTROTATION;
            }
            else if (LeftStickInput.magnitude >= 1.0f || RightStickInput.magnitude <= -1.0f)
            {
                sendkey = KeyList.RIGHTROTATION;
            }
            else
            {
                sendkey = KeyList.NONE;
            }
        }
        
        if(keyboard != null)
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
