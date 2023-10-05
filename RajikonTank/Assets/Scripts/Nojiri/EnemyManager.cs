using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    Rajikon rajikon;

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

        SpawnNormalEnemy();

        isGenerate = false;
    }
    #endregion

    #region 外部からの敵生成用

    // 通常敵生成
    public void SpawnNormalEnemy()
    {
        GameObject enemyChildObj;

        enemyChildObj = TankGenerateClass.TankInstantiate(TankPrefabNames.Enemy_Normal); // タンク生成
        enemyChildObj.transform.parent = this.transform;    // 生成した敵を子オブジェクトに追加
        enemyChildObj.transform.position = GetPosition();   // 初期位置に設定

        GameManager.instance.PushTank(TeamID.CPU, rajikon); // チームID送信
    }

    // 反射敵生成
    //public void SpawnReflectEnemy()
    //{
    //GameObject enemyChildObj;
    //Vector3 spawnPos = GetPosition(); // 初期位置を取得

    //enemyChildObj = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_REFLECT); // タンク生成
    //    enemyChildObj.transform.parent = this.transform;   // 生成した敵を子オブジェクトに追加
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
