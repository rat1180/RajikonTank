using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

/// <summary>
/// ���[�U�[��l�Ɋ��蓖�Ă���v���C���[�N���X
/// �^���N�ōs������ƃQ�[���}�l�[�W���[�Ƃ̋��n�����s���C���[�W
/// ���̃I�u�W�F�N�g�ɂ�PlayerInput�N���X�����蓖�Ă��A�^���N�͂�����Q�Ƃ��Ĉړ������s�����߁A
/// �ړ����̂��̂͂��̃N���X�ł͍s��Ȃ�
/// </summary>
public class PlayerClass : TankEventHandler
{

    #region �ϐ�
    [SerializeField, Tooltip("���[�U�[�����삷�邽�߂�InputClass")] private PlayerInput PlayerInputScript;

    [SerializeField,Tooltip("���̃N���X���������Ă���^���N"),Header("�f�o�b�O�p�\��")] private Rajikon PossessionTank;

    [SerializeField,Tooltip("�������t���邩")] private bool isControl;

    [SerializeField, Tooltip("�Q�[���}�l�[�W���[�̃C���X�^���X")] private GameManager GameManagerInstance;

    [SerializeField, Tooltip("�������Ă���`�[��ID")] private TeamID TeamID;

    [SerializeField, Tooltip("���̃v���C���[�̃X�|�[���|�C���g")] private Vector3 SpawnPoint;
    #endregion

    #region Unity�C�x���g
    // Start is called before the first frame update
    void Start()
    {
        //�v���C���[�̏�����
        InitPlayer();
        
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
    private void InitPlayer()
    {
        //�}�l�[�W���[�̎擾
        GameManagerInstance = GameManager.instance;

        //�e�X�g
        GameManager.instance.PushTeamList(TeamID);

        //���쌠���̏�����
        isControl = false;

        //�L�[���͎擾�p�X�N���v�g�m�F�E�ǉ�
        SetPlayerInputScript();

        //�e�X�g
        PopTank();

        //�G���[�m�F
        if (PossessionTank == null)
        {
            Debug.LogWarning("���������Ƀ^���N����������Ă��܂���");
        }

        //�`�[���֒ǉ�
        GameManagerInstance.PushTank(TeamID, PossessionTank);

        isControl = true;
        
    }

    /// <summary>
    /// PlayerInput���Z�b�g����Ă��邩���m�F���A
    /// ����Ă��Ȃ���΃Z�b�g����
    /// </summary>
    private void SetPlayerInputScript()
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
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.Rajikon);

        //�^���N�̏�����
        tank.GetComponent<Rajikon>().SetPlayerInput(PlayerInputScript);

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
        Vector3 mousepos = Input.mousePosition;
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
        //GameManagerInstance

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

    public void SetSpawnPoint(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;
    }

    public void SetisControl(bool iscontrol)
    {
        isControl = iscontrol;
    }

    #endregion
}
