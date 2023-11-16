using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class StageManager : MonoBehaviour
{
    #region �萔�l
    const int PLAYER_NUM = 0;
    const int CPU_NUM = 1;

    const int SPOWN_POINTS = 0;
    #endregion

    #region private�ϐ�
    private int ChindCnt;//SpawnPoints�̎q���v�f�̐��𐔂���p�ϐ�.
    private bool createFlg;//Player�𐶐��������𔻒肷��.
    private GameObject Stage;
    #endregion

    [SerializeField] GameObject playerTank; //Player�v���t�@�u.

    [Tooltip("Enemys������"), SerializeField]
    EnemyManager enemyManager;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����.

        
    /// <summary>
    /// SpawnPoints�̎q�v�f���擾���AID�ɉ����ă^���N�𐶐�����.
    /// </summary>
    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//�q���̐����擾����.
        TeamID teamID;                              //ID�擾�p.
        EnemyName enemyName;                        //CPU�̏ꍇ���O���擾.
        for (int i = 0; i < ChindCnt; i++)          //�q�I�u�W�F�N�g�̐������[�v���ă^���N�𐶐�����.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;      //�`�[��ID�擾.
            enemyName = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().enemyName;//���O�擾.

            switch (teamID)
            {
                case TeamID.player:
                    CreateTank(spawnPoints.transform.GetChild(i).gameObject.transform.position);            //�^���N�����֐�.
                    break;
                case TeamID.CPU:
                    CreateTank(enemyName, spawnPoints.transform.GetChild(i).gameObject.transform.position, spawnPoints.transform.GetChild(i).gameObject); //�^���N�����֐�.
                    break;
                default:
                    Debug.LogError("�����ɖ�蔭��");
                    break;
            }
        }
        spawnPoints.SetActive(false);                        //�g���I�������ڈ���������߂�SpawnPoints���\���ɂ���.
        GameManager.instance.NowGameState = GAMESTATUS.READY;//�S�Ẵ^���N�̐������I�������Ready��Ԃɂ���.
    }

    /// <summary>
    /// GameManager����Ăяo���֐�
    /// �����ɃA�N�e�B�u�ɂ���X�e�[�W�i���o�[���w��A�X�|�[���|�C���g���擾���^���N�𐶐�����.
    /// </summary>
    public void ActiveStage(int stage)
    {
        GameObject spawnPoints;
        GameObject previousStage;//�O��̃X�e�[�W�j���p.
        previousStage = Stage;
        Destroy(previousStage);
        Stage = Instantiate((ResorceManager.Instance.GetStageResorce((StageNames)stage)));//Stage�𐶐����A�ϐ��ɑ������.
        spawnPoints = Stage.transform.GetChild(SPOWN_POINTS).gameObject;
        GetSpawnID(spawnPoints);
    }

    #region CreateTank(Player�ECPU�p)
    /// <summary>
    /// Player�^���N�𐶐�����֐�
    /// ������ID�Ɛ���������W���w��.
    /// </summary>
    void CreateTank(Vector3 position)
    {

        if (createFlg)//���ɐ������Ă�����ʒu��ς��邾��.
        {
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].SetPosition(0, position);
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].
                tankList[GameManager.instance.OWN_playerID].SetPlayTrail(false);
        }
        else
        {
            GameObject tank;
            tank = Instantiate(playerTank, position, Quaternion.identity);
            tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
            createFlg = true;
        }
    }

    /// <summary>
    /// �G�����p��CreateTank
    /// �G�̖��O�ƍ��W�������Ɏw�肳�ꂽ�G�𐶐�����
    /// </summary>
    void CreateTank(EnemyName name, Vector3 position,GameObject spawnpoint)
    {
        List<Vector3> points;
        switch (name)
        {
            case EnemyName.TUTORIAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Tutorial);//EnemyManager�̐����֐����Ăяo��.

                break;
            case EnemyName.NORMAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.MOVEMENT:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//����擾.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Movement,points);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.FAST_BULLET:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastBullet);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.FAST_AND_MOVE:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//����擾.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastAndMove,points);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.BOMBER:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//����擾.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Bomber, points);//EnemyManager�̐����֐����Ăяo��.
                break;
        }
    }
    #endregion

  public  void AllDestoroy()
    {
        Debug.LogWarning("���");
        for (int i = 0; i < enemyManager.transform.childCount; i++)
        {
            Destroy(enemyManager.transform.GetChild(i).gameObject);
            GameManager.instance.teamInfo[GameManager.instance.CPU_IDnum].MemberDeath();
        }
        GameManager.instance.ChangeReadyMode();
    }

}