using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    public Vector3[] MovePointsArray { get; set; }   // 巡回用の位置情報配列

    #region GameManager用メソッド
    /// <summary>
    /// 敵生成(巡回なしver)
    /// 
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
    /// 敵生成(巡回ありver)
    /// </summary>
    /// <param name="getSpawnPos">初期位置</param>
    /// <param name="spawnName">生成する敵の名前</param>
    /// <param name="points">巡回する位置情報リスト</param>
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName,List<Vector3> points)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // タンク生成
        enemyChildObj.transform.parent = this.transform;  // 生成した敵を子オブジェクトに追加
        enemyChildObj.transform.position = getSpawnPos;   // 受け取った初期位置に設定
        enemyChildObj.GetComponent<StateBaseAI>().SetPatrolPoint(points);
        PatrolPositionSet(points);  // リストの情報をコピー
    }

    /// <summary>
    /// 巡回位置のリストを取得
    /// 送られた位置情報を保存
    /// </summary>
    /// <param name="points">巡回する位置情報リスト</param>
    public void PatrolPositionSet(List<Vector3> points)
    {
        // 要素数と位置のコピー
        MovePointsArray = new Vector3[points.Count];

        for(int i = 0; i < points.Count; i++)
        {
            MovePointsArray[i] = points[i];
        }
    }
    #endregion
}
