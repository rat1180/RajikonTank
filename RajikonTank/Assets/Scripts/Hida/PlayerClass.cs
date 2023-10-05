using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

/// <summary>
/// ユーザー一人に割り当てられるプレイヤークラス
/// タンクで行う操作とゲームマネージャーとの橋渡しを行うイメージ
/// このオブジェクトにはPlayerInputクラスが割り当てられ、タンクはそれを参照して移動等を行うため、
/// 移動そのものはこのクラスでは行わない
/// </summary>
public class PlayerClass : TankEventHandler
{

    #region 変数
    [SerializeField, Tooltip("ユーザーが操作するためのInputClass")] private PlayerInput PlayerInputScript;

    [SerializeField,Tooltip("このクラスが所持しているタンク"),Header("デバッグ用表示")] private Rajikon PossessionTank;

    [SerializeField,Tooltip("操作を受付するか")] private bool isControl;

    [SerializeField, Tooltip("ゲームマネージャーのインスタンス")] private GameManager GameManagerInstance;

    [SerializeField, Tooltip("所属しているチームID")] private TeamID TeamID;

    [SerializeField, Tooltip("このプレイヤーのスポーンポイント")] private Vector3 SpawnPoint;
    #endregion

    #region Unityイベント
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
    #endregion

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

        //キー入力取得用スクリプト確認・追加
        SetPlayerInputScript();

        //テスト
        PopTank();

        //エラー確認
        if (PossessionTank == null)
        {
            Debug.LogWarning("初期化時にタンクが生成されていません");
        }

        //チームへ追加
        GameManagerInstance.PushTank(TeamID, PossessionTank);

        isControl = true;
        
    }

    /// <summary>
    /// PlayerInputがセットされているかを確認し、
    /// されていなければセットする
    /// </summary>
    private void SetPlayerInputScript()
    {
        //セットされているか確認
        if(PlayerInputScript == null)
        {
            //持っていればそれを、いなければ追加してセット
            PlayerInputScript = gameObject.AddComponent<PlayerInput>();
        }
    }

    /// <summary>
    /// タンクを生成する
    /// </summary>
    /// <returns></returns>
    private GameObject TankSpawn()
    {
        //タンクの生成
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.Rajikon);

        //タンクの初期化
        tank.GetComponent<Rajikon>().SetPlayerInput(PlayerInputScript);

        //タンクの初期位置を設定
        tank.transform.position = SpawnPoint;

        //自分を親に設定
        tank.transform.parent = gameObject.transform;

        return tank;
    }

    #endregion

    #region タンクに関する処理

    /// <summary>
    /// タンクへの操作の反映を行う関数
    /// isControlによって操作を受付するかどうかを決定する
    /// </summary>
    private void UpdateTank()
    {
        if (isControl)
        {
            //操作を取得
            InputControler();
        }
    }

    /// <summary>
    /// コントローラーなどの入力を取得する
    /// 他のPCによって複製されたオブジェクトなら、他のPCの操作や情報を取得する
    /// </summary>
    private void InputControler()
    {
        //マウスの先をPlayerInputに反映する
        PlayerInputScript.sendtarget = GetMousePos();
    }

    /// <summary>
    /// マウス座標を取得する
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMousePos()
    {
        Vector3 mousepos = Input.mousePosition;
        mousepos.z = 10;
        mousepos = Camera.main.ScreenToWorldPoint(mousepos);
        return mousepos;
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
        SetisControl(false);
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
