using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class StageManager : MonoBehaviour
{
    #region �萔�l
    const int PLAYER_NUM = 0;
    const int CPU_NUM = 1;
    #endregion

    [SerializeField] GameObject SpawnPoints;//�^���N�𐶐�������W����ꂽ���X�g.
    [SerializeField] GameObject[] tanks;    //�e�X�g�p�v���t�@�u.

    [SerializeField] EnemyManager enemyManager;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����(��Tanks).

    private int ChindCnt;//SpawnPoints�̎q���v�f�̐��𐔂���p�ϐ�.
    // Start is called before the first frame update
    void Start()
    {
        ChindCnt = SpawnPoints.transform.childCount;//�q���̐����擾����.
        TeamID teamID;                              //ID�擾�p.
        for(int i = 0; i < ChindCnt; i++)           //�q�I�u�W�F�N�g�̐������[�v���ă^���N�𐶐�����.
        {
            teamID = SpawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID�擾.
            CreateTank(teamID, SpawnPoints.transform.GetChild(i).gameObject.transform.position);    //�^���N�����֐�.
        }
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
                //GameManager.instance.PushTank(TeamID.player, tank.GetComponent<Rajikon>()); // �`�[��ID���M
                //tank = TankGenerateClass.TankInstantiate(TankPrefabNames.NONE);//Player���p�X�w��Ő���.
                //tank.transform.position = position;                                     //�����ʒu���Z�b�g.
                break;
            case TeamID.CPU:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManager�̐����֐����Ăяo��.
                break;
        }
    }
}