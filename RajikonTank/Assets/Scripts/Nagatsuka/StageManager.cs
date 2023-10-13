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

    public List<GameObject> Stages;
    [SerializeField] GameObject[] tanks;    //�e�X�g�p�v���t�@�u.

    [SerializeField] EnemyManager enemyManager;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����.

    private int ChindCnt;//SpawnPoints�̎q���v�f�̐��𐔂���p�ϐ�.

    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//�q���̐����擾����.
        TeamID teamID;                              //ID�擾�p.
        for (int i = 0; i < ChindCnt; i++)           //�q�I�u�W�F�N�g�̐������[�v���ă^���N�𐶐�����.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID�擾.
            CreateTank(teamID, spawnPoints.transform.GetChild(i).gameObject.transform.position);    //�^���N�����֐�.
        }
        spawnPoints.SetActive(false);
        GameManager.instance.NowGameState = GAMESTATUS.READY;
    }

    /// <summary>
    /// �^���N�𐶐�����֐�
    /// ������ID�Ɛ���������W���w��.
    /// </summary>
    void CreateTank(TeamID teamID,Vector3 position)
    {
        GameObject tank;
        switch (teamID) {
            case TeamID.player:
                tank = Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
                tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
                break;
            case TeamID.CPU:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManager�̐����֐����Ăяo��.
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
        for (int i = 0; i < Stages.Count; i++)
        {
            if(i == stage)
            {
                Stages[i].SetActive(true);
                spawnPoints = Stages[i].transform.GetChild(SPOWN_POINTS).gameObject;
                GetSpawnID(spawnPoints);
            }
            else
            {
                Stages[i].SetActive(false);
            }
        }
    }
}