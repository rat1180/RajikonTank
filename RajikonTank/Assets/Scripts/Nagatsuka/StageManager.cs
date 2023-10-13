using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class StageManager : MonoBehaviour
{
    #region 定数値
    const int PLAYER_NUM = 0;
    const int CPU_NUM = 1;

    const int SPOWN_POINTS = 0;
    #endregion

    public List<GameObject> Stages;
    [SerializeField] GameObject[] tanks;    //テスト用プレファブ.

    [SerializeField] EnemyManager enemyManager;//このオブジェクトの子供要素にCPUを生成する.

    private int ChindCnt;//SpawnPointsの子供要素の数を数える用変数.

    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//子供の数を取得する.
        TeamID teamID;                              //ID取得用.
        for (int i = 0; i < ChindCnt; i++)           //子オブジェクトの数分ループしてタンクを生成する.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID取得.
            CreateTank(teamID, spawnPoints.transform.GetChild(i).gameObject.transform.position);    //タンク生成関数.
        }
        spawnPoints.SetActive(false);
        GameManager.instance.NowGameState = GAMESTATUS.READY;
    }

    /// <summary>
    /// タンクを生成する関数
    /// 引数にIDと生成する座標を指定.
    /// </summary>
    void CreateTank(TeamID teamID,Vector3 position)
    {
        GameObject tank;
        switch (teamID) {
            case TeamID.player:
                tank = Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
                tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
                break;
            case TeamID.CPU:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManagerの生成関数を呼び出す.
                break;
        }
    }

    /// <summary>
    /// GameManagerから呼び出す関数
    /// 引数にアクティブにするステージナンバーを指定、スポーンポイントを取得しタンクを生成する.
    /// </summary>
    /// <param name="stage"></param>
    public void ActiveStage(int stage)
    {
        GameObject spawnPoints;
        for (int i = 0; i < Stages.Count; i++)
        {
            if(i == stage)
            {
                Stages[i].SetActive(true);
                spawnPoints = Stages[i].transform.GetChild(SPOWN_POINTS).gameObject;
                GetSpawnID(spawnPoints);
            }
            else
            {
                Stages[i].SetActive(false);
            }
        }
    }
}