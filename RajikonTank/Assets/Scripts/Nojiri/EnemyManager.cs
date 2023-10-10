using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// 通常敵生成
    /// </summary>
    /// <param name="getSpawnPos">初期位置</param>
    /// <param name="spawnName">生成する敵の名前</param>
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // タンク生成
        enemyChildObj.transform.parent = this.transform;  // 生成した敵を子オブジェクトに追加
        enemyChildObj.transform.position = getSpawnPos;   // 受け取った初期位置に設定
    }
}
