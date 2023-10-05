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
    [SerializeField] GameObject[] tanks;

    [SerializeField] EnemyManager enemyManager;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����(��Tanks).

    [Header("�m�F�p�ϐ�")]
    [SerializeField]int ChindCnt;
    // Start is called before the first frame update
    void Start()
    {
        ChindCnt = SpawnPoints.transform.childCount;//�q���̐����擾����.
        TeamID teamID;                              //ID�擾�p.
        for(int i = 0; i < ChindCnt; i++)           //�q�I�u�W�F�N�g�̐������[�v���ă^���N�𐶐�����.
        {
            teamID = SpawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID�擾.
            Debug.Log(teamID);
            CreateTank(teamID, SpawnPoints.transform.GetChild(i).gameObject.transform.position);    //�^���N�����֐�.
        }
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
                tank = TankGenerateClass.TankInstantiate(TankPrefabNames.NONE);//Player���p�X�w��Ő���.
                tank.transform.position = position;                                     //�����ʒu���Z�b�g.
                break;
            case TeamID.CPU:
                //enemyManager.SpawnNormalEnemy(position);
                //tank = TankGenerateClass.TankInstantiate(TankPrefabNames.Enemy_Normal);//CPU���p�X�w��Ő���.
                //tank.transform.position = position;                                    //�����ʒu���Z�b�g.
                //tank.transform.parent = enemyManager.transform;                              //CPU��Ememys�̎q����.
                //GameManager.instance.PushTank(teamID, tank.GetComponent<Rajikon>());   //�`�[��ID���M
                break;
        }
       // GameManager.instance.testOBJ = GameManager.instance.teamInfo[GameManager.instance.player_IDnum].tankList[0].gameObject;
    }
}