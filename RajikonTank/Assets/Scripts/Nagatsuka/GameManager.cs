using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{

    [Header("ゲーム状態")]
    public GAMESTATUS NowGameState;//現在のゲーム状態.

    #region Unityイベント(Awake・Start・Update)

    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        //NowGameState = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        switch (NowGameState)//ゲームモードによって処理を分岐する.
        {
            case GAMESTATUS.READY:
                ReadyRoop();
                break;
            case GAMESTATUS.INGAME:
                InGameRoop();
                break;
            default:
                Debug.Log("エラー:予期せぬゲームモード");
                break;
        }
    }
    #endregion

    /// <summary>
    /// Readyの時に動かす関数.
    /// </summary>
    private void ReadyRoop()
    {

    }

    /// <summary>
    /// InGameの時に動かす関数.
    /// </summary>
    private void InGameRoop()
    {

    }
}
