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

    #region private変数
    private int ChindCnt;//SpawnPointsの子供要素の数を数える用変数.
    private bool createFlg;//Playerを生成したかを判定する.
    private GameObject Stage;
    #endregion

    [SerializeField] GameObject playerTank; //Playerプレファブ.

    [Tooltip("Enemysを入れる"), SerializeField]
    EnemyManager enemyManager;//このオブジェクトの子供要素にCPUを生成する.

        
    /// <summary>
    /// SpawnPointsの子要素を取得し、IDに応じてタンクを生成する.
    /// </summary>
    private void GetSpawnID(GameObject spawnPoints)
    {
        ChindCnt = spawnPoints.transform.childCount;//子供の数を取得する.
        TeamID teamID;                              //ID取得用.
        EnemyName enemyName;                        //CPUの場合名前も取得.
        for (int i = 0; i < ChindCnt; i++)          //子オブジェクトの数分ループしてタンクを生成する.
        {
            teamID = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;      //チームID取得.
            enemyName = spawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().enemyName;//名前取得.

            switch (teamID)
            {
                case TeamID.player:
                    CreateTank(spawnPoints.transform.GetChild(i).gameObject.transform.position);            //タンク生成関数.
                    break;
                case TeamID.CPU:
                    CreateTank(enemyName, spawnPoints.transform.GetChild(i).gameObject.transform.position, spawnPoints.transform.GetChild(i).gameObject); //タンク生成関数.
                    break;
                default:
                    Debug.LogError("生成に問題発生");
                    break;
            }
        }
        spawnPoints.SetActive(false);                        //使い終わったら目印を消すためにSpawnPointsを非表示にする.
        GameManager.instance.NowGameState = GAMESTATUS.READY;//全てのタンクの生成が終わったらReady状態にする.
    }

    /// <summary>
    /// GameManagerから呼び出す関数
    /// 引数にアクティブにするステージナンバーを指定、スポーンポイントを取得しタンクを生成する.
    /// </summary>
    public void ActiveStage(int stage)
    {
        GameObject spawnPoints;
        GameObject previousStage;//前回のステージ破棄用.
        previousStage = Stage;
        Destroy(previousStage);
        Stage = Instantiate((ResorceManager.Instance.GetStageResorce((StageNames)stage)));//Stageを生成し、変数に代入する.
        spawnPoints = Stage.transform.GetChild(SPOWN_POINTS).gameObject;
        GetSpawnID(spawnPoints);
    }

    #region CreateTank(Player・CPU用)
    /// <summary>
    /// Playerタンクを生成する関数
    /// 引数にIDと生成する座標を指定.
    /// </summary>
    void CreateTank(Vector3 position)
    {

        if (createFlg)//既に生成していたら位置を変えるだけ.
        {
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].SetPosition(0, position);
            GameManager.instance.teamInfo[GameManager.instance.player_IDnum].
                tankList[GameManager.instance.OWN_playerID].SetPlayTrail(false);
        }
        else
        {
            GameObject tank;
            tank = Instantiate(playerTank, position, Quaternion.identity);
            tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.NATURAL);
            createFlg = true;
        }
    }

    /// <summary>
    /// 敵生成用のCreateTank
    /// 敵の名前と座標を引数に指定された敵を生成する
    /// </summary>
    void CreateTank(EnemyName name, Vector3 position,GameObject spawnpoint)
    {
        List<Vector3> points;
        switch (name)
        {
            case EnemyName.TUTORIAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Tutorial);//EnemyManagerの生成関数を呼び出す.

                break;
            case EnemyName.NORMAL:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Normal);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.MOVEMENT:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//巡回取得.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Movement,points);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.FAST_BULLET:
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastBullet);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.FAST_AND_MOVE:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//巡回取得.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_FastAndMove,points);//EnemyManagerの生成関数を呼び出す.
                break;
            case EnemyName.BOMBER:
                points = spawnpoint.GetComponent<SpawnPoint>().position;//巡回取得.
                enemyManager.SpawnEnemy(position, TankPrefabNames.Enemy_Bomber, points);//EnemyManagerの生成関数を呼び出す.
                break;
        }
    }
    #endregion

  public  void AllDestoroy()
    {
        Debug.LogWarning("よんだ");
        for (int i = 0; i < enemyManager.transform.childCount; i++)
        {
            Destroy(enemyManager.transform.GetChild(i).gameObject);
            GameManager.instance.teamInfo[GameManager.instance.CPU_IDnum].MemberDeath();
        }
        GameManager.instance.ChangeReadyMode();
    }

}