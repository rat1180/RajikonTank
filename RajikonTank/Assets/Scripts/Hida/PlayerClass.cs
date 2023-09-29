using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class PlayerClass : MonoBehaviour
{
    
    [SerializeField,Tooltip("���̃N���X���������Ă���^���N"),Header("�f�o�b�O�p�\��")] private GameObject PossessionTank;

    [SerializeField,Tooltip("�������t���邩")] private bool isControl;

    [SerializeField, Tooltip("�Q�[���}�l�[�W���[�̃C���X�^���X")] private GameObject GameManagerInstance;

    [SerializeField, Tooltip("�������Ă���`�[��ID")] private int TeamID;

    [SerializeField, Tooltip("���̃v���C���[�̃X�|�[���|�C���g")] private Vector3 SpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        //�v���C���[�̏�����
        InitPlayer();

        //�e�X�g
        PopTank();
    }

    // Update is called once per frame
    void Update()
    {
        //�^���N���A�b�v�f�[�g����
        UpdateTank();
    }

    #region �������E�����֐�

    /// <summary>
    /// Player�N���X�̏��������s��
    /// �Q�[���}�l�[�W���[�ւ̃`�[���o�^�ƃ^���N�̐����A�^���N�̏�����
    /// </summary>
    private void InitPlayer()
    {
        //�}�l�[�W���[�̎擾
        //GameManagerInstance = ;

        //���쌠���̏�����
        isControl = false;

        //�G���[�m�F
        if(PossessionTank == null)
        {
            Debug.LogWarning("�^���N����������Ă��܂���");
        }

        //�`�[���֒ǉ�
        //GameManagerInstance.AddTeam();
        
    }

    private GameObject TankSpawn()
    {
        //�^���N�̐���
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_TEST);

        //�^���N�̏�����


        //�^���N�̏����ʒu��ݒ�
        tank.transform.position = SpawnPoint;

        //������e�ɐݒ�
        tank.transform.parent = gameObject.transform;

        return tank;
    }

    #endregion

    #region �^���N�Ɋւ��鏈��

    /// <summary>
    /// �^���N�̈ړ����s���֐�
    /// isControl�ɂ���đ������t���邩�ǂ��������肷��
    /// </summary>
    private void UpdateTank()
    {
        //���e���V������̂��p�N���\��
        if (isControl)
        {
            //������擾
            InputControler();

            //����𔽉f
            TankMove(new Vector3());
        }
    }

    /// <summary>
    /// �R���g���[���[�Ȃǂ̓��͂��擾����
    /// ����PC�ɂ���ĕ������ꂽ�I�u�W�F�N�g�Ȃ�A����PC�̑��������擾����
    /// </summary>
    private void InputControler()
    {
        //���e���V������̂��p�N��
    }

    /// <summary>
    /// �擾������������L���Ă���^���N�ɔ��f����
    /// </summary>
    private void TankMove(Vector3 input)
    {
        //�^���N�ɔ��f
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
        PossessionTank = TankSpawn();

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
        PossessionTank = TankSpawn();

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
