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

    [SerializeField] GameObject Enemys;//���̃I�u�W�F�N�g�̎q���v�f��CPU�𐶐�����(��Tanks).

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
                tank = Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
                //tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_NORMAL);//Player���p�X�w��Ő���.
                tank.transform.position = position;                                     //�����ʒu���Z�b�g.
                GameManager.instance.PushTank(teamID, tank.GetComponent<Rajikon>());    //�`�[��ID���M
                break;
            case TeamID.CPU:
                tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_NORMAL);//CPU���p�X�w��Ő���.
                tank.transform.position = position;                                   //�����ʒu���Z�b�g.
                tank.transform.parent = Enemys.transform;                             //CPU��Ememys�̎q����.
                GameManager.instance.PushTank(teamID, tank.GetComponent<Rajikon>());  //�`�[��ID���M
                break;
        }
    }
}