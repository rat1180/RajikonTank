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
    public static GameManager instance;

    [Header("ゲーム状態")]
    public GAMESTATUS NowGameState;//現在のゲーム状態.

    /// <summary>
    /// TeamInfoクラスに入れるID一覧.
    /// </summary>
    public enum TeamID { 
        player1,
        player2,
        player3,
        player4,
        CPU
    }


    #region デバック確認用一覧
    [Header("デバッグ確認フラグ")]
    public bool DebugFlg;
    public List<TeamInfo> TestteamInfo = new List<TeamInfo>();
    public bool chackflg;
    #endregion

    GameObject modeManager;

    #region 各チーム(陣営)のクラス(TeamInfo).
    /// <summary>
    /// 各チームの情報を入れるクラス
    /// このクラスで勝敗を判定したりアクティブメンバーの数を数える.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        public bool isActive;//生存状態.
        public int memberNum;//チームの生存人数をカウント.

        #region コンストラクタ・デストラクタ
        public TeamInfo()
        {
            isActive = true;
        }
        public TeamInfo(TeamID iD)
        {
            isActive = true;
            ID = iD;
        }
        ~TeamInfo(){}
        #endregion

        /// <summary>
        /// 死亡した場合呼び出す関数.
        /// </summary>
        public void Death()
        {
            isActive = false;
        }

        public void MemberDeath()
        {
            memberNum--;
            if (memberNum == 0)
            {
                Death();
            }
        }

        public void AddMember()
        {
            memberNum++;
        }
        #region ID変更・return関数
        public void ChangeID(TeamID iD)
        {
            ID = iD;
        }
        public TeamID ReturnID()
        {
            return ID;
        }
        #endregion
    }

    #endregion

    public List<TeamInfo> teamInfo = new List<TeamInfo>();

    #region Unityイベント(Awake・Start・Update)

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        TestteamInfo.Add(new TeamInfo(TeamID.player1));
        //TestteamInfo[0].Death();
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

        if (DebugFlg) CheckDebug();
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

    /// <summary>
    /// DebugFlgがTrueの時にインスペクター上,Debug.Logで数値を確認できる
    /// </summary>
    private void CheckDebug()
    {
        chackflg = TestteamInfo[0].isActive;
        Debug.Log(TestteamInfo[0].ReturnID());
    }
}
