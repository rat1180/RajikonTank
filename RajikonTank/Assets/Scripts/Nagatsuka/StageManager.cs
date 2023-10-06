using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class StageManager : MonoBehaviour
{
    #region 定数値
    const int PLAYER_NUM = 0;
    const int CPU_NUM = 1;
    #endregion

    [SerializeField] GameObject SpawnPoints;//タンクを生成する座標を入れたリスト.
    [SerializeField] GameObject[] tanks;

    [SerializeField] EnemyManager enemyManager;//このオブジェクトの子供要素にCPUを生成する(旧Tanks).

    [Header("確認用変数")]
    [SerializeField]int ChindCnt;
    // Start is called before the first frame update
    void Start()
    {
        ChindCnt = SpawnPoints.transform.childCount;//子供の数を取得する.
        TeamID teamID;                              //ID取得用.
        for(int i = 0; i < ChindCnt; i++)           //子オブジェクトの数分ループしてタンクを生成する.
        {
            teamID = SpawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID取得.
            Debug.Log(teamID);
            CreateTank(teamID, SpawnPoints.transform.GetChild(i).gameObject.transform.position);    //タンク生成関数.
        }
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
                tank = TankGenerateClass.TankInstantiate(TankPrefabNames.NONE);//Playerをパス指定で生成.
                tank.transform.position = position;                                     //生成位置をセット.
                break;
            case TeamID.CPU:
                //enemyManager.SpawnNormalEnemy(position);
                //tank = TankGenerateClass.TankInstantiate(TankPrefabNames.Enemy_Normal);//CPUをパス指定で生成.
                //tank.transform.position = position;                                    //生成位置をセット.
                //tank.transform.parent = enemyManager.transform;                              //CPUをEmemysの子供に.
                //GameManager.instance.PushTank(teamID, tank.GetComponent<Rajikon>());   //チームID送信
                break;
        }
       // GameManager.instance.testOBJ = GameManager.instance.teamInfo[GameManager.instance.player_IDnum].tankList[0].gameObject;
    }
}