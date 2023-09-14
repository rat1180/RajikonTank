using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyName // 行動パターン
{
    NORMAL,              // 通常敵
    REFLECT              // 反射敵
}

public enum EnemyAiState // 行動パターン
{
    WAIT,
    MOVE,
    TURN,
    ATTACK,
    AVOID
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

    #region AI遷移ループ(通常)
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
                    break;
                case EnemyAiState.MOVE:
                    break;
                case EnemyAiState.TURN:
                    break;
                case EnemyAiState.ATTACK:
                    Debug.Log("敵射撃");
                    break;
                case EnemyAiState.AVOID:
                    break;
            }

            StartCoroutine("AiTimer");
        }
    }

    /// <summary>
    /// Rayに触れたオブジェクトによるStateの割り当て
    /// Playerの場合：攻撃
    /// それ以外の場合：移動or旋回
    /// </summary>
    private void AiMainRoutine()
    {
        RaycastHit hit;

        // Rayを飛ばす処理(発射位置, 方向, 衝突したオブジェクト情報, 長さ(記載なし：無限))
        if (Physics.Raycast(enemyPos, playerPos, out hit))
        {
            GameObject hitObj = hit.collider.gameObject; // RaycastHit型からGameObject型へ変換

            if (hitObj.tag == "Player")
            {
                aiState = EnemyAiState.ATTACK; // 攻撃
            }
            else
            {
                aiState = EnemyAiState.MOVE;   // 旋回or移動？
            }
            Debug.Log(hitObj.name + "に当たった");
        }
        else
        {
            Debug.LogError("Rayが当たりませんでした。");
            return;
        }
    }

    // 毎フレーム処理による負荷の軽減用タイマー
    IEnumerator AiTimer()
    {
        timerFlg = true;
        yield return new WaitForSeconds(1);

        timerFlg = false;
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
