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

    [SerializeField] Controller NowController;

    Keyboard keyboard = Keyboard.current;
    Gamepad gamepad = Gamepad.current;
    float Threshold = 0.8f;  // 臒l�̐ݒ�.

    bool LeftStickLeft;    // ���X�e�B�b�N�����ɓ|������.
    bool LeftStickRight;   // ���X�e�B�b�N���E�ɓ|������.
    bool LeftStickUp;      // ���X�e�B�b�N��O�ɓ|������.
    bool LeftStickDown;    // ���X�e�B�b�N�����ɓ|������.

    bool RightStickUp;     // �E�X�e�B�b�N��O�ɓ|������.
    bool RightStickDown;   // �E�X�e�B�b�N�����ɓ|������.

    void Start()
    {
        
    }

    void Update()
    {
        InformationStick();
    }

    /// <summary>
    /// �X�e�B�b�N�̓|���Ă���������.
    /// </summary>
    void InformationStick()
    {
        LeftStickLeft = LeftStickInput.x <= -Threshold;   // ���X�e�B�b�N�����ɓ|������.
        LeftStickRight = LeftStickInput.x >= Threshold;   // ���X�e�B�b�N���E�ɓ|������.
        LeftStickUp = LeftStickInput.y >= Threshold;      // ���X�e�B�b�N��O�ɓ|������.
        LeftStickDown = LeftStickInput.y <= -Threshold;   // ���X�e�B�b�N�����ɓ|������.

        RightStickUp = RightStickInput.y >= Threshold;    // �E�X�e�B�b�N��O�ɓ|������.
        RightStickDown = RightStickInput.y <= -Threshold; // �E�X�e�B�b�N�����ɓ|������.
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
        if (gamepad != null)
        {
            Debug.Log("�Q�[���p�b�h���ڑ�����܂���");
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
