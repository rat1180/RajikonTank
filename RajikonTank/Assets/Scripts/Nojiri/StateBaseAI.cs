using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;  //�G�����̐ݒ�
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT; //�G�̏����J��

    private GameObject player;     // �v���C���[�擾
    private GameObject grandChild; // ���I�u�W�F�N�g�擾
    private Vector3 enemyPos;      // �G(����)�̈ʒu�擾
    private Vector3 playerPos;     // �v���C���[�̈ʒu�擾
    private bool isInit  = false;  // ��������Ԋm�F
    private bool isTimer = false;  // �^�C�}�[�t���O
    private const int shotNum = 1; // ���ː�


    public enum EnemyName // �G���
    {
        NORMAL,              // �ʏ�G
        REFLECT              // ���˓G
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
        // (��)
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
    }

    #region AI�J�ڃ��[�v
    /// <summary>
    /// �G�̎�ނɂ���čs���J�ڂ�ύX
    /// </summary>
    private void UpdateAI()
    {
        InitAI();

        switch (aiName)
        {
            case EnemyName.NORMAL:
                Normal();
                break;
            case EnemyName.REFLECT:
                //Reflect();
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
        enemyPos = transform.position;
        playerPos = player.transform.position;

        if (isInit == true)
        {
            return;
        }

        AddTeam(); // �`�[���ǉ�

        Debug.Log("���������s");
        isInit = true;
    }

    /// <summary>
    /// �`�[��ID���M
    /// </summary>
    public void AddTeam()
    {
        // CPU�̃`�[��ID�𑗂��ă`�[���ɒǉ�
    }
    #endregion

    #region �ʏ�G�J��
    /// <summary>
    /// AI��ԑJ��
    /// </summary>
    private void Normal()
    {
        if(isTimer == false)
        {
            NormalAiRoutine();

            switch (aiState)
            {
                case EnemyAiState.WAIT:
                    Debug.Log("�ҋ@");
                    break;
                case EnemyAiState.MOVE:
                    Debug.Log("�ړ�");
                    VectorCalc();
                    break;
                case EnemyAiState.TURN:
                    Debug.Log("����");
                    Turn();
                    break;
                case EnemyAiState.ATTACK:
                    Debug.Log("�ˌ�");
                    Attack();
                    break;
                case EnemyAiState.AVOID:
                    Debug.Log("���");
                    break;
                case EnemyAiState.DEATH:
                    Debug.Log("���S");
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Ray�ɐG�ꂽ�I�u�W�F�N�g�ɂ��State�̊��蓖��
    /// Player�̏ꍇ�@�F�U��
    /// ����ȊO�̏ꍇ�F�ړ�or����
    /// </summary>
    private void NormalAiRoutine()
    {
        RaycastHit hit; // Ray���Փ˂����I�u�W�F�N�g���
        bool attackFlg; // �U������t���O

        // Ray���΂�����(���ˈʒu, ����, �Փ˂����I�u�W�F�N�g���, ����(�L�ڂȂ��F����))
        //if (Physics.Raycast(enemyPos, playerPos, out hit))
        if (Physics.Raycast(enemyPos, playerPos, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit�^����GameObject�^�֕ϊ�

            if (hitObj.tag == "Player")
            {
                attackFlg = TurretDirection();
                //if(attackFlg) aiState = EnemyAiState.ATTACK; // �U��
                //else aiState = EnemyAiState.TURN;            // ����

                aiState = EnemyAiState.MOVE;   // �e�X�g�p
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

        StartCoroutine("AiTimer");
    }

    /// <summary>
    /// �C��̌������Q�Ƃ��A�U�������Ԃ�
    /// Player�Ɍ����Ă���Ƃ��Fture
    /// ����ȊO�@�@�@�@�@�@�@�Ffalse
    /// </summary>
    private bool TurretDirection()
    {
        grandChild = transform.Find("LAV25/LAV25_Turret").gameObject;  // LAV25_Turret�擾
        //grandChild = transform.GetChild(0).GetChild(1).gameObject;     // ���Ԃ���擾
        RaycastHit turretHit;   // ���C�ɏՓ˂����I�u�W�F�N�g���

        if (grandChild == null) return false;

        // �C��̑O����Ray�𔭎�
        if (Physics.SphereCast(enemyPos, 0.5f, grandChild.transform.forward, out turretHit))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            if (turretHitObj.tag == "Player")
            {
                Debug.Log("Player�ɓ�������");
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

    // ���t���[�������ɂ�镉�ׂ̌y���p�^�C�}�[
    IEnumerator AiTimer()
    {
        isTimer = true;
        yield return new WaitForSeconds(0.5f); // 0.5�b���ƂɎ��s

        isTimer = false;
    }

    #region �s���J�ڂ��Ƃ̃��\�b�h
    private void Attack()
    {
        GameObject shotObj = grandChild.transform.GetChild(0).gameObject;  // ShotPosition�擾
        bool isInstans = BulletGenerateClass.BulletInstantiate(gameObject, shotObj, "Bullet", shotNum); // �e����
        if (isInstans) Debug.LogError("�e����������܂���ł���");
    }

    private void Turn()
    {
        // ����
        //Vector3 aim = playerPos - enemyPos;       // �Ώۂւ̑��΃x�N�g���擾
        //var look = Quaternion.LookRotation(aim);  // �Ώۂ̕����Ɍ������\�b�h
        //grandChild.transform.rotation = look;     // Player�̕���������
    }

    /// <summary>
    /// bullet�ɓ����������ɌĂ΂�鎀�S���s���\�b�h
    /// 
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        aiState = EnemyAiState.DEATH; // ���S�J�ڂɈڍs�A�G����
    }
    #endregion

    #region �x�N�g���ϊ�
    /// <summary>
    /// Enemy����Player�̂���p�x���v�Z
    /// ���߂��p�x��8�����ɕ������AVector2(-1�`1, -1�`1)�ɕϊ�
    /// </summary>
    private void VectorCalc()
    {
        // ���ς����߁A�p�x�ɕϊ�(���ρ��p�x)
        float VectorX = enemyPos.x - playerPos.x;
        float VectorZ = enemyPos.z - playerPos.z;
        float Radian = Mathf.Atan2(VectorZ, VectorX) * Mathf.Rad2Deg;

        //�p�x�\���ύX
        if (Radian < 0)
        {
            // -180�`180�ŕԂ邽�߁A0�`360�ɕϊ�
            Radian += 360;
        }

        // 360�x��8�������A�l�̌ܓ�����(0�`8)
        int Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8�̏ꍇ0�ɕϊ�(9�����h�~)
        if (Division == 8) Division = 0;

        // Vector2�ɕϊ����Ď擾
        //Vector2 Direction = Conversion(Division);
        Vector3 Direction = Conversion(Division); // �e�X�g

        //transform.position += Direction; // Player�Ǐ]�e�X�g
    }

    // �x�N�g���ϊ����\�b�h
    Vector3 Conversion(int index)
    {
        switch (index)
        {
            // �����
            //case 0: return new Vector2(-1,  0);  // 0�x
            //case 1: return new Vector2(-1, -1);  // 45�x
            //case 2: return new Vector2( 0, -1);  // 90�x
            //case 3: return new Vector2( 1, -1);  // 135�x
            //case 4: return new Vector2( 1,  0);  // 180�x
            //case 5: return new Vector2( 1,  1);  // 225�x
            //case 6: return new Vector2( 0,  1);  // 270�x
            //case 7: return new Vector2(-1,  1);  // 315�x
            //default: return Vector2.zero;

            // �e�X�g
            case 0: return new Vector3(-1, 0,  0);   // 0�x
            case 1: return new Vector3(-1, 0, -1);   // 45�x
            case 2: return new Vector3( 0, 0, -1);   // 90�x
            case 3: return new Vector3( 1, 0, -1);  // 135�x
            case 4: return new Vector3( 1, 0,  0);  // 180�x
            case 5: return new Vector3( 1, 0,  1); // 225�x
            case 6: return new Vector3( 0, 0,  1);  // 270�x
            case 7: return new Vector3(-1, 0,  1);  // 315�x
            default: return Vector2.zero;
        }
    }
    #endregion
}
