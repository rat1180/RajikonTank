using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class StateBaseAI : MonoBehaviour
{
    public EnemyName aiName = EnemyName.NORMAL;      //�G�����̐ݒ�
    public EnemyAiState aiState = EnemyAiState.WAIT; //�G�̍U���J��

    [SerializeField] public Transform player;// �v���C���[�̈ʒu�擾
    private Vector3 enemyPos;   // �G(����)�̈ʒu�擾
    private Vector3 playerPos;  // �v���C���[�̈ʒu�擾

    private bool initFlg = true;
    private bool timerFlg = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
    }

    #region �������[�v
    /// <summary>
    /// ����������
    /// </summary>
    void InitAI()
    {
        enemyPos = transform.position;
        playerPos = player.position;

        if (initFlg == false)
        {
            return;
        }

        Debug.Log("���������s");
        initFlg = false;
    }
    #endregion

    #region AI�J�ڃ��[�v
    /// <summary>
    /// �G�̎�ނɂ���čs���J�ڂ�ύX
    /// </summary>
    void UpdateAI()
    {
        InitAI();

        switch (aiName)
        {
            case EnemyName.NORMAL:  // �ʏ�G
                Normal();
                break;
            case EnemyName.REFLECT: // ���˓G
                //Move2();
                break;
            default:
                break;
        }
    }
    #endregion

    #region �ʏ�G�J��
    /// <summary>
    /// AI��ԑJ��
    /// </summary>
    private void Normal()
    {
        if(timerFlg == false)
        {
            AiMainRoutine();

            switch (aiState)
            {
                case EnemyAiState.WAIT:
                    Debug.Log("�ҋ@");
                    break;
                case EnemyAiState.MOVE:
                    Debug.Log("�ړ�");
                    break;
                case EnemyAiState.TURN:
                    Debug.Log("����");
                    break;
                case EnemyAiState.ATTACK:
                    Debug.Log("�ˌ�");
                    break;
                case EnemyAiState.AVOID:
                    break;
                case EnemyAiState.DEATH:
                    break;
                default:
                    break;
            }

            StartCoroutine("AiTimer");
        }
    }

    /// <summary>
    /// Ray�ɐG�ꂽ�I�u�W�F�N�g�ɂ��State�̊��蓖��
    /// Player�̏ꍇ�@�F�U��
    /// ����ȊO�̏ꍇ�F�ړ�or����
    /// </summary>
    private void AiMainRoutine()
    {
        RaycastHit hit;
        bool attackFlg;

        // Ray���΂�����(���ˈʒu, ����, �Փ˂����I�u�W�F�N�g���, ����(�L�ڂȂ��F����))
        if (Physics.Raycast(enemyPos, playerPos, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit�^����GameObject�^�֕ϊ�

            if (hitObj.tag == "Player")
            {
                attackFlg = TurretDirection();
                if(attackFlg) aiState = EnemyAiState.ATTACK; // �U��
                else aiState = EnemyAiState.TURN;
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
    }

    // ���t���[�������ɂ�镉�ׂ̌y���p�^�C�}�[
    IEnumerator AiTimer()
    {
        timerFlg = true;
        yield return new WaitForSeconds(0.1f); // 0.2�b���ƂɎ��s

        timerFlg = false;
    }
    #endregion

    #region �U������
    /// <summary>
    /// �C��̌������Q�Ƃ��A�U�������Ԃ�
    /// Player�Ɍ����Ă���Ƃ��Fture
    /// ����ȊO�@�@�@�@�@�@�@�Ffalse
    /// </summary>
    private bool TurretDirection()
    {
        GameObject grandChild = transform.GetChild(0).GetChild(1).gameObject;  // ���I�u�W�F�N�g���擾
        RaycastHit turretHit;   // ���C�ɏՓ˂����I�u�W�F�N�g���

        Debug.Log(grandChild);

        // ����
        //Vector3 aim = playerPos - enemyPos;       // �Ώۂւ̑��΃x�N�g���擾
        //var look = Quaternion.LookRotation(aim);  // �Ώۂ̕����Ɍ������\�b�h
        //child.transform.rotation = look;  // Player�̕���������

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
        Vector2 Direction = Conversion(Division);

        // Vector2�ϊ���̌��ʕ\��
        Debug.Log("�p�x�F " + Direction);
    }

    // �x�N�g���ϊ����\�b�h
    Vector2 Conversion(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(0, 1);   // 0�x
            case 1: return new Vector2(1, 1);   // 45�x
            case 2: return new Vector2(1, 0);   // 90�x
            case 3: return new Vector2(1, -1);  // 135�x
            case 4: return new Vector2(0, -1);  // 180�x
            case 5: return new Vector2(-1, -1); // 225�x
            case 6: return new Vector2(-1, 0);  // 270�x
            case 7: return new Vector2(-1, 1);  // 315�x
            default: return Vector2.zero;
        }
    }
    #endregion
}
