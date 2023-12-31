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
    public Vector3[] movePos;
    public bool patrolMode = false;   // 巡回モードON/OFF

    private Rajikon rajikon;          // Rajikonクラス
    private CPUInput cpuInput;        // CPUInputクラス
    private GameObject player;        // プレイヤー情報
    private GameObject enemy;         // エネミー情報
    private GameObject grandChild;    // 孫オブジェクト

    private Vector3 enemyPos;         // 敵(自分)の位置
    private Vector3 playerPos;        // プレイヤーの位置
    public List<Vector3> patrolPoints;      // 巡回する位置

    private string playerTag;         // Playerのtag
    private float nowDistance;        // プレイヤーとの距離
    private bool isInit   = false;    // 初期化状態確認
    private bool isAttack = false;    // 攻撃間隔用フラグ
    private bool isTimer  = false;    // タイマーフラグ
    private bool isBomber = false;    // 地雷設置フラグ
    private bool canAttack = true;    // 攻撃可否フラグ
    private const int rayLength = 50; // Rayの長さ
    private int points = 0;      // 巡回地点の番号

    public enum EnemyAiState // 行動パターン
    {
        WAIT,                // 待機
        PATROL,              // 巡回
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
        playerTag = player.tag; // Tag取得

        enemy = rajikon.Tank.transform.gameObject; // 敵情報取得
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

    #region 敵共通処理
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
        bool isTurnPlayer;    //　Playerを向いているかどうか

        nowDistance = Vector3.Distance(playerPos, enemyPos); // プレイヤーとの距離

        // AiTimer実行中
        if (isTimer == true || aiState == EnemyAiState.DEATH)
        {
            return;
        }

        // タイマー実行
        StartCoroutine(AiTimer());

        fireDirection = (playerPos - enemyPos).normalized;

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        if (Physics.Raycast(enemyPos, fireDirection, out hit, rayLength))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == playerTag && hitObj == player) // Playerと自分の間に遮蔽物がないとき
            {
                // 敵の名前によって処理を変更
                EnemyNameTransition();

                // 射程圏外の場合、攻撃遷移へ移行しない
                if (!canAttack)
                {
                    return;
                }

                // 地雷敵かつクールタイム中のとき、巡回に切り替え
                if (isBomber && aiName == EnemyName.BOMBER)
                {
                    StartCoroutine(BomCoolTimer()); // クールタイム後にフラグをfalse

                    if (!patrolMode)
                    {
                        Debug.Log("地雷敵のPatrolModeがONになっていません");
                        return;
                    }
                    aiState = EnemyAiState.PATROL;

                    return;
                }

                isTurnPlayer = TurretDirection(); // 砲台がPlayerに向いているかどうか
                if (isTurnPlayer)
                {
                    aiState = EnemyAiState.ATTACK; // true ：攻撃
                    isBomber = true; // 地雷敵クールタイム

                }
                else aiState = EnemyAiState.TURN;             // false：旋回
            }
            else
            {
                // patrolModeがONの時、敵を巡回させる
                // 巡回地点に到着したら次の巡回地点に目標を変える
                if (patrolMode)
                {
                    aiState = EnemyAiState.PATROL;
                }
                else
                {
                    aiState = EnemyAiState.WAIT;   // 待機
                }
            }
        }
        else
        {
            aiState = EnemyAiState.WAIT;   // 待機
            Debug.LogError("Rayが当たりませんでした。");
        }
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
            case EnemyName.FAST_BULLET:
                FastBulletEnemy();
                break;
            case EnemyName.FAST_AND_MOVE:
                FastAndMoveEnemy();
                break;
            case EnemyName.BOMBER:
                BomberEnemy();
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
            case EnemyAiState.PATROL:
                Patrol();
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
        canAttack = true; // 攻撃可
    }
    #endregion

    #region 移動敵遷移
    /// <summary>
    /// 移動敵設定
    /// </summary>
    private void MoveEnemy()
    {
        maxDistance = 10; // 攻撃可能範囲

        aiState = EnemyAiState.MOVE; // 移動

        AttackChange();   // 攻撃可能距離かどうか
    }
    #endregion

    #region 高速弾敵
    /// <summary>
    /// 高速弾敵設定
    /// </summary>
    private void FastBulletEnemy()
    {
        canAttack = true; // 攻撃可
    }
    #endregion

    #region 高速弾＆移動敵
    /// <summary>
    /// 移動＆高速弾敵設定
    /// </summary>
    private void FastAndMoveEnemy()
    {
        maxDistance = 25;  // 攻撃可能範囲

        aiState = EnemyAiState.MOVE; // 移動

        AttackChange();    // 攻撃可能距離かどうか
    }
    #endregion

    /// <summary>
    /// 地雷敵設定
    /// 基本動作は移動敵の同じ
    /// </summary>
    #region 地雷敵
    private void BomberEnemy()
    {
        maxDistance = 11; // 攻撃可能範囲

        aiState = EnemyAiState.MOVE; // 移動

        AttackChange();  // 攻撃可能距離かどうか
    }

    // 攻撃可能フラグの切り替え
    private void AttackChange()
    {
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

    // 指定秒数ごとに攻撃処理を実行するタイマー（連射防止）
    private IEnumerator AttackTimer()
    {
        float second; // 発射間隔

        // 敵に種類に応じて発射間隔を変更
        if (aiName == EnemyName.FAST_BULLET || aiName == EnemyName.FAST_AND_MOVE)
        {
            second = Random.Range(3, 5);
        }
        else
        {
            second = Random.Range(1, 3);
        }
        
        yield return new WaitForSeconds(second);
        isAttack = false;
    }

    private IEnumerator BomCoolTimer()
    {
        // 指定秒後に地雷敵クールタイムのリセット
        yield return new WaitForSeconds(5);
        Debug.Log("タイマー終了");
        isBomber = false;
    }
    #endregion

    #region 行動遷移ごとのメソッド
    /// <summary>
    /// 待機処理
    /// 変数の初期化
    /// </summary>
    private void Wait()
    {
        cpuInput.moveveckey = KeyList.NONE; // 初期状態
    }

    /// <summary>
    /// 巡回用メソッド
    /// patrolModeがTrueの時に呼ばれる
    /// 配列内の座標に向かって順番に移動する
    /// </summary>
    private void Patrol()
    {
        float minDistance = 3f; // 巡回地点の切替距離
        int maxArray;           // 巡回地点数取得用

        // 配列に要素が入っていない時
        if (patrolPoints == null)
        {
            Debug.LogError("patrolPointsに要素が入っていません");
            return;
        }

        // 配列の要素数取得
        maxArray = patrolPoints.Count;

        // 指定の方向を向く
        cpuInput.sendtarget = patrolPoints[points];

        // patrolPointsに対しての8分割した角度取得
        int patorolPosDiv = VectorConversion(patrolPoints[points]);

        // 移動処理
        ConversionKey(patorolPosDiv);

        // 2点間の距離計算
        float PointToDistance = Vector3.Distance(patrolPoints[points], enemyPos);

        // 巡回地点に接近した時、次の巡回地点に変更
        if(PointToDistance < minDistance)
        {
            points++;
        }

        // 最後の巡回地点に着いたとき、初めの巡回地点に戻る
        if (points >= maxArray)
        {
            points = 0;
        }
    }

    /// <summary>
    /// 移動用メソッド
    /// Playerとの位置関係を計算
    /// キーボード処理に変換してPlayerを追従
    /// </summary>
    private void Move()
    {
        // 射程圏外のとき
        if (!canAttack)
        {
            int enemyMovePos = VectorConversion(playerPos); // Playerまでの角度を受け取る
            ConversionKey(enemyMovePos); // キーボード変換
        }
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

            if(aiName == EnemyName.BOMBER)
            {
                rajikon.AddBomb(1);
                cpuInput.moveveckey = KeyList.PLANT; // 地雷ボタン押下
                Debug.Log("地雷設置");
            }
            else
            {
                cpuInput.moveveckey = KeyList.FIRE;  // 攻撃ボタン押下
            }

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
        // GameManagerに死亡処理送信
        GameManager.instance.DeathTank(TeamID.CPU,aiName);

        Destroy(gameObject);
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
    /// ベクトル変換メソッド
    /// 敵からTargetへの角度を0〜7に変換
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    private int VectorConversion(Vector3 Target)
    {
        // EnemyからTargetへの相対ベクトル取得
        float vectorX = Target.x - enemyPos.x;
        float vectorZ = Target.z - enemyPos.z;

        // 敵からTargetに対しての角度
        float Radian = Mathf.Atan2(vectorZ, vectorX) * Mathf.Rad2Deg;

        // 計算で出た角度が-180〜180なため、0〜360に変換
        if (Radian < 0)
        {
            Radian += 360;
        }

        // 360度を8分割(実際は0〜8の9分割)
        int Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8と0が被るため修正
        if (Division == 8) Division = 0;

        return Division;
    }

    /// <summary>
    /// キーボード変換メソッド
    /// 受け取った角度に応じてキーボード変換
    /// </summary>
    /// <param name="index">現在の角度からTargetまでの角度を受け取る</param>
    private void ConversionKey(int index)
    {
        switch (index)
        {
            case 0:
                cpuInput.moveveckey = KeyList.D;   // 右
                break;
            case 1:
                cpuInput.moveveckey = KeyList.WD;  // 右上
                break;
            case 2:
                cpuInput.moveveckey = KeyList.W;   // 上
                break;
            case 3:
                cpuInput.moveveckey = KeyList.WA;  // 左上
                break;
            case 4:
                cpuInput.moveveckey = KeyList.A;   // 左
                break;
            case 5:
                cpuInput.moveveckey = KeyList.SA;  // 左下
                break;
            case 6:
                cpuInput.moveveckey = KeyList.S;   // 下
                break;
            case 7:
                cpuInput.moveveckey = KeyList.SD;  // 右下
                break;
            default:
                Debug.LogError("patrolPosDivエラー");
                break;
        }
    }
    #endregion

    #region 外部用メソッド
    /// <summary>
    /// 送られた巡回位置リストを取得
    /// </summary>
    /// <param name="points">巡回する位置情報リスト</param>
    public void SetPatrolPoint(List<Vector3> points)
    {
        patrolPoints = points;
    }
    #endregion
}