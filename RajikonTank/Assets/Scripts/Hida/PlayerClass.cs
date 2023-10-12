using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;
using UnityEngine.InputSystem;

/// <summary>
/// ���[�U�[��l�Ɋ��蓖�Ă���v���C���[�N���X
/// �^���N�ōs������ƃQ�[���}�l�[�W���[�Ƃ̋��n�����s���C���[�W
/// ���̃I�u�W�F�N�g�ɂ�PlayerInput�N���X�����蓖�Ă��A�^���N�͂�����Q�Ƃ��Ĉړ������s�����߁A
/// �ړ����̂��̂͂��̃N���X�ł͍s��Ȃ�
/// </summary>
public class PlayerClass : TankEventHandler
{

    #region �񋓑́E�\���́E�����N���X

    /// <summary>
    /// �������̃��[�h���w�肷��
    /// DEBUG�ȊO��PopTank���ĂԕK�v������
    /// �ʏ�̏�������DEFAULT
    /// �Q�[�����ɑg�ݍ��ޏ�������NATURAL
    /// �f�o�b�O�p�ɂƂ肠����������DEBUG
    /// </summary>
    public enum InitMode
    {
        /// <summary>
        /// �ʏ�̏����ŏ����������
        /// �����ʒu�F�ݒ肳��Ă�΂����ɁA����Ă��Ȃ����(0,0,0)�ɐ���
        /// ���N���X�ւ̈ˑ��F����
        /// </summary>
        DEFAULT,
        /// <summary>
        /// �X�e�[�W�ɑg�ݍ��߂��Ԃŏ����������
        /// �����ʒu�F�ݒ肷��΂��̈ʒu�ɁA����Ă��Ȃ���΂��̃I�u�W�F�N�g�̈ʒu�ɐ���
        /// ���N���X�ւ̈ˑ��F����
        /// </summary>
        NATURAL,
        /// <summary>
        /// �f�o�b�O�p�ɏ������ȈՓI�ɐ��������
        /// �����ʒu�F���̃I�u�W�F�N�g�̈ʒu
        /// ���N���X�ւ̈ˑ��F���Ȃ�(�������A�^���N�ƒe�̖��O���w�肵�Ȃ���΂����Ȃ�)
        /// </summary>
        DEBUG
    }

    /// <summary>
    /// �f�o�b�O���[�h�ł̏������p�ɐݒ肵�Ȃ���΂Ȃ�Ȃ��ݒ�
    /// ����ȊO�̃��[�h�ł͎g�p���Ȃ�
    /// </summary>
    [System.Serializable]
    public class DebugModeSettings
    {

    }

    #endregion

    #region �ϐ�

    #region �ύX�E�Z�b�g���K�{�̕ϐ�
    [SerializeField, Tooltip("���������[�h(���ɂȂ����NATURAL)"), Header("�ύX�E�Z�b�g���K�{�̕ϐ�")] public InitMode InitModeSelect;
    #endregion

    #region �����Ɏg���ϐ�
    [SerializeField, Tooltip("���[�U�[�����삷�邽�߂�InputClass"),Header("���̃N���X���ŏ����Ɏg���ϐ�")] private PlayerInput PlayerInputScript;

    [SerializeField, Tooltip("�Q�[���}�l�[�W���[�̃C���X�^���X")] private GameManager GameManagerInstance;

    [SerializeField, Tooltip("�ő�e��(�f�t�H���g�l��5)")] private int MaxBulletNm = 5;
    #endregion

    #region �f�o�b�O�p�\��
    [SerializeField,Tooltip("���̃N���X���������Ă���^���N"),Header("�f�o�b�O�p�\��")] private Rajikon PossessionTank;

    [SerializeField,Tooltip("�������t���邩")] private bool isControl;

    [SerializeField, Tooltip("�������Ă���`�[��ID")] private TeamID TeamID;

    [SerializeField, Tooltip("���̃v���C���[�̃X�|�[���|�C���g")] private Vector3 SpawnPoint;

    [SerializeField, Tooltip("�c�e��")] private int RemainingBulletNm;
    #endregion

    #endregion

    #region Unity�C�x���g
    // Start is called before the first frame update
    void Start()
    {
        if(InitModeSelect != InitMode.NATURAL)
        {
            //�v���C���[�̏������E�e�X�g
            InitPlayer(InitModeSelect);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�^���N���A�b�v�f�[�g����
        UpdateTank();
    }
    #endregion

    #region �������E�����֐�

    /// <summary>
    /// Player�N���X�̏��������s��
    /// �Q�[���}�l�[�W���[�ւ̃`�[���o�^�ƃ^���N�̐����A�^���N�̏�����
    /// </summary>
    public void InitPlayer(InitMode mode)
    {
        //���쌠���̏�����
        isControl = false;

        //�L�[���͎擾�p�X�N���v�g�m�F�E�ǉ�
        CheckPlayerInputScript();

        //���[�h�ɍ��킹������
        switch (mode)
        {
            case InitMode.DEFAULT:
                CheckGameManagerInstance();
                break;
            case InitMode.NATURAL:
                SetSpawnPoint(transform.position);
                CheckGameManagerInstance();
                PopTank();
                break;
            case InitMode.DEBUG:
                SetSpawnPoint(transform.position);

                //�e�X�g
                PopTank();
                break;
            default:
                break;
        }

        //�e�X�g
        //PopTank();

        //�G���[�m�F
        if (PossessionTank == null)
        {
            Debug.LogWarning("���������Ƀ^���N����������Ă��܂���");
        }

        //�e�X�g
        isControl = true;
    }

    /// <summary>
    /// �Q�[���}�l�[�W���[�C���X�^���X���m�F���A
    /// ����΃Z�b�g����
    /// </summary>
    private void CheckGameManagerInstance()
    {
        //�}�l�[�W���[�̎擾
        GameManagerInstance = GameManager.instance;
    }

    /// <summary>
    /// PlayerInput���Z�b�g����Ă��邩���m�F���A
    /// ����Ă��Ȃ���΃Z�b�g����
    /// </summary>
    private void CheckPlayerInputScript()
    {
        //�Z�b�g����Ă��邩�m�F
        if(PlayerInputScript == null)
        {
            //�����Ă���΂�����A���Ȃ���Βǉ����ăZ�b�g
            PlayerInputScript = gameObject.AddComponent<PlayerInput>();
        }
    }

    /// <summary>
    /// �^���N�𐶐�����
    /// </summary>
    /// <returns></returns>
    private GameObject TankSpawn()
    {
        //�^���N�̐���
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TankBase);

        //�^���N�̏�����
        tank.GetComponent<Rajikon>().SetPlayerInput(PlayerInputScript);
        //add.h
        tank.GetComponent<Rajikon>().SetEventHandler(this);
        RemainingBulletNm = MaxBulletNm;

        //�^���N�̏����ʒu��ݒ�
        tank.transform.position = SpawnPoint;

        //������e�ɐݒ�
        tank.transform.parent = gameObject.transform;

        return tank;
    }

    #endregion

    #region �^���N�Ɋւ��鏈��

    /// <summary>
    /// �^���N�ւ̑���̔��f���s���֐�
    /// isControl�ɂ���đ������t���邩�ǂ��������肷��
    /// </summary>
    private void UpdateTank()
    {
        if (isControl)
        {
            //������擾
            InputControler();
        }
    }

    /// <summary>
    /// �R���g���[���[�Ȃǂ̓��͂��擾����
    /// ����PC�ɂ���ĕ������ꂽ�I�u�W�F�N�g�Ȃ�A����PC�̑��������擾����
    /// </summary>
    private void InputControler()
    {
        //�}�E�X�̐��PlayerInput�ɔ��f����
        PlayerInputScript.sendtarget = GetMousePos();
    }

    /// <summary>
    /// �}�E�X���W���擾����
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMousePos()
    {
        Mouse mouse = Mouse.current;
        Vector3 mousepos = mouse.position.ReadValue();
        mousepos.z = 10;
        mousepos = Camera.main.ScreenToWorldPoint(mousepos);
        return mousepos;
    }

    /// <summary>
    /// �^���N�̃q�b�g���ɃC�x���g�n���h���[�o�R�ŌĂ΂��
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        //�Q�[���}�l�[�W���[�Ƀq�b�g�������Ƃ�ʒm
        GameManagerInstance.DeathTank(TeamID);

        //������~
        SetisControl(false);
    }

    #endregion

    #region GM�Ɋւ��鏈��

    /// <summary>
    /// ���̃v���C���[�p�̃^���N�𐶐�����
    /// �����ŏ����ʒu�t���A�w��Ȃ��̏ꍇ�͊��Ɏw�肳��Ă����ʒu�ɐ�������
    /// ����ɐ�������Ă����true���Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool PopTank()
    {
        if(SpawnPoint == new Vector3(0, 0, 0))
        {
            Debug.LogWarning("�����ʒu���w�肳��Ă��Ȃ����A0,0,0�ł�");
        }

        //�������Ă���^���N�ɑ��
        PossessionTank = TankSpawn().GetComponent<Rajikon>();

        if(PossessionTank == null)
        {
            Debug.LogWarning("�^���N����������Ă��܂���");
            return false;
        }

        //�`�[���֒ǉ�
        if(GameManagerInstance != null) GameManagerInstance.PushTank(TeamID, PossessionTank);
        return true;
    }

    /// <summary>
    /// ���̃v���C���[�p�̃^���N�𐶐�����
    /// �����ŏ����ʒu�t���A�w��Ȃ��̏ꍇ�͊��Ɏw�肳��Ă����ʒu�ɐ�������
    /// ����ɐ�������Ă����true���Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool PopTank(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;

        if (SpawnPoint == new Vector3(0, 0, 0))
        {
            Debug.LogWarning("�����ʒu���w�肳��Ă��Ȃ����A0,0,0�ł�");
        }

        //�������Ă���^���N�ɑ��
        PossessionTank = TankSpawn().GetComponent<Rajikon>();

        if (PossessionTank == null)
        {
            Debug.LogWarning("�^���N����������Ă��܂���");
            return false;
        }
        return true;
    }

    #region �Z�b�^�[�E�Q�b�^�[

    /// <summary>
    /// �c��c�e�����擾����
    /// �f�t�H���g�̒l��5�ł���A�����тɌ����Ă���̂�
    /// �c�e���̕\���Ȃǂ͐����擾����
    /// </summary>
    /// <returns></returns>
    public int GetBulletCount()
    {
        return RemainingBulletNm;
    }

    /// <summary>
    /// �X�|�[������|�W�V�����̐ݒ�
    /// </summary>
    /// <param name="spawnpoint"></param>
    public void SetSpawnPoint(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;
    }

    public void SetisControl(bool iscontrol)
    {
        isControl = iscontrol;
    }

    #endregion

    #endregion
}
