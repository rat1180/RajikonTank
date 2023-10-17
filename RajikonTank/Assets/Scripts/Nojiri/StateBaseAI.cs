using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;   //�G�����̐ݒ�
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT; //�G�̏����J��

    private Rajikon rajikon;        // Rajikon�N���X
    private CPUInput cpuInput;      // CPUInput�N���X
    private GameObject player;      // �v���C���[���
    private GameObject enemy;       // �G�l�~�[���
    private GameObject grandChild;  // ���I�u�W�F�N�g
    private Vector3 enemyPos;       // �G(����)�̈ʒu
    private Vector3 playerPos;      // �v���C���[�̈ʒu
    private string playerTag;       // Player��tag
    private bool isInit   = false;  // ��������Ԋm�F
    private bool isAttack = false;  // �U���Ԋu�p�t���O
    private bool isTimer  = false;  // �^�C�}�[�t���O
    private const int forwardAngle  = 20;  // �O���p�x
    private const int backwardAngle = 160; // ����p�x

    Quaternion lookAngle; // �e�X�g
    public enum EnemyName // �G���
    {
        NORMAL,              // �ʏ�G
        MOVEMENT,                // �ړ��G
        FASTBULLET,          // �����e�G
        FASTANDMOVE          // �����e�ƈړ��G
    }

    public enum EnemyAiState // �s���p�^�[��
    {
        WAIT,                // �ҋ@
        MOVE,                // �ړ�
        TURN,                // ����
        ATTACK,              // �U��
        AVOID,               // ���
        DEATH,               // ���S
    }

    // Start is called before the first frame update
    void Start()
    {
        // PlayerInput�N���X�擾
        rajikon = gameObject.GetComponent<Rajikon>();
        cpuInput = gameObject.GetComponent<CPUInput>();

        // �Q�[���I�u�W�F�N�g�Ő������ꂽPlayer���擾
        player = GameManager.instance.teamInfo[GameManager.instance.player_IDnum].tankList[0].gameObject.transform.Find("Tank").gameObject;
        playerTag = player.tag; // tag���擾

        enemy = rajikon.Tank.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
        var relative = playerPos - enemyPos;
        lookAngle = Quaternion.LookRotation(relative.normalized);  // Player�Ɍ��������̊p�x
        Debug.Log("Y  " + lookAngle.y);
    }

    #region ���O�J��/���C�����[�v
    /// <summary>
    /// �G�̎�ނɂ���čs���J�ڂ�ύX
    /// </summary>
    private void UpdateAI()
    {
        InitAI();

        // INGAME�łȂ��Ƃ��A�����������ȊO���J�n���Ȃ�
        if (GameManager.instance.NowGameState != GAMESTATUS.INGAME)
        {
            return;
        }

        switch (aiName)
        {
            case EnemyName.NORMAL:
                NormalEnemy();
                break;
            case EnemyName.MOVEMENT:
                MoveEnemy();
                break;
            case EnemyName.FASTBULLET:
                //FastBulletEnemy();
                break;
            case EnemyName.FASTANDMOVE:
                //FastAndMoveEnemy();
                break;
            default:
                break;
        }
    }
    #endregion

    #region ����������
    /// <summary>
    /// ���������s
    /// �`�[��ID�A�G����
    /// </summary>
    private void InitAI()
    {
        enemyPos = enemy.transform.position;
        playerPos = player.transform.position;

        if (isInit == true)
        {
            return;
        }

        isInit = true;

        rajikon.SetPlayerInput(cpuInput);
        rajikon.SetEventHandler(this); // �^���N�̃C�x���g��ʒm����

        AddTeam(); // �`�[���ǉ�
    }

    /// <summary>
    /// �`�[��ID���M
    /// </summary>
    private  void AddTeam()
    {
        // CPU�̃`�[��ID�𑗂��ă`�[���ɒǉ�
        GameManager.instance.PushTank(TeamID.CPU, rajikon);
    }
    #endregion

    #region �s���J��/AI���C�����[�`��
    /// <summary>
    /// �S�Ă̓G�Ŏg�p�����ԑJ��
    /// </summary>
    private void AiMainRoutine()
    {
        switch (aiState)
        {
            case EnemyAiState.WAIT:
                Wait();
                break;
            case EnemyAiState.MOVE:
                Move();
                break;
            case EnemyAiState.TURN:
                Turn();
                break;
            case EnemyAiState.ATTACK:
                Attack();
                break;
            case EnemyAiState.AVOID:
                break;
            case EnemyAiState.DEATH:
                EnemyDeath();
                break;
            default:
                break;
        }
    }

    #endregion

    #region �ʏ�G�J��
    /// <summary>
    /// �ʏ�G���[�v
    /// </summary>
    private void NormalEnemy()
    {
        NormalEnemyRoutine(); // �ʏ�G�@�\
        AiMainRoutine();   // �s���J��
    }

    /// <summary>
    /// Ray�ɐG�ꂽ�I�u�W�F�N�g�ɂ��State�̊��蓖��
    /// Player�̏ꍇ�@�F�U��
    /// ����ȊO�̏ꍇ�F�ړ�or����
    /// </summary>
    private void NormalEnemyRoutine()
    {
        //GameManager.instance.id = CPU_ID.Normal; // ID�ݒ�K��

        // AiTimer���s��
        if (isTimer == true || aiState == EnemyAiState.DEATH)
        {
            return;
        }

        RaycastHit hit;         // Ray���Փ˂����I�u�W�F�N�g���
        Vector3 fireDirection;  // ���˕���  
        bool attackFlg;         // �U������t���O

        fireDirection = (playerPos - enemyPos).normalized;

        // Ray���΂�����(���ˈʒu, ����, �Փ˂����I�u�W�F�N�g���, ����(�L�ڂȂ��F����))
        if (Physics.Raycast(enemyPos, fireDirection, out hit, 50f))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit�^����GameObject�^�֕ϊ�

            if (hitObj.tag == playerTag && hitObj == player) // Player�Ǝ����̊ԂɎՕ������Ȃ��Ƃ�
            {
                // �ړ��G���v���C���[�Ƃ̋���������Ă���Ƃ�
                //if(aiName == EnemyName.MOVEMENT)
                //{
                //    aiState = EnemyAiState.MOVE;
                //}

                attackFlg = TurretDirection(); // �C�䂪Player�Ɍ����Ă��邩�ǂ���

                if (attackFlg) aiState = EnemyAiState.ATTACK; // true �F�U��
                else aiState = EnemyAiState.TURN;             // false�F����
            }
            else
            {
                aiState = EnemyAiState.WAIT;   // �ҋ@
            }
        }
        else
        {
            aiState = EnemyAiState.WAIT;   // �ҋ@
            Debug.LogError("Ray��������܂���ł����B");
        }

        StartCoroutine(AiTimer());
    }

    /// <summary>
    /// �C��̌������Q�Ƃ��A�U�������Ԃ�
    /// Player�Ɍ����Ă���Ƃ��Fture
    /// ����ȊO�@�@�@�@�@�@�@�Ffalse
    /// </summary>
    private bool TurretDirection()
    {
        grandChild = transform.GetChild(0).GetChild(1).gameObject;   // ���Ԃ���Turret�擾
        RaycastHit turretHit;   // ���C�ɏՓ˂����I�u�W�F�N�g���

        if (grandChild == null) return false;

        // �C��̑O����Ray�𔭎�
        if (Physics.SphereCast(enemyPos, 0.5f, grandChild.transform.forward, out turretHit, 50f))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            if (turretHitObj.tag == playerTag && turretHitObj == player)
            {
                // �U����
                return true;
            }
            else
            {
                // �U���s��
                return false;
            }
        }
        return false;
    }
    #endregion

    #region �ړ��G�J��
    /// <summary>
    /// �ړ��G���[�v
    /// </summary>
    private void MoveEnemy()
    {
        MoveEnemyRoutine(); // �ړ��G�@�\
        AiMainRoutine();   // �s���J��
    }

    private void MoveEnemyRoutine()
    {

    }
    #endregion

    #region �e��^�C�}�[
    // ���t���[�������ɂ�镉�ׂ̌y���p�^�C�}�[
    private IEnumerator AiTimer()
    {
        isTimer = true;
        yield return new WaitForSeconds(1); // 1�b���ƂɎ��s

        isTimer = false;
    }

    // �w��b�����ƂɍU�����������s����^�C�}�[
    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(5);
        isAttack = false;
    }
    #endregion

    #region �s���J�ڂ��Ƃ̃��\�b�h
    /// <summary>
    /// �ҋ@����
    /// �ϐ��̏�����
    /// </summary>
    private void Wait()
    {
        cpuInput.moveveckey = KeyList.NONE;
    }

    /// <summary>
    /// �ړ��p���\�b�h
    /// Player�Ƃ̈ʒu�֌W���v�Z
    /// �L�[�{�[�h�����ɕϊ�����Player��Ǐ]
    /// </summary>
    private void Move()
    {
        float enemyMovePos = VectorCalc(); // Player�܂ł̊p�x���󂯎��

        ConversionKey(enemyMovePos); // �L�[�{�[�h�ϊ�
    }

    /// <summary>
    /// �U���p���\�b�h
    /// �e�𔭎ˌ�́A5�b�Ԓe��łĂȂ�����
    /// </summary>
    private void Attack()
    {
        cpuInput.moveveckey = KeyList.NONE; // �L�[�{�[�h���͂�������

        if (!isAttack)
        {
            isAttack = true; // �U����
            cpuInput.moveveckey = KeyList.FIRE; // �U���{�^������

            StartCoroutine(AttackTimer()); // �w��b����ɏ������ĊJ
        }
    }

    // ����p���\�b�h
    private void Turn()
    {
        cpuInput.sendtarget = playerPos; // Player�̕���������
    }

    /// <summary>
    /// ���S�p���\�b�h
    /// ���S������GameManager�ɑ��M���Destroy����
    /// </summary>
    private void EnemyDeath()
    {
        GameManager.instance.DeathTank(TeamID.CPU); // GameManager�Ɏ��S�������M

        Destroy(gameObject); // �G�폜
    }

    /// <summary>
    /// bullet�ɓ����������ɌĂ΂�郁�\�b�h
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        aiState = EnemyAiState.DEATH;  // ���S�J�ڂɈڍs
    }
    #endregion

    #region �x�N�g���ϊ�
    /// <summary>
    /// Enemy����Player�̂���p�x���v�Z
    /// ���߂��p�x��8�����ɕ������AVector2(-1�`1, -1�`1)�ɕϊ�
    /// </summary>
    private float VectorCalc()
    {
        Vector3 relativePos = playerPos - enemyPos; // Enemy����Player�ւ̑��΃x�N�g��
        //lookAngle = Quaternion.LookRotation(relativePos.normalized);  // Player�Ɍ��������̊p�x
        float rotateAngle = Quaternion.Angle(enemy.transform.localRotation,lookAngle); // ���݂̊p�x����Player�܂ł̊Ԃ̊p�x

        return rotateAngle; // Player�܂ł̊p�x��Ԃ�
    }

    /// <summary>
    /// �L�[�{�[�h�ϊ����\�b�h
    /// �󂯎�����p�x�̑傫���ɉ����ăL�[�{�[�h�ϊ�
    /// </summary>
    /// <param name="index">���݂̊p�x����Player�܂ł̊p�x</param>
    private void ConversionKey(float index)
    {
        if (index < forwardAngle) // Player���O���p�x���ɂ���Ƃ�
        {
            cpuInput.moveveckey = KeyList.ACCELE;
        }
        else if(index >= forwardAngle && index <= backwardAngle) // Player�������p�x���ɂ���Ƃ�
        {
            // ���ɂ���Ƃ�
            if (lookAngle.y < 0 && lookAngle.y < -0.5f)
            {
                // �O������̋߂��ɂ���ĉ�������ς���
                if (lookAngle.y > -0.5f)
                {
                    cpuInput.moveveckey = KeyList.LEFTROTATION;
                    Debug.Log("���O��");
                }
                else if(lookAngle.y < -0.5f)
                {
                    cpuInput.moveveckey = KeyList.RIGHTROTATION;
                    Debug.Log("�����");
                }
            }

            // �E�ɂ���Ƃ�
            if (lookAngle.y > 0)
            {
                // �O������̋߂��ɂ���ĉ�������ς���
                if (lookAngle.y < 0.5f)
                {
                    cpuInput.moveveckey = KeyList.RIGHTROTATION;
                    Debug.Log("�E�O��");
                }
                else if(lookAngle.y >= 0.5f)
                {
                    cpuInput.moveveckey = KeyList.LEFTROTATION;
                    Debug.Log("�E���");
                }
            }
        }
        else if(index > backwardAngle) // Player������p�x���ɂ���Ƃ�
        {
            cpuInput.moveveckey = KeyList.BACK;
        }
        else
        {
            cpuInput.moveveckey = KeyList.NONE;
        }
    }
    #endregion
}