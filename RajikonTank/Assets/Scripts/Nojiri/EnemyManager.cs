using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    #region 外部からの敵生成用

    // 通常敵生成
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // タンク生成
        enemyChildObj.transform.parent = this.transform;  // 生成した敵を子オブジェクトに追加
        enemyChildObj.transform.position = getSpawnPos;   // 受け取った初期位置に設定
    }

    #endregion
}
