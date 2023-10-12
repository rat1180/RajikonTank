using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;
using UnityEngine.InputSystem;

/// <summary>
/// ユーザー一人に割り当てられるプレイヤークラス
/// タンクで行う操作とゲームマネージャーとの橋渡しを行うイメージ
/// このオブジェクトにはPlayerInputクラスが割り当てられ、タンクはそれを参照して移動等を行うため、
/// 移動そのものはこのクラスでは行わない
/// </summary>
public class PlayerClass : TankEventHandler
{

    #region 列挙体・構造体・内部クラス

    /// <summary>
    /// 初期化のモードを指定する
    /// DEBUG以外はPopTankを呼ぶ必要がある
    /// 通常の初期化はDEFAULT
    /// ゲーム内に組み込む初期化はNATURAL
    /// デバッグ用にとりあえず生成はDEBUG
    /// </summary>
    public enum InitMode
    {
        /// <summary>
        /// 通常の処理で初期化される
        /// 初期位置：設定されてればそこに、されていなければ(0,0,0)に生成
        /// 他クラスへの依存：する
        /// </summary>
        DEFAULT,
        /// <summary>
        /// ステージに組み込める状態で初期化される
        /// 初期位置：設定すればその位置に、されていなければこのオブジェクトの位置に生成
        /// 他クラスへの依存：する
        /// </summary>
        NATURAL,
        /// <summary>
        /// デバッグ用に処理を簡易的に生成される
        /// 初期位置：このオブジェクトの位置
        /// 他クラスへの依存：しない(ただし、タンクと弾の名前を指定しなければいけない)
        /// </summary>
        DEBUG
    }

    /// <summary>
    /// デバッグモードでの初期化用に設定しなければならない設定
    /// それ以外のモードでは使用しない
    /// </summary>
    [System.Serializable]
    public class DebugModeSettings
    {

    }

    #endregion

    #region 変数

    #region 変更・セットが必須の変数
    [SerializeField, Tooltip("初期化モード(特になければNATURAL)"), Header("変更・セットが必須の変数")] public InitMode InitModeSelect;
    #endregion

    #region 処理に使う変数
    [SerializeField, Tooltip("ユーザーが操作するためのInputClass"),Header("このクラス内で処理に使う変数")] private PlayerInput PlayerInputScript;

    [SerializeField, Tooltip("ゲームマネージャーのインスタンス")] private GameManager GameManagerInstance;

    [SerializeField, Tooltip("最大弾数(デフォルト値は5)")] private int MaxBulletNm = 5;
    #endregion

    #region デバッグ用表示
    [SerializeField,Tooltip("このクラスが所持しているタンク"),Header("デバッグ用表示")] private Rajikon PossessionTank;

    [SerializeField,Tooltip("操作を受付するか")] private bool isControl;

    [SerializeField, Tooltip("所属しているチームID")] private TeamID TeamID;

    [SerializeField, Tooltip("このプレイヤーのスポーンポイント")] private Vector3 SpawnPoint;

    [SerializeField, Tooltip("残弾数")] private int RemainingBulletNm;
    #endregion

    #endregion

    #region Unityイベント
    // Start is called before the first frame update
    void Start()
    {
        if(InitModeSelect != InitMode.NATURAL)
        {
            //プレイヤーの初期化・テスト
            InitPlayer(InitModeSelect);
        }
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
    public void InitPlayer(InitMode mode)
    {
        //操作権限の初期化
        isControl = false;

        //キー入力取得用スクリプト確認・追加
        CheckPlayerInputScript();

        //モードに合わせた処理
        switch (mode)
        {
            case InitMode.DEFAULT:
                CheckGameManagerInstance();
                break;
            case InitMode.NATURAL:
                SetSpawnPoint(transform.position);
                CheckGameManagerInstance();
                PopTank();
                break;
            case InitMode.DEBUG:
                SetSpawnPoint(transform.position);

                //テスト
                PopTank();
                break;
            default:
                break;
        }

        //テスト
        //PopTank();

        //エラー確認
        if (PossessionTank == null)
        {
            Debug.LogWarning("初期化時にタンクが生成されていません");
        }

        //テスト
        isControl = true;
    }

    /// <summary>
    /// ゲームマネージャーインスタンスを確認し、
    /// あればセットする
    /// </summary>
    private void CheckGameManagerInstance()
    {
        //マネージャーの取得
        GameManagerInstance = GameManager.instance;
    }

    /// <summary>
    /// PlayerInputがセットされているかを確認し、
    /// されていなければセットする
    /// </summary>
    private void CheckPlayerInputScript()
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
        var tank = TankGenerateClass.TankInstantiate(TankPrefabNames.TankBase);

        //タンクの初期化
        tank.GetComponent<Rajikon>().SetPlayerInput(PlayerInputScript);
        //add.h
        tank.GetComponent<Rajikon>().SetEventHandler(this);
        RemainingBulletNm = MaxBulletNm;

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
        Mouse mouse = Mouse.current;
        Vector3 mousepos = mouse.position.ReadValue();
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
        GameManagerInstance.DeathTank(TeamID);

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

        //チームへ追加
        if(GameManagerInstance != null) GameManagerInstance.PushTank(TeamID, PossessionTank);
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

    #region セッター・ゲッター

    /// <summary>
    /// 残り残弾数を取得する
    /// デフォルトの値は5であり、撃つたびに減っているので
    /// 残弾数の表示などは随時取得する
    /// </summary>
    /// <returns></returns>
    public int GetBulletCount()
    {
        return RemainingBulletNm;
    }

    /// <summary>
    /// スポーンするポジションの設定
    /// </summary>
    /// <param name="spawnpoint"></param>
    public void SetSpawnPoint(Vector3 spawnpoint)
    {
        SpawnPoint = spawnpoint;
    }

    public void SetisControl(bool iscontrol)
    {
        isControl = iscontrol;
    }

    #endregion

    #endregion
}
