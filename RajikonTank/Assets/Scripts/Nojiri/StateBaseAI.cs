using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;   //敵属性の設定
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT;   //敵の初期遷移
    public int maxDistance; // 攻撃可能範囲

    private Rajikon rajikon;           // Rajikonクラス
    private CPUInput cpuInput;         // CPUInputクラス
    private GameObject player;         // プレイヤー情報
    private GameObject enemy;          // エネミー情報
    private GameObject grandChild;     // 孫オブジェクト

    private Quaternion lookAngle;      // Playerがいる方向
    private Vector3 enemyPos;          // 敵(自分)の位置
    private Vector3 playerPos;         // プレイヤーの位置

    private string playerTag;          // Playerのtag
    private float nowDistance;         // プレイヤーとの距離
    private bool isInit   = false;     // 初期化状態確認
    private bool isAttack = false;     // 攻撃間隔用フラグ
    private bool isTimer  = false;     // タイマーフラグ
    private bool canAttack = true;     // 攻撃可否フラグ
    private const int rayLength = 50;  // Rayの長さ

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

    #region 名前遷移/メインループ
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

        EnemyMain();             // 敵共通処理
        EnemyStateTransition();  // 敵状態遷移
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
    private  void AddTeam()
    {
        // CPUのチームIDを送ってチームに追加
        GameManager.instance.PushTank(TeamID.CPU, rajikon);
    }
    #endregion

    #region 
    /// <summary>
    /// 敵共通メソッド
    /// Rayに触れたオブジェクトによるStateの割り当て
    /// Playerの場合　：敵名前ごとに処理の変更
    /// それ以外の場合：待機
    /// </summary>
    private void EnemyMain()
    {
        RaycastHit hit;         // Rayが衝突したオブジェクト情報
        Vector3 fireDirection;  // 発射方向  
        bool isFacingPlayer;    //　Playerを向いているかどうか

        nowDistance = Vector3.Distance(playerPos, enemyPos); // プレイヤーとの距離

        // AiTimer実行中
        if (isTimer == true || aiState == EnemyAiState.DEATH)
        {
            return;
        }

        fireDirection = (playerPos - enemyPos).normalized;

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        if (Physics.Raycast(enemyPos, fireDirection, out hit, rayLength))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == playerTag && hitObj == player) // Playerと自分の間に遮蔽物がないとき
            {
                EnemyNameTransition(); // 敵の名前によって処理を変更

                // falseの場合、攻撃遷移へ移行しない
                if (!canAttack)
                {
                    return;
                }

                isFacingPlayer = TurretDirection(); // 砲台がPlayerに向いているかどうか

                if (isFacingPlayer) aiState = EnemyAiState.ATTACK; // true ：攻撃
                else aiState = EnemyAiState.TURN;             // false：旋回
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
    /// 敵の名前によって行動遷移を変更
    /// </summary>
    private void EnemyNameTransition()
    {
        switch (aiName)
        {
            case EnemyName.TUTORIAL:
                TutorialEnemy();
                break;
            case EnemyName.NORMAL:
                NormalEnemy();
                break;
            case EnemyName.MOVEMENT:
                MoveEnemy();
                break;
            case EnemyName.FASTBULLET:
                FastBulletEnemy();
                break;
            case EnemyName.FASTANDMOVE:
                FastAndMoveEnemy();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 砲台の向きを参照し、攻撃判定を返す
    /// Playerに向いているとき：ture
    /// それ以外　　　　　　　：false
    /// </summary>
    private bool TurretDirection()
    {
        const float radius = 0.5f; //発射するRayの半径

        grandChild = transform.GetChild(0).GetChild(1).gameObject;   // 順番からTurret取得
        RaycastHit turretHit;   // レイに衝突したオブジェクト情報

        if (grandChild == null) return false;

        // 砲台の前方にRayを発射
        if (Physics.SphereCast(enemyPos, radius, grandChild.transform.forward, out turretHit, rayLength))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            // プレイヤーを向いているかどうか
            if (turretHitObj.tag == playerTag && turretHitObj == player)
            {
                // 向いている
                return true;
            }
            else
            {
                // 向いていない
                return false;
            }
        }
        return false;
    }
    #endregion

    #region 敵状態遷移
    /// <summary>
    /// 全ての敵で使用する状態遷移
    /// </summary>
    private void EnemyStateTransition()
    {
        switch (aiState)
        {
            case EnemyAiState.WAIT:
                Wait();
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
    #endregion

    #region 敵ごとのメソッド

    #region チュートリアル敵遷移
    private void TutorialEnemy()
    {
        aiName = EnemyName.TUTORIAL; // GameManagerに送るID設定

        aiState = EnemyAiState.WAIT; // 待機
        canAttack = false; // 攻撃不可
    }
    #endregion

    #region 通常敵遷移
    /// <summary>
    /// 通常敵設定
    /// </summary>
    private void NormalEnemy()
    {
        aiName = EnemyName.NORMAL; // GameManagerに送るID設定

        canAttack = true; // 攻撃可
    }
    #endregion

    #region 移動敵遷移
    /// <summary>
    /// 移動敵設定
    /// </summary>
    private void MoveEnemy()
    {
        const int maxDistance = 10; // 攻撃可能範囲

        aiName = EnemyName.MOVEMENT; // GameManagerに送るID設定
        aiState = EnemyAiState.MOVE; // 移動

        // プレイヤーとの距離が一定値以上の時
        if (nowDistance > maxDistance)
        {
            canAttack = false; // 攻撃不可
        }
        else
        {
            canAttack = true; // 攻撃可
        }
    }
    #endregion

    #region 高速弾敵
    /// <summary>
    /// 高速弾敵設定
    /// </summary>
    private void FastBulletEnemy()
    {
        aiName = EnemyName.FASTBULLET; // GameManagerに送るID設定

        canAttack = true; // 攻撃可
    }
    #endregion

    #region 高速弾＆移動敵
    /// <summary>
    /// 移動＆高速弾敵設定
    /// </summary>
    private void FastAndMoveEnemy()
    { 

        aiName = EnemyName.FASTANDMOVE; // GameManagerに送るID設定
        aiState = EnemyAiState.MOVE; // 移動

        // プレイヤーとの距離が一定値以上の時
        if (nowDistance > maxDistance)
        {
            canAttack = false; // 攻撃不可
        }
        else
        {
            canAttack = true; // 攻撃可
        }
    }
    #endregion

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
        int second; // 発射間隔

        // 早い弾を撃つ敵の場合、発射間隔を変更
        if (aiName == EnemyName.FASTBULLET || aiName == EnemyName.FASTANDMOVE)
        {
            second = 5;
        }
        else
        {
            second = 3;
        }
            yield return new WaitForSeconds(second);
        isAttack = false;
    }
    #endregion

    #region 行動遷移ごとのメソッド
    /// <summary>
    /// 待機処理
    /// 変数の初期化
    /// </summary>
    private void Wait()
    {
        cpuInput.moveveckey = KeyList.NONE;
    }

    /// <summary>
    /// 移動用メソッド
    /// Playerとの位置関係を計算
    /// キーボード処理に変換してPlayerを追従
    /// </summary>
    private void Move()
    {
        Debug.Log("Move実行");
        float enemyMovePos = VectorCalc(); // Playerまでの角度を受け取る

        ConversionKey(enemyMovePos); // キーボード変換
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
        GameManager.instance.DeathTank(TeamID.CPU,aiName); // GameManagerに死亡処理送信

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
    /// 求めた角度を8方向に分割し、Vector2(-1～1, -1～1)に変換
    /// </summary>
    private float VectorCalc()
    {
        Vector3 relativePos = playerPos - enemyPos; // EnemyからPlayerへの相対ベクトル
        lookAngle = Quaternion.LookRotation(relativePos.normalized);  // Playerに向いた角度
        float rotateAngle = Quaternion.Angle(enemy.transform.localRotation,lookAngle); // 現在の角度からPlayerまでの間の角度

        return rotateAngle; // Playerまでの角度を返す
    }

    /// <summary>
    /// キーボード変換メソッド
    /// 受け取った角度の大きさに応じてキーボード変換
    /// </summary>
    /// <param name="index">現在の角度からPlayerまでの角度0～1、0～-1を受け取る</param>
    private void ConversionKey(float index)
    {
        const int forwardZero = 0;
        const int forwardAngle  = 20;    // 前方角度
        const int backwardAngle = 160;   // 後方角度
        const float leftAngle   = -0.5f; // 左前後確認用
        const float rightAngle  = 0.5f;  // 右前後確認用

        if (index < forwardAngle) // Playerが前方角度内にいるとき
        {
            cpuInput.moveveckey = KeyList.ACCELE;
        }
        else if(index >= forwardAngle && index <= backwardAngle) // Playerが中方角度内にいるとき
        {
            // 左にいるとき
            if (lookAngle.y < forwardZero && lookAngle.y < leftAngle)
            {
                // 前方後方の近さによって回る向きを変える
                if (lookAngle.y > leftAngle)
                {
                    cpuInput.moveveckey = KeyList.LEFTROTATION;
                    Debug.Log("左前方");
                }
                else if(lookAngle.y < leftAngle)
                {
                    cpuInput.moveveckey = KeyList.RIGHTROTATION;
                    Debug.Log("左後方");
                }
            }

            // 右にいるとき
            if (lookAngle.y > forwardZero)
            {
                // 前方後方の近さによって回る向きを変える
                if (lookAngle.y < rightAngle)
                {
                    cpuInput.moveveckey = KeyList.RIGHTROTATION;
                    Debug.Log("右前方");
                }
                else if(lookAngle.y >= rightAngle)
                {
                    cpuInput.moveveckey = KeyList.LEFTROTATION;
                    Debug.Log("右後方");
                }
            }
        }
        else if(index > backwardAngle) // Playerが後方角度内にいるとき
        {
            cpuInput.moveveckey = KeyList.BACK;
        }
        else
        {
            cpuInput.moveveckey = KeyList.NONE;
        }
    }
    #endregion
}