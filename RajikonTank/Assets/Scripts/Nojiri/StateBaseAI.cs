using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;   //敵属性の設定
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT; //敵の初期遷移

    private Rajikon rajikon;        // Rajikonクラス
    private CPUInput cpuInput;      // CPUInputクラス
    private GameObject player;      // プレイヤー情報
    private GameObject enemy;       // エネミー情報
    private GameObject grandChild;  // 孫オブジェクト
    private Vector3 enemyPos;       // 敵(自分)の位置
    private Vector3 playerPos;      // プレイヤーの位置
    private string playerTag;       // Playerのtag
    private bool isInit   = false;  // 初期化状態確認
    private bool isAttack = false;  // 攻撃間隔用フラグ
    private bool isTimer  = false;  // タイマーフラグ

    public enum EnemyName // 敵種類
    {
        NORMAL,              // 通常敵
        REFLECT              // 反射敵
    }

    public enum EnemyAiState // 行動パターン
    {
        WAIT,                // 待機
        MOVE,                // 移動
        TURN,                // 旋回
        ATTACK,              // 攻撃
        AVOID,               // 回避
        DEATH,               // 死亡
    }

    // Start is called before the first frame update
    void Start()
    {
        // PlayerInputクラス取得
        rajikon = gameObject.GetComponent<Rajikon>();
        cpuInput = gameObject.GetComponent<CPUInput>();

        // ゲームオブジェクトで生成されたPlayerを取得
        player = GameManager.instance.teamInfo[GameManager.instance.player_IDnum].tankList[0].gameObject.transform.Find("Tank").gameObject;
        playerTag = player.tag; // tagを取得

        enemy = rajikon.Tank.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
    }

    #region AI遷移ループ
    /// <summary>
    /// 敵の種類によって行動遷移を変更
    /// </summary>
    private void UpdateAI()
    {
        InitAI();

        // INGAMEでないとき、初期化処理以外を開始しない
        if (GameManager.instance.NowGameState != GAMESTATUS.INGAME)
        {
            return;
        }

        switch (aiName)
        {
            case EnemyName.NORMAL:
                Normal();
                break;
            case EnemyName.REFLECT:
                //Reflect();
                break;
            default:
                break;
        }
    }
    #endregion

    #region 初期化処理
    /// <summary>
    /// 初期化実行
    /// チームID、敵生成
    /// </summary>
    private void InitAI()
    {
        enemyPos = enemy.transform.position;
        playerPos = player.transform.position;

        if (isInit == true)
        {
            return;
        }

        isInit = true;

        rajikon.SetPlayerInput(cpuInput);
        rajikon.SetEventHandler(this); // タンクのイベントを通知する

        AddTeam(); // チーム追加
    }

    /// <summary>
    /// チームID送信
    /// </summary>
    public void AddTeam()
    {
        // CPUのチームIDを送ってチームに追加
        GameManager.instance.PushTank(TeamID.CPU, rajikon);
    }
    #endregion

    #region 通常敵遷移
    /// <summary>
    /// AI状態遷移
    /// </summary>
    private void Normal()
    {
        NormalAiRoutine();

        switch (aiState)
        {
            case EnemyAiState.WAIT:
                break;
            case EnemyAiState.MOVE:
                Move();
                break;
            case EnemyAiState.TURN:
                Turn();
                break;
            case EnemyAiState.ATTACK:
                Attack();
                break;
            case EnemyAiState.AVOID:
                break;
            case EnemyAiState.DEATH:
                EnemyDeath();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Rayに触れたオブジェクトによるStateの割り当て
    /// Playerの場合　：攻撃
    /// それ以外の場合：移動or旋回
    /// </summary>
    private void NormalAiRoutine()
    {
        // AiTimer実行中
        if(isTimer == true || aiState == EnemyAiState.DEATH)
        {
            return;
        }

        RaycastHit hit;         // Rayが衝突したオブジェクト情報
        Vector3 fireDirection;  // 発射方向  
        bool attackFlg;         // 攻撃判定フラグ

        fireDirection = (playerPos - enemyPos).normalized;

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        if (Physics.Raycast(enemyPos, fireDirection, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == playerTag && hitObj == player) // Playerと自分の間に遮蔽物がないとき
            {
                //attackFlg = TurretDirection(); // 砲台がPlayerに向いているかどうか

                //if (attackFlg) aiState = EnemyAiState.ATTACK; // true ：攻撃
                //else aiState = EnemyAiState.TURN;             // false：旋回
                aiState = EnemyAiState.MOVE;
            }
            else
            {
                aiState = EnemyAiState.WAIT;   // 待機
            }
        }
        else
        {
            aiState = EnemyAiState.WAIT;   // 待機
            Debug.LogError("Rayが当たりませんでした。");
        }

        StartCoroutine(AiTimer());
    }

    /// <summary>
    /// 砲台の向きを参照し、攻撃判定を返す
    /// Playerに向いているとき：ture
    /// それ以外　　　　　　　：false
    /// </summary>
    private bool TurretDirection()
    {
        grandChild = transform.GetChild(0).GetChild(1).gameObject;   // 順番からTurret取得
        RaycastHit turretHit;   // レイに衝突したオブジェクト情報

        if (grandChild == null) return false;

        // 砲台の前方にRayを発射
        if (Physics.SphereCast(enemyPos, 0.5f, grandChild.transform.forward, out turretHit))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            if (turretHitObj.tag == playerTag && turretHitObj == player)
            {
                // 攻撃可
                return true;
            }
            else
            {
                // 攻撃不可
                return false;
            }
        }
        return false;
    }
    #endregion

    #region 各種タイマー
    // 毎フレーム処理による負荷の軽減用タイマー
    private IEnumerator AiTimer()
    {
        isTimer = true;
        yield return new WaitForSeconds(1); // 1秒ごとに実行

        isTimer = false;
    }

    // 指定秒数ごとに攻撃処理を実行するタイマー
    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(5);
        isAttack = false;
    }
    #endregion

    #region 行動遷移ごとのメソッド
    /// <summary>
    /// 移動用メソッド
    /// Playerとの位置関係を計算
    /// キーボード処理に変換してPlayerを追従
    /// </summary>
    private void Move()
    {
        float enemyMovePos = VectorCalc(); // Playerまでの角度を受け取る

        Conversion(enemyMovePos); // キーボード変換
    }

    /// <summary>
    /// 攻撃用メソッド
    /// 弾を発射後は、5秒間弾を打てなくする
    /// </summary>
    private void Attack()
    {
        cpuInput.moveveckey = KeyList.NONE; // キーボード入力を初期化

        if (!isAttack)
        {
            isAttack = true; // 攻撃中
            cpuInput.moveveckey = KeyList.FIRE; // 攻撃ボタン押下

            StartCoroutine(AttackTimer()); // 指定秒数後に処理を再開
        }
    }

    // 旋回用メソッド
    private void Turn()
    {
        cpuInput.sendtarget = playerPos; // Playerの方向を向く
    }

    /// <summary>
    /// 死亡用メソッド
    /// 死亡処理をGameManagerに送信後にDestroyする
    /// </summary>
    private void EnemyDeath()
    {
        GameManager.instance.DeathTank(TeamID.CPU); // GameManagerに死亡処理送信

        Destroy(gameObject); // 敵削除
    }

    /// <summary>
    /// bulletに当たった時に呼ばれるメソッド
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        aiState = EnemyAiState.DEATH;  // 死亡遷移に移行
    }
    #endregion

    #region ベクトル変換
    /// <summary>
    /// EnemyからPlayerのいる角度を計算
    /// 求めた角度を8方向に分割し、Vector2(-1〜1, -1〜1)に変換
    /// </summary>
    private float VectorCalc()
    {
        //// 内積を求め、角度に変換(内積＊角度)
        //float VectorX = playerPos.x - enemyPos.x;
        //float VectorZ = playerPos.z - enemyPos.z;
        //float Radian = Mathf.Atan2(VectorZ, VectorX) * Mathf.Rad2Deg;

        ////角度表示変更
        //if (Radian < 0)
        //{
        //    // -180〜180で返るため、0〜360に変換
        //    Radian += 360;
        //}

        //// 360度を4分割し、四捨五入する(0〜8)
        //int Division = Mathf.RoundToInt(Radian / 90.0f);

        //// 4の場合0に変換(9等分防止)
        //if (Division == 4) Division = 0;

        Vector3 relativePos = playerPos - enemyPos; // EnemyからPlayerへの相対ベクトル
        Quaternion lookAngle = Quaternion.LookRotation(relativePos.normalized);        // Playerに向いた時の角度
        float rotateAngle = Quaternion.Angle(enemy.transform.localRotation,lookAngle); // 現在の角度からPlayerまでの間の角度

        return rotateAngle; // Playerまでの角度を返す
    }

    /// <summary>
    /// ベクトル変換メソッド
    /// 受け取った角度の大きさに応じてキーボード変換
    /// </summary>
    /// <param name="index">現在の角度からPlayerまでの角度</param>
    private void Conversion(float index)
    {
        // 0〜180
        if (index <= 10f)
        {
            cpuInput.moveveckey = KeyList.W;
        }
        else if(index > 10f && index <= 170f)
        {
            //cpuInput.moveveckey = KeyList.A;
            Debug.Log("旋回");
        }
        else if(index > 170f)
        {
            cpuInput.moveveckey = KeyList.S;
        }
    }
    #endregion
}
