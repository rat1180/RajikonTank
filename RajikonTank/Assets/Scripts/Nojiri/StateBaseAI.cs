using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAiState // 行動パターン
{
    // (仮)
    MOVE1,
    MOVE2,
    MOVE3,
}

public class StateBaseAI : MonoBehaviour
{
    public EnemyAiState aiState = EnemyAiState.MOVE1;
    bool flg = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAI();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void InitAI()
    {
        if (flg == false)
        {
            return;
        }

        Debug.Log("初期化実行");
        flg = false;
    }

    void UpdateAI()
    {
        InitAI();

        switch (aiState)
        {
            case EnemyAiState.MOVE1:
                //Move1();
                break;
            case EnemyAiState.MOVE2:
                //Move2();
                break;
            case EnemyAiState.MOVE3:
                //Move3();
                break;
            default:
                break;
        }
    }
}
