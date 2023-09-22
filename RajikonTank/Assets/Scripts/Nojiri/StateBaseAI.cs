using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class StateBaseAI : MonoBehaviour
{
    public EnemyName aiName = EnemyName.NORMAL;      //敵属性の設定
    public EnemyAiState aiState = EnemyAiState.WAIT; //敵の攻撃遷移

    [SerializeField] public Transform player;// プレイヤーの位置取得
    private Vector3 enemyPos;   // 敵(自分)の位置取得
    private Vector3 playerPos;  // プレイヤーの位置取得

    private bool initFlg = true;
    private bool timerFlg = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
    }

    #region 初期ループ
    /// <summary>
    /// 初期化処理
    /// </summary>
    void InitAI()
    {
        enemyPos = transform.position;
        playerPos = player.position;

        if (initFlg == false)
        {
            return;
        }

        Debug.Log("初期化実行");
        initFlg = false;
    }
    #endregion

    #region AI遷移ループ
    /// <summary>
    /// 敵の種類によって行動遷移を変更
    /// </summary>
    void UpdateAI()
    {
        InitAI();

        switch (aiName)
        {
            case EnemyName.NORMAL:  // 通常敵
                Normal();
                break;
            case EnemyName.REFLECT: // 反射敵
                //Move2();
                break;
            default:
                break;
        }
    }
    #endregion

    #region 通常敵遷移
    /// <summary>
    /// AI状態遷移
    /// </summary>
    private void Normal()
    {
        if(timerFlg == false)
        {
            AiMainRoutine();

            switch (aiState)
            {
                case EnemyAiState.WAIT:
                    Debug.Log("待機");
                    break;
                case EnemyAiState.MOVE:
                    Debug.Log("移動");
                    break;
                case EnemyAiState.TURN:
                    Debug.Log("旋回");
                    break;
                case EnemyAiState.ATTACK:
                    Debug.Log("射撃");
                    break;
                case EnemyAiState.AVOID:
                    break;
                case EnemyAiState.DEATH:
                    break;
                default:
                    break;
            }

            StartCoroutine("AiTimer");
        }
    }

    /// <summary>
    /// Rayに触れたオブジェクトによるStateの割り当て
    /// Playerの場合　：攻撃
    /// それ以外の場合：移動or旋回
    /// </summary>
    private void AiMainRoutine()
    {
        RaycastHit hit;
        bool attackFlg;

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        if (Physics.Raycast(enemyPos, playerPos, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == "Player")
            {
                attackFlg = TurretDirection();
                if(attackFlg) aiState = EnemyAiState.ATTACK; // 攻撃
                else aiState = EnemyAiState.TURN;
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
    }

    // 毎フレーム処理による負荷の軽減用タイマー
    IEnumerator AiTimer()
    {
        timerFlg = true;
        yield return new WaitForSeconds(0.1f); // 0.2秒ごとに実行

        timerFlg = false;
    }
    #endregion

    #region 攻撃判定
    /// <summary>
    /// 砲台の向きを参照し、攻撃判定を返す
    /// Playerに向いているとき：ture
    /// それ以外　　　　　　　：false
    /// </summary>
    private bool TurretDirection()
    {
        GameObject grandChild = transform.GetChild(0).GetChild(1).gameObject;  // 孫オブジェクトを取得
        RaycastHit turretHit;   // レイに衝突したオブジェクト情報

        Debug.Log(grandChild);

        // 旋回
        //Vector3 aim = playerPos - enemyPos;       // 対象への相対ベクトル取得
        //var look = Quaternion.LookRotation(aim);  // 対象の方向に向くメソッド
        //child.transform.rotation = look;  // Playerの方向を向く

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
        Vector2 Direction = Conversion(Division);

        // Vector2変換後の結果表示
        Debug.Log("角度： " + Direction);
    }

    // ベクトル変換メソッド
    Vector2 Conversion(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(0, 1);   // 0度
            case 1: return new Vector2(1, 1);   // 45度
            case 2: return new Vector2(1, 0);   // 90度
            case 3: return new Vector2(1, -1);  // 135度
            case 4: return new Vector2(0, -1);  // 180度
            case 5: return new Vector2(-1, -1); // 225度
            case 6: return new Vector2(-1, 0);  // 270度
            case 7: return new Vector2(-1, 1);  // 315度
            default: return Vector2.zero;
        }
    }
    #endregion
}
