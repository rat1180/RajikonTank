using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    private bool isGenerate = true;   // 生成フラグ

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
    }

    #region 敵生成テスト用
    private void UpdateEnemy()
    {
        // 一度のみ実行
        if (isGenerate == false)
        {
            return;
        }

        EnemyGenerate();

        isGenerate = false;
    }

    // 敵生成
    private void EnemyGenerate()
    {
        // (仮作成)
        GameObject enemyObj = null;

        enemyObj = (GameObject)Resources.Load("NormalTank");   // NormalEnemy取得

        if (enemyObj != null)
        {
            Debug.Log("生成");
            SpawnEnemy(enemyObj);
        }

        //TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_TEST); // タンク生成置き換え予定
    }

    private void SpawnEnemy(GameObject enemy)
    {
        GameObject enemyChildObj;
        Vector3 spawnPos = GetPosition(); // 初期位置を取得

        enemyChildObj = Instantiate(enemy, spawnPos, Quaternion.identity);  // 敵生成
        enemyChildObj.transform.parent = this.transform;   // 生成した敵を子オブジェクトに追加
    }
    #endregion

    #region 外部からの敵生成用

    // 通常敵生成
    public void SpawnNormalEnemy()
    {
        GameObject enemyChildObj;

        Vector3 spawnPos = GetPosition(); // 初期位置を取得
        GameObject enemyObj = (GameObject)Resources.Load("NormalTank");       // NormalEnemy取得

        enemyChildObj = Instantiate(enemyObj, spawnPos, Quaternion.identity);  // 敵生成
        enemyChildObj.transform.parent = this.transform;  // 生成した敵を子オブジェクトに追加
    }

    // 反射敵生成
    //public void SpawnReflectEnemy()
    //{
    //    GameObject enemyChildObj;

    //    Vector3 spawnPos = GetPosition(); // 初期位置を取得
    //    GameObject enemyObj = (GameObject)Resources.Load("ReflectEnemy");      // ReflectEnemy取得

    //    enemyChildObj = Instantiate(enemyObj, spawnPos, Quaternion.identity);
    //    enemyChildObj.transform.parent = this.transform;
    //}
    #endregion

    // 初期位置を受け取る
    private Vector3 GetPosition()
    {
        //SpawnPoint spawnPoint;

        // (仮)
        return new Vector3(0, 0, 0);
    }
}
