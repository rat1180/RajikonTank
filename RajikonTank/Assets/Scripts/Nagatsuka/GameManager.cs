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
        player,
        player2,
        player3,
        player4,
        CPU
    }


    #region デバック確認用一覧
    [Header("デバッグ確認フラグ")]
    public bool DebugFlg;
    public Text teamNameList;
    [SerializeField] GameObject EndGamePanel;
    public bool chackflg;
    TeamID WinId;
    #endregion
    GameObject modeManager;

    #region 各チーム(陣営)のクラス(TeamInfo).
    /// <summary>
    /// 各チームの情報を入れるクラス
    /// このクラスで勝敗を判定したりアクティブメンバーの数を数える.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        bool isActive;//生存状態.
        public List<Tank> tankList;
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

        #region Tankのリストを操作する関数.
        void PushTank(Tank tank)
        {
            tankList.Add(tank);
        }

        /// <summary>
        /// プレイヤーのタンクを格納したリストで使用
        /// ID番号を引数に非アクティブにするタンクを指定
        /// </summary>
        public void NotActiveTank(int iD)
        {
            //タンクを非アクティブにする.
            //tankList[iD].
        }

        #endregion

        /// <summary>
        /// 死亡した場合呼び出す関数.
        /// </summary>
        public void Death()
        {
            isActive = false;
        }
        /// <summary>
        /// 単純なint型で数を管理、0になったら死亡関数を呼び出す.
        /// </summary>
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
        public bool ReturnActive()
        {
            return isActive;
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
        
        teamInfo.Add(new TeamInfo(TeamID.player));
        teamInfo.Add(new TeamInfo(TeamID.player2));
        teamInfo.Add(new TeamInfo(TeamID.CPU));
        NowGameState = GAMESTATUS.INGAME;
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
            case GAMESTATUS.ENDGAME:
                EndGameRoop();
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
        CheckActive();
    }

    /// <summary>
    /// EndGameの時に動かす関数.
    /// </summary>
    private void EndGameRoop()
    {
        EndGamePanel.SetActive(true);
        //勝利したチームのIDを表示.
        EndGamePanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "勝利したチーム：" + WinId;
    }

    /// <summary>
    /// Tankを格納しているリストのアクティブ状態を参照.
    /// 残っている陣営が1つのみならゲームを終了する
    /// </summary>
    void CheckActive()
    {
        int activeNum = 0;
        for(int i=0;i< teamInfo.Count; i++)
        {
            if (teamInfo[i].ReturnActive())
            {
                activeNum++;
                WinId = teamInfo[i].ReturnID();
            }
        }
        if (activeNum == 1)
        {
            NowGameState = GAMESTATUS.ENDGAME;
        }
        Debug.Log("アクティブ数" + activeNum);
    }

    /// <summary>
    /// DebugFlgがTrueの時にインスペクター上,Debug.Logで数値を確認できる
    /// </summary>
    private void CheckDebug()
    {
        teamNameList.text = teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActive() + "\n" +
                            teamInfo[2].ReturnID().ToString() + ":" + teamInfo[2].ReturnActive();

        //Debug.Log(teamInfo[0].ReturnID());
        //Debug.Log("チーム数" + teamInfo.Count);
    }


    #region デバック用関数
    public void TestDeathplayer()
    {
        teamInfo[0].Death();
    }
    public void TestDeathPlayer2()
    {
        teamInfo[1].Death();
    }
    public void TestDeathCPU()
    {
        teamInfo[2].Death();
    }
    #endregion
}