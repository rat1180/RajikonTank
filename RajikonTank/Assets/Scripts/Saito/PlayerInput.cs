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

    public KeyList sendkey;        // �����ꂽ�L�[�̏��𑗂�ϐ�.
    public RightStickList RightStickSend; // �E�X�e�B�b�N�̏��𑗂�ϐ�.
    public Vector3 sendtarget;     // �_���Ă���ꏊ�𑗂�ϐ�.

    [SerializeField] ControllerMode NowController;

    private Keyboard keyboard = Keyboard.current;
    private Gamepad gamepad = Gamepad.current;

    private float Threshold = 0.1f;  // 臒l�̐ݒ�.
    protected float angle;

    private bool W_Key;
    private bool S_Key;
    private bool A_Key;
    private bool D_Key;
    private bool Up_Key;
    private bool Down_Key;

    private bool Space_Key;

    private bool LeftStickLeft;    // ���X�e�B�b�N�����ɓ|������.
    private bool LeftStickRight;   // ���X�e�B�b�N���E�ɓ|������.
    private bool LeftStickUp;      // ���X�e�B�b�N��O�ɓ|������.
    private bool LeftStickDown;    // ���X�e�B�b�N�����ɓ|������.

    private bool RightStickLeft;   // �E�X�e�B�b�N�����ɓ|������.
    private bool RightStickRight;  // �E�X�e�B�b�N���E�ɓ|������.
    private bool RightStickUp;     // �E�X�e�B�b�N��O�ɓ|������.
    private bool RightStickDown;   // �E�X�e�B�b�N�����ɓ|������.

    private bool LeftTrigger2;     // ���g���K�[�̉�. 
    private bool RightTrigger2;    // �E�g���K�[�̉�.
    private bool ButtonEast;       // ��(��)�̃{�^������������.
    private bool ButtonSouth;      // ��(�~)�̃{�^������������.

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
    /// �X�e�B�b�N�̓|���Ă���������.
    /// </summary>
    private void InformationStick()
    {
        LeftStickLeft = LeftStickVector.x <= -Threshold;   // ���X�e�B�b�N�����ɓ|������.
        LeftStickRight = LeftStickVector.x >= Threshold;   // ���X�e�B�b�N���E�ɓ|������.
        LeftStickUp = LeftStickVector.y >= Threshold;      // ���X�e�B�b�N��O�ɓ|������.
        LeftStickDown = LeftStickVector.y <= -Threshold;   // ���X�e�B�b�N�����ɓ|������.

        RightStickLeft = RightStickVector.x <= -Threshold; // �E�X�e�B�b�N�����ɓ|������.
        RightStickRight = RightStickVector.x >= Threshold; // �E�X�e�B�b�N���E�ɓ|������.
        RightStickUp = RightStickVector.y >= Threshold;    // �E�X�e�B�b�N��O�ɓ|������.
        RightStickDown = RightStickVector.y <= -Threshold; // �E�X�e�B�b�N�����ɓ|������.

        LeftTrigger2 = gamepad.leftTrigger.wasPressedThisFrame;
        RightTrigger2 = gamepad.rightTrigger.wasPressedThisFrame;
        ButtonEast = gamepad.buttonEast.wasPressedThisFrame;
        ButtonSouth = gamepad.buttonSouth.wasPressedThisFrame;
    }

    /// <summary>
    /// �^���N���_�����W
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 TargetPosition()
    {
        if (GameManager.instance.NowGameState != GAMESTATUS.INGAME) return new Vector3(0,0,0);

        return sendtarget;
    }

    /// <summary>
    /// ���͂��ꂽ�����L�[
    /// </summary>
    /// <returns></returns>
    public virtual KeyList KeyInput()
    {
        if(GameManager.instance.NowGameState != GAMESTATUS.INGAME) return KeyList.NONE;

        if (gamepad != null)
        {
            Debug.Log("�Q�[���p�b�h���ڑ�����܂���");
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
    /// �E�X�e�B�b�N�̓|���Ă��������x���ɕς��鏈��.
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
    /// �E�X�e�B�b�N�����͂���Ă��邩.
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
    /// �L�[�{�[�h�̉����ꂽ�L�[�ɂ���đΉ������l������֐�.
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
    /// �Q�[���p�b�h�̉����ꂽ�{�^���ɂ���đΉ������l������֐�.
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
