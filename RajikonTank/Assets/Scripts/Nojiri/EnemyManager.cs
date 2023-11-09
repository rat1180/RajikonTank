using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;   // 参照用
    private Vector3[] enemyMovePos;        // 敵巡回用の位置情報配列

    private void Awake()
    {
        instance = this;
    }

    #region GameManager用メソッド
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

    /// <summary>
    /// 外部から巡回位置の配列を取得
    /// 送られた位置情報を保存
    /// </summary>
    /// <param name="points">巡回位置の配列</param>
    public void PatrolPositionSet(Vector3[] points)
    {
        // 要素数と巡回位置の保存
        enemyMovePos = new Vector3[points.Length];
        enemyMovePos = points;
    }
    #endregion

    #region StateBaseAI用メソッド
    /// <summary>
    /// StateBaseAI用
    /// 巡回位置情報の取得用メソッド
    /// PatrolPositionSet実行前、返り値：NULL
    /// PatrolPositionSet実行後、返り値：位置情報配列
    /// </summary>
    /// <returns></returns>
    public Vector3[] PatrolPositionGet()
    {
        return enemyMovePos;
    }
    #endregion
}
