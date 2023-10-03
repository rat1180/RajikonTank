using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
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

        EnemyGenerate();

        isGenerate = false;
    }

    // �G����
    private void EnemyGenerate()
    {
        // (���쐬)
        GameObject enemyObj = null;

        enemyObj = (GameObject)Resources.Load("NormalTank");   // NormalEnemy�擾

        if (enemyObj != null)
        {
            Debug.Log("����");
            SpawnEnemy(enemyObj);
        }

        //TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_TEST); // �^���N�����u�������\��
    }

    private void SpawnEnemy(GameObject enemy)
    {
        GameObject enemyChildObj;
        Vector3 spawnPos = GetPosition(); // �����ʒu���擾

        enemyChildObj = Instantiate(enemy, spawnPos, Quaternion.identity);  // �G����
        enemyChildObj.transform.parent = this.transform;   // ���������G���q�I�u�W�F�N�g�ɒǉ�
    }
    #endregion

    #region �O������̓G�����p

    // �ʏ�G����
    public void SpawnNormalEnemy()
    {
        GameObject enemyChildObj;

        Vector3 spawnPos = GetPosition(); // �����ʒu���擾
        GameObject enemyObj = (GameObject)Resources.Load("NormalTank");       // NormalEnemy�擾

        enemyChildObj = Instantiate(enemyObj, spawnPos, Quaternion.identity);  // �G����
        enemyChildObj.transform.parent = this.transform;  // ���������G���q�I�u�W�F�N�g�ɒǉ�
    }

    // ���˓G����
    //public void SpawnReflectEnemy()
    //{
    //    GameObject enemyChildObj;

    //    Vector3 spawnPos = GetPosition(); // �����ʒu���擾
    //    GameObject enemyObj = (GameObject)Resources.Load("ReflectEnemy");      // ReflectEnemy�擾

    //    enemyChildObj = Instantiate(enemyObj, spawnPos, Quaternion.identity);
    //    enemyChildObj.transform.parent = this.transform;
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
