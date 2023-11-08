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

    [SerializeField] bool createFlg;//Player�𐶐��������𔻒肷��.

    public GameObject Stages;//Stages���q�G�����L�[��������.
    public List<GameObject> Stage;
    [SerializeField] GameObject[] tanks;    //�e�X�g�p�v���t�@�u.

    [SerializeField] EnemyManager enemyManager;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����.

    private int ChindCnt;//SpawnPoints�̎q���v�f�̐��𐔂���p�ϐ�.

    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//�q���̐����擾����.
        TeamID teamID;                              //ID�擾�p.
        EnemyName enemyName;                        //CPU�̏ꍇ���O���擾.
        for (int i = 0; i < ChindCnt; i++)           //�q�I�u�W�F�N�g�̐������[�v���ă^���N�𐶐�����.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID�擾.
            enemyName = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().enemyName;//ID�擾.
            switch (teamID) {
                case TeamID.player:
                    CreateTank(spawnPoints.transform.GetChild(i).gameObject.transform.position);    //�^���N�����֐�.
                    break;
                case TeamID.CPU:
                    CreateTank(enemyName, spawnPoints.transform.GetChild(i).gameObject.transform.position);    //�^���N�����֐�.
                    break;
                default:
                    Debug.LogError("�����ɖ�蔭��");
                    break;
            }  
        }
        spawnPoints.SetActive(false);
        GameManager.instance.NowGameState = GAMESTATUS.READY;
    }

    /// <summary>
    /// Player�^���N�𐶐�����֐�
    /// ������ID�Ɛ���������W���w��.
    /// </summary>
    void CreateTank(Vector3 position)
    {

        if (createFlg)
        {
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].SetPosition(0,position);
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].
                tankList[GameManager.instance.OWN_playerID].SetPlayTrail(false);
        }
        else
        {
            GameObject tank;
            tank = Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
            tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
            createFlg = true;
        }
    }
    void CreateTank(EnemyName name, Vector3 position)
    {
        switch (name)
        {
            case EnemyName.TUTORIAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Tutorial);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.NORMAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.MOVEMENT:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Movement);//EnemyManager�̐����֐����Ăяo��.
                break;       
            case EnemyName.FASTBULLET:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastBullet);//EnemyManager�̐����֐����Ăяo��.
                break;
            case EnemyName.FASTANDMOVE:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastAndMove);//EnemyManager�̐����֐����Ăяo��.
                break;
        }
    }

    /// <summary>
    /// GameManager����Ăяo���֐�
    /// �����ɃA�N�e�B�u�ɂ���X�e�[�W�i���o�[���w��A�X�|�[���|�C���g���擾���^���N�𐶐�����.
    /// </summary>
    /// <param name="stage"></param>
    public void ActiveStage(int stage)
    {
        GameObject spawnPoints;
        for (int i = 0; i < Stage.Count; i++)
        {
            if(i == stage)
            {
                Stage[i].SetActive(true);
                spawnPoints = Stage[i].transform.GetChild(SPOWN_POINTS).gameObject;
                GetSpawnID(spawnPoints);
            }
            else
            {
                Stage[i].SetActive(false);
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < Stages.transform.childCount; i++)
        {
            Stage.Add(Stages.transform.GetChild(i).gameObject);
        }
    }
}