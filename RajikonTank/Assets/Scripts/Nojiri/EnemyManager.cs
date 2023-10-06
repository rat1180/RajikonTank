using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    #region �O������̓G�����p

    // �ʏ�G����
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // �^���N����
        enemyChildObj.transform.parent = this.transform;  // ���������G���q�I�u�W�F�N�g�ɒǉ�
        enemyChildObj.transform.position = getSpawnPos;   // �󂯎���������ʒu�ɐݒ�
    }

    #endregion
}
