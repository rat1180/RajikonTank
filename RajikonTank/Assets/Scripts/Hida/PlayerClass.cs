using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class PlayerClass : TankEventHandler
{
    
    [SerializeField,Tooltip("このクラスが所持しているタンク"),Header("デバッグ用表示")] private Rajikon PossessionTank;

    [SerializeField,Tooltip("操作を受付するか")] private bool isControl;

    [SerializeField, Tooltip("ゲームマネージャーのインスタンス")] private GameManager GameManagerInstance;

    [SerializeField, Tooltip("所属しているチームID")] private TeamID TeamID;

    [SerializeField, Tooltip("このプレイヤーのスポーンポイント")] private Vector3 SpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤーの初期化
        InitPlayer();

        
    }

    // Update is called once per frame
    void Update()
    {
        //タンクをアップデートする
        UpdateTank();
    }

    #region 初期化・生成関数

    /// <summary>
    /// Playerクラスの初期化を行う
    /// ゲームマネージャーへのチーム登録とタンクの生成、タンクの初期化
    /// </summary>
    private void InitPlayer()
    {
        //マネージャーの取得
        GameManagerInstance = GameManager.instance;

        //テスト
        GameManager.instance.PushTeamList(TeamID);

        //操作権限の初期化
        isControl = false;

        //テスト
        PopTank();

        //エラー確認
        if (PossessionTank == null)
        {
            Debug.LogWarning("初期化時にタンクが生成されていません");
        }

        //チームへ追加
        GameManagerInstance.PushTank(TeamID, PossessionTank);
        
    }

    private GameObject TankSpawn()
    {
        //タンクの生成
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TANK_TEST);

        //タンクの初期化


        //タンクの初期位置を設定
        tank.transform.position = SpawnPoint;

        //自分を親に設定
        tank.transform.parent = gameObject.transform;

        return tank;
    }

    #endregion

    #region タンクに関する処理

    /// <summary>
    /// タンクの移動を行う関数
    /// isControlによって操作を受付するかどうかを決定する
    /// </summary>
    private void UpdateTank()
    {
        //内容は齋藤さんのをパクリ予定
        if (isControl)
        {
            //操作を取得
            InputControler();

            //操作を反映
            TankMove(new Vector3());
        }
    }

    /// <summary>
    /// コントローラーなどの入力を取得する
    /// 他のPCによって複製されたオブジェクトなら、他のPCの操作や情報を取得する
    /// </summary>
    private void InputControler()
    {
        //内容は齋藤さんのをパクリ
    }

    /// <summary>
    /// 取得した操作を所有しているタンクに反映する
    /// </summary>
    private void TankMove(Vector3 input)
    {
        //タンクに反映
    }

    /// <summary>
    /// タンクのヒット時にイベントハンドラー経由で呼ばれる
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        //ゲームマネージャーにヒットしたことを通知
        //GameManagerInstance

        //操作を停止
        isControl = false;
    }

    #endregion

    #region GMに関する処理

    /// <summary>
    /// このプレイヤー用のタンクを生成する
    /// 引数で初期位置付き、指定なしの場合は既に指定されていた位置に生成する
    /// 正常に生成されていればtrueが返る
    /// </summary>
    /// <returns></returns>
    public bool PopTank()
    {
        if(SpawnPoint == new Vector3(0, 0, 0))
        {
            Debug.LogWarning("初期位置が指定されていないか、0,0,0です");
        }

        //所持しているタンクに代入
        PossessionTank = TankSpawn().GetComponent<Rajikon>();

        if(PossessionTank == null)
        {
            Debug.LogWarning("タンクが生成されていません");
            return false;
        }
        return true;
    }

    /// <summary>
    /// このプレイヤー用のタンクを生成する
    /// 引数で初期位置付き、指定なしの場合は既に指定されていた位置に生成する
    /// 正常に生成されていればtrueが返る
    /// </summary>
    /// <returns></returns>
    public bool PopTank(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;

        if (SpawnPoint == new Vector3(0, 0, 0))
        {
            Debug.LogWarning("初期位置が指定されていないか、0,0,0です");
        }

        //所持しているタンクに代入
        PossessionTank = TankSpawn().GetComponent<Rajikon>();

        if (PossessionTank == null)
        {
            Debug.LogWarning("タンクが生成されていません");
            return false;
        }
        return true;
    }

    public void SetSpawnPoint(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;
    }

    public void SetisControl(bool iscontrol)
    {
        isControl = iscontrol;
    }

    #endregion
}
