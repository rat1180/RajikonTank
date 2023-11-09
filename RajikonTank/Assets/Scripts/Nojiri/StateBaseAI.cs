using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;   //�G�����̐ݒ�
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT;   //�G�̏����J��
    public int maxDistance; // �U���\�͈�
    public Vector3[] movePos;
    public bool patrolMode = false;   // ���񃂁[�hON/OFF

    private Rajikon rajikon;          // Rajikon�N���X
    private CPUInput cpuInput;        // CPUInput�N���X
    private GameObject player;        // �v���C���[���
    private GameObject enemy;         // �G�l�~�[���
    private GameObject grandChild;    // ���I�u�W�F�N�g

    private Vector3 enemyPos;         // �G(����)�̈ʒu
    private Vector3 playerPos;        // �v���C���[�̈ʒu
    private Vector3[] patrolPos;      // ���񂷂�ʒu

    private string playerTag;         // Player��tag
    private float nowDistance;        // �v���C���[�Ƃ̋���
    private bool isInit   = false;    // ��������Ԋm�F
    private bool isAttack = false;    // �U���Ԋu�p�t���O
    private bool isTimer  = false;    // �^�C�}�[�t���O
    private bool canAttack = true;    // �U���ۃt���O
    private const int rayLength = 50; // Ray�̒���
    private int patrolPoint = 0;      // ����n�_�̔ԍ�

    public enum EnemyAiState // �s���p�^�[��
    {
        WAIT,                // �ҋ@
        PATROL,              // ����
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
        playerTag = player.tag; // Tag�擾

        enemy = rajikon.Tank.transform.gameObject; // �G���擾
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
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

        EnemyMain();             // �G���ʏ���
        EnemyStateTransition();  // �G��ԑJ��
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

    #region �G���ʏ���
    /// <summary>
    /// �G���ʃ��\�b�h
    /// Ray�ɐG�ꂽ�I�u�W�F�N�g�ɂ��State�̊��蓖��
    /// Player�̏ꍇ�@�F�G���O���Ƃɏ����̕ύX
    /// ����ȊO�̏ꍇ�F�ҋ@
    /// </summary>
    private void EnemyMain()
    {
        RaycastHit hit;         // Ray���Փ˂����I�u�W�F�N�g���
        Vector3 fireDirection;  // ���˕���  
        bool isFacingPlayer;    //�@Player�������Ă��邩�ǂ���

        nowDistance = Vector3.Distance(playerPos, enemyPos); // �v���C���[�Ƃ̋���

        // AiTimer���s��
        if (isTimer == true || aiState == EnemyAiState.DEATH)
        {
            return;
        }

        // �^�C�}�[���s
        StartCoroutine(AiTimer());

        fireDirection = (playerPos - enemyPos).normalized;

        // Ray���΂�����(���ˈʒu, ����, �Փ˂����I�u�W�F�N�g���, ����(�L�ڂȂ��F����))
        if (Physics.Raycast(enemyPos, fireDirection, out hit, rayLength))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit�^����GameObject�^�֕ϊ�

            if (hitObj.tag == playerTag && hitObj == player) // Player�Ǝ����̊ԂɎՕ������Ȃ��Ƃ�
            {
                EnemyNameTransition(); // �G�̖��O�ɂ���ď�����ύX

                // false�̏ꍇ�A�U���J�ڂֈڍs���Ȃ�
                if (!canAttack)
                {
                    return;
                }

                isFacingPlayer = TurretDirection(); // �C�䂪Player�Ɍ����Ă��邩�ǂ���

                if (isFacingPlayer) aiState = EnemyAiState.ATTACK; // true �F�U��
                else aiState = EnemyAiState.TURN;             // false�F����
            }
            else
            {
                // patrolMode��ON�̎��A�G�����񂳂���
                // ����n�_�ɓ��������玟�̏���n�_�ɖڕW��ς���
                if (patrolMode)
                {
                    aiState = EnemyAiState.PATROL;
                }
                else
                {
                    aiState = EnemyAiState.WAIT;   // �ҋ@
                }
            }
        }
        else
        {
            aiState = EnemyAiState.WAIT;   // �ҋ@
            Debug.LogError("Ray��������܂���ł����B");
        }
    }

    /// <summary>
    /// �G�̖��O�ɂ���čs���J�ڂ�ύX
    /// </summary>
    private void EnemyNameTransition()
    {
        switch (aiName)
        {
            case EnemyName.TUTORIAL:
                TutorialEnemy();
                break;
            case EnemyName.NORMAL:
                NormalEnemy();
                break;
            case EnemyName.MOVEMENT:
                MoveEnemy();
                break;
            case EnemyName.FASTBULLET:
                FastBulletEnemy();
                break;
            case EnemyName.FASTANDMOVE:
                FastAndMoveEnemy();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �C��̌������Q�Ƃ��A�U�������Ԃ�
    /// Player�Ɍ����Ă���Ƃ��Fture
    /// ����ȊO�@�@�@�@�@�@�@�Ffalse
    /// </summary>
    private bool TurretDirection()
    {
        const float radius = 0.5f; //���˂���Ray�̔��a

        grandChild = transform.GetChild(0).GetChild(1).gameObject;   // ���Ԃ���Turret�擾
        RaycastHit turretHit;   // ���C�ɏՓ˂����I�u�W�F�N�g���

        if (grandChild == null) return false;

        // �C��̑O����Ray�𔭎�
        if (Physics.SphereCast(enemyPos, radius, grandChild.transform.forward, out turretHit, rayLength))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            // �v���C���[�������Ă��邩�ǂ���
            if (turretHitObj.tag == playerTag && turretHitObj == player)
            {
                // �����Ă���
                return true;
            }
            else
            {
                // �����Ă��Ȃ�
                return false;
            }
        }
        return false;
    }
    #endregion

    #region �G��ԑJ��
    /// <summary>
    /// �S�Ă̓G�Ŏg�p�����ԑJ��
    /// </summary>
    private void EnemyStateTransition()
    {
        switch (aiState)
        {
            case EnemyAiState.WAIT:
                Wait();
                break;
            case EnemyAiState.PATROL:
                Patrol();
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

    #region �G���Ƃ̃��\�b�h

    #region �`���[�g���A���G�J��
    private void TutorialEnemy()
    {
        aiName = EnemyName.TUTORIAL; // GameManager�ɑ���ID�ݒ�

        aiState = EnemyAiState.WAIT; // �ҋ@
        canAttack = false; // �U���s��
    }
    #endregion

    #region �ʏ�G�J��
    /// <summary>
    /// �ʏ�G�ݒ�
    /// </summary>
    private void NormalEnemy()
    {
        aiName = EnemyName.NORMAL; // GameManager�ɑ���ID�ݒ�

        canAttack = true; // �U����
    }
    #endregion

    #region �ړ��G�J��
    /// <summary>
    /// �ړ��G�ݒ�
    /// </summary>
    private void MoveEnemy()
    {
        const int maxDistance = 10; // �U���\�͈�

        aiName = EnemyName.MOVEMENT; // GameManager�ɑ���ID�ݒ�
        aiState = EnemyAiState.MOVE; // �ړ�

        // �v���C���[�Ƃ̋��������l�ȏ�̎�
        if (nowDistance > maxDistance)
        {
            canAttack = false; // �U���s��
        }
        else
        {
            canAttack = true; // �U����
        }
    }
    #endregion

    #region �����e�G
    /// <summary>
    /// �����e�G�ݒ�
    /// </summary>
    private void FastBulletEnemy()
    {
        aiName = EnemyName.FASTBULLET; // GameManager�ɑ���ID�ݒ�

        canAttack = true; // �U����
    }
    #endregion

    #region �����e���ړ��G
    /// <summary>
    /// �ړ��������e�G�ݒ�
    /// </summary>
    private void FastAndMoveEnemy()
    { 

        aiName = EnemyName.FASTANDMOVE; // GameManager�ɑ���ID�ݒ�
        aiState = EnemyAiState.MOVE; // �ړ�

        // �v���C���[�Ƃ̋��������l�ȏ�̎�
        if (nowDistance > maxDistance)
        {
            canAttack = false; // �U���s��
        }
        else
        {
            canAttack = true; // �U����
        }
    }
    #endregion

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
        float second; // ���ˊԊu

        // �����e�����G�̏ꍇ�A���ˊԊu��ύX
        if (aiName == EnemyName.FASTBULLET || aiName == EnemyName.FASTANDMOVE)
        {
            second = Random.Range(3, 5);
        }
        else
        {
            second = Random.Range(1,3);
        }
            yield return new WaitForSeconds(second);
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
        cpuInput.moveveckey = KeyList.NONE; // �������
    }

    /// <summary>
    /// ����p���\�b�h
    /// patrolMode��True�̎��ɌĂ΂��
    /// �z����̍��W�Ɍ������ď��ԂɈړ�����
    /// </summary>
    private void Patrol()
    {
        float minDistance = 1.5f;

        // ����n�_�擾
        patrolPos = EnemyManager.instance.PatrolPositionGet();

        // �z��ɗv�f�������Ă��Ȃ���
        if (patrolPos == null)
        {
            Debug.LogError("patrolPos�ɗv�f�������Ă��܂���");
            return;
        }

        // ����n�_�擾
        patrolPos = EnemyManager.instance.PatrolPositionGet();

        // Null���������z��̗v�f��
        int maxArray = patrolPos.Length - 1;

        // �w��̕���������
        cpuInput.sendtarget = patrolPos[patrolPoint];

        // patrolPos�ɑ΂��Ă�8���������p�x�擾
        int patorolPosDiv = VectorConversion(patrolPos[patrolPoint]);

        // �ړ�����
        ConversionKey(patorolPosDiv);

        // 2�_�Ԃ̋����v�Z
        float pos = Vector3.Distance(patrolPos[patrolPoint], enemyPos);

        // ����n�_�ɐڋ߂������A���̏���n�_�ɕύX
        if(pos < minDistance)
        {
            // ����n�_�����I��������A���߂̏���n�_�ɖ߂�
            if(patrolPoint < maxArray)
            {
                patrolPoint++;
            }
            else
            {
                patrolPoint = 0;
            }
        }
    }

    /// <summary>
    /// �ړ��p���\�b�h
    /// Player�Ƃ̈ʒu�֌W���v�Z
    /// �L�[�{�[�h�����ɕϊ�����Player��Ǐ]
    /// </summary>
    private void Move()
    {
        Debug.Log("Move���s");
        int enemyMovePos = VectorConversion(playerPos); // Player�܂ł̊p�x���󂯎��

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
        // GameManager�Ɏ��S�������M
        GameManager.instance.DeathTank(TeamID.CPU,aiName);

        Destroy(gameObject);
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
    /// �x�N�g���ϊ����\�b�h
    /// �G����Target�ւ̊p�x��0�`7�ɕϊ�
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    private int VectorConversion(Vector3 Target)
    {
        // Enemy����Target�ւ̑��΃x�N�g���擾
        float vectorX = Target.x - enemyPos.x;
        float vectorZ = Target.z - enemyPos.z;

        // �G����Target�ɑ΂��Ă̊p�x
        float Radian = Mathf.Atan2(vectorZ, vectorX) * Mathf.Rad2Deg;

        // �v�Z�ŏo���p�x��-180�`180�Ȃ��߁A0�`360�ɕϊ�
        if (Radian < 0)
        {
            Radian += 360;
        }

        // 360�x��8����(���ۂ�0�`8��9����)
        int Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8��0����邽�ߏC��
        if (Division == 8) Division = 0;

        return Division;
    }

    /// <summary>
    /// �L�[�{�[�h�ϊ����\�b�h
    /// �󂯎�����p�x�ɉ����ăL�[�{�[�h�ϊ�
    /// </summary>
    /// <param name="index">���݂̊p�x����Target�܂ł̊p�x���󂯎��</param>
    private void ConversionKey(int index)
    {
        switch (index)
        {
            case 0:
                cpuInput.moveveckey = KeyList.D;   // �E
                break;
            case 1:
                cpuInput.moveveckey = KeyList.WD;  // �E��
                break;
            case 2:
                cpuInput.moveveckey = KeyList.W;   // ��
                break;
            case 3:
                cpuInput.moveveckey = KeyList.WA;  // ����
                break;
            case 4:
                cpuInput.moveveckey = KeyList.A;   // ��
                break;
            case 5:
                cpuInput.moveveckey = KeyList.SA;  // ����
                break;
            case 6:
                cpuInput.moveveckey = KeyList.S;   // ��
                break;
            case 7:
                cpuInput.moveveckey = KeyList.SD;  // �E��
                break;
            default:
                Debug.LogError("patrolPosDiv�G���[");
                break;
        }
    }
    #endregion
}