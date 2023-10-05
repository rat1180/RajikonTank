using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;
using TankClassInfomations;

public class StateBaseAI : TankEventHandler
{
    public EnemyName aiName = EnemyName.NORMAL;  //敵属性の設定
    [SerializeField] private EnemyAiState aiState = EnemyAiState.WAIT; //敵の初期遷移

    private GameObject player;     // プレイヤー取得
    private GameObject grandChild; // 孫オブジェクト取得
    private Vector3 enemyPos;      // 敵(自分)の位置取得
    private Vector3 playerPos;     // プレイヤーの位置取得
    private bool isInit  = false;  // 初期化状態確認
    private bool isTimer = false;  // タイマーフラグ
    private const int shotNum = 1; // 発射数


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
        // (仮)
        player = GameObject.Find("Player");
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
        enemyPos = transform.position;
        playerPos = player.transform.position;

        if (isInit == true)
        {
            return;
        }

        AddTeam(); // チーム追加

        Debug.Log("初期化実行");
        isInit = true;
    }

    /// <summary>
    /// チームID送信
    /// </summary>
    public void AddTeam()
    {
        // CPUのチームIDを送ってチームに追加
    }
    #endregion

    #region 通常敵遷移
    /// <summary>
    /// AI状態遷移
    /// </summary>
    private void Normal()
    {
        if(isTimer == false)
        {
            NormalAiRoutine();

            switch (aiState)
            {
                case EnemyAiState.WAIT:
                    Debug.Log("待機");
                    break;
                case EnemyAiState.MOVE:
                    Debug.Log("移動");
                    VectorCalc();
                    break;
                case EnemyAiState.TURN:
                    Debug.Log("旋回");
                    Turn();
                    break;
                case EnemyAiState.ATTACK:
                    Debug.Log("射撃");
                    Attack();
                    break;
                case EnemyAiState.AVOID:
                    Debug.Log("回避");
                    break;
                case EnemyAiState.DEATH:
                    Debug.Log("死亡");
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Rayに触れたオブジェクトによるStateの割り当て
    /// Playerの場合　：攻撃
    /// それ以外の場合：移動or旋回
    /// </summary>
    private void NormalAiRoutine()
    {
        RaycastHit hit; // Rayが衝突したオブジェクト情報
        bool attackFlg; // 攻撃判定フラグ

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        //if (Physics.Raycast(enemyPos, playerPos, out hit))
        if (Physics.Raycast(enemyPos, playerPos, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == "Player")
            {
                attackFlg = TurretDirection();
                //if(attackFlg) aiState = EnemyAiState.ATTACK; // 攻撃
                //else aiState = EnemyAiState.TURN;            // 旋回

                aiState = EnemyAiState.MOVE;   // テスト用
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

        StartCoroutine("AiTimer");
    }

    /// <summary>
    /// 砲台の向きを参照し、攻撃判定を返す
    /// Playerに向いているとき：ture
    /// それ以外　　　　　　　：false
    /// </summary>
    private bool TurretDirection()
    {
        grandChild = transform.Find("LAV25/LAV25_Turret").gameObject;  // LAV25_Turret取得
        //grandChild = transform.GetChild(0).GetChild(1).gameObject;     // 順番から取得
        RaycastHit turretHit;   // レイに衝突したオブジェクト情報

        if (grandChild == null) return false;

        // 砲台の前方にRayを発射
        if (Physics.SphereCast(enemyPos, 0.5f, grandChild.transform.forward, out turretHit))
        {
            GameObject turretHitObj = turretHit.collider.gameObject;

            if (turretHitObj.tag == "Player")
            {
                Debug.Log("Playerに当たった");
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

    // 毎フレーム処理による負荷の軽減用タイマー
    IEnumerator AiTimer()
    {
        isTimer = true;
        yield return new WaitForSeconds(0.5f); // 0.5秒ごとに実行

        isTimer = false;
    }

    #region 行動遷移ごとのメソッド
    private void Attack()
    {
        GameObject shotObj = grandChild.transform.GetChild(0).gameObject;  // ShotPosition取得
        bool isInstans = BulletGenerateClass.BulletInstantiate(gameObject, shotObj, "Bullet", shotNum); // 弾生成
        if (isInstans) Debug.LogError("弾が生成されませんでした");
    }

    private void Turn()
    {
        // 旋回
        //Vector3 aim = playerPos - enemyPos;       // 対象への相対ベクトル取得
        //var look = Quaternion.LookRotation(aim);  // 対象の方向に向くメソッド
        //grandChild.transform.rotation = look;     // Playerの方向を向く
    }

    /// <summary>
    /// bulletに当たった時に呼ばれる死亡実行メソッド
    /// 
    /// </summary>
    public override void TankHit()
    {
        base.TankHit();

        aiState = EnemyAiState.DEATH; // 死亡遷移に移行、敵消滅
    }
    #endregion

    #region ベクトル変換
    /// <summary>
    /// EnemyからPlayerのいる角度を計算
    /// 求めた角度を8方向に分割し、Vector2(-1～1, -1～1)に変換
    /// </summary>
    private void VectorCalc()
    {
        // 内積を求め、角度に変換(内積＊角度)
        float VectorX = enemyPos.x - playerPos.x;
        float VectorZ = enemyPos.z - playerPos.z;
        float Radian = Mathf.Atan2(VectorZ, VectorX) * Mathf.Rad2Deg;

        //角度表示変更
        if (Radian < 0)
        {
            // -180～180で返るため、0～360に変換
            Radian += 360;
        }

        // 360度を8分割し、四捨五入する(0～8)
        int Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8の場合0に変換(9等分防止)
        if (Division == 8) Division = 0;

        // Vector2に変換して取得
        //Vector2 Direction = Conversion(Division);
        Vector3 Direction = Conversion(Division); // テスト

        //transform.position += Direction; // Player追従テスト
    }

    // ベクトル変換メソッド
    Vector3 Conversion(int index)
    {
        switch (index)
        {
            // 左回り
            //case 0: return new Vector2(-1,  0);  // 0度
            //case 1: return new Vector2(-1, -1);  // 45度
            //case 2: return new Vector2( 0, -1);  // 90度
            //case 3: return new Vector2( 1, -1);  // 135度
            //case 4: return new Vector2( 1,  0);  // 180度
            //case 5: return new Vector2( 1,  1);  // 225度
            //case 6: return new Vector2( 0,  1);  // 270度
            //case 7: return new Vector2(-1,  1);  // 315度
            //default: return Vector2.zero;

            // テスト
            case 0: return new Vector3(-1, 0,  0);   // 0度
            case 1: return new Vector3(-1, 0, -1);   // 45度
            case 2: return new Vector3( 0, 0, -1);   // 90度
            case 3: return new Vector3( 1, 0, -1);  // 135度
            case 4: return new Vector3( 1, 0,  0);  // 180度
            case 5: return new Vector3( 1, 0,  1); // 225度
            case 6: return new Vector3( 0, 0,  1);  // 270度
            case 7: return new Vector3(-1, 0,  1);  // 315度
            default: return Vector2.zero;
        }
    }
    #endregion
}
