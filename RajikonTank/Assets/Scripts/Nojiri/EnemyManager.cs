using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    Rajikon rajikon;

    private bool isGenerate = true;   // �����t���O

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
    }

    #region �G�����e�X�g�p
    private void UpdateEnemy()
    {
        // ��x�̂ݎ��s
        if (isGenerate == false)
        {
            return;
        }

        SpawnNormalEnemy();

        isGenerate = false;
    }
    #endregion

    #region �O������̓G�����p

    // �ʏ�G����
    public void SpawnNormalEnemy()
    {
        GameObject enemyChildObj;

        enemyChildObj = TankGenerateClass.TankInstantiate(TankPrefabNames.Enemy_Normal); // �^���N����
        enemyChildObj.transform.parent = this.transform;    // ���������G���q�I�u�W�F�N�g�ɒǉ�
        enemyChildObj.transform.position = GetPosition();   // �����ʒu�ɐݒ�

        GameManager.instance.PushTank(TeamID.CPU, rajikon); // �`�[��ID���M
    }

    // ���˓G����
    //public void SpawnReflectEnemy()
    //{
    //GameObject enemyChildObj;
    //Vector3 spawnPos = GetPosition(); // �����ʒu���擾

    //enemyChildObj = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_REFLECT); // �^���N����
    //    enemyChildObj.transform.parent = this.transform;   // ���������G���q�I�u�W�F�N�g�ɒǉ�
    //}
    #endregion

    // �����ʒu���󂯎��
    private Vector3 GetPosition()
    {
        //SpawnPoint spawnPoint;

        // (��)
        return new Vector3(0, 0, 0);
    }
}
