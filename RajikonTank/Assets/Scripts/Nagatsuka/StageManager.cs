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

    [SerializeField] bool createFlg;//Playerを生成したかを判定する.

    public GameObject Stages;//Stagesをヒエラルキーから入れる.
    public List<GameObject> Stage;
    [SerializeField] GameObject[] tanks;    //テスト用プレファブ.

    [SerializeField] EnemyManager enemyManager;//このオブジェクトの子供要素にCPUを生成する.

    private int ChindCnt;//SpawnPointsの子供要素の数を数える用変数.

    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//子供の数を取得する.
        TeamID teamID;                              //ID取得用.
        EnemyName enemyName;                        //CPUの場合名前も取得.
        for (int i = 0; i < ChindCnt; i++)           //子オブジェクトの数分ループしてタンクを生成する.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;//ID取得.
            enemyName = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().enemyName;//ID取得.
            switch (teamID) {
                case TeamID.player:
                    CreateTank(spawnPoints.transform.GetChild(i).gameObject.transform.position);    //タンク生成関数.
                    break;
                case TeamID.CPU:
                    CreateTank(enemyName, spawnPoints.transform.GetChild(i).gameObject.transform.position);    //タンク生成関数.
                    break;
                default:
                    Debug.LogError("生成に問題発生");
                    break;
            }  
        }
        spawnPoints.SetActive(false);
        GameManager.instance.NowGameState = GAMESTATUS.READY;
    }

    /// <summary>
    /// Playerタンクを生成する関数
    /// 引数にIDと生成する座標を指定.
    /// </summary>
    void CreateTank(Vector3 position)
    {

        if (createFlg)
        {
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].SetPosition(0,position);
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].
                tankList[GameManager.instance.OWN_playerID].SetPlayTrail(false);
        }
        else
        {
            GameObject tank;
            tank = Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
            tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
            createFlg = true;
        }
    }
    void CreateTank(EnemyName name, Vector3 position)
    {
        switch (name)
        {
            case EnemyName.TUTORIAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Tutorial);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.NORMAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.MOVEMENT:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Movement);//EnemyManagerの生成関数を呼び出す.
                break;       
            case EnemyName.FASTBULLET:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastBullet);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.FASTANDMOVE:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastAndMove);//EnemyManagerの生成関数を呼び出す.
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
        for (int i = 0; i < Stage.Count; i++)
        {
            if(i == stage)
            {
                Stage[i].SetActive(true);
                spawnPoints = Stage[i].transform.GetChild(SPOWN_POINTS).gameObject;
                GetSpawnID(spawnPoints);
            }
            else
            {
                Stage[i].SetActive(false);
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < Stages.transform.childCount; i++)
        {
            Stage.Add(Stages.transform.GetChild(i).gameObject);
        }
    }
}