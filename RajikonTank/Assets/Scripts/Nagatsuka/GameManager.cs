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

    #region 定数宣言
    //InGameCanvas使用定数.
    const int OPERATION_IMAGE = 0;
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    #endregion

    [Header("ゲーム状態")]
    public GAMESTATUS NowGameState;//現在のゲーム状態.

    [Header("InGame時のキャンバス関連")]
    [SerializeField] GameObject InGameCanvas;//InGame中に表示しているキャンバス(UI).
    public int RestBullets;                  //残弾数.

    [Header("使用画像")]
    [SerializeField] Sprite[] BulletsImage;  //残弾数表示画像.

    #region デバック確認用一覧
    [Header("デバッグ確認フラグ")]
    public bool DebugFlg;                  //ONOFFでデバックの表示を切り換える.
    [SerializeField] GameObject DebugPanel;//デバック情報をまとめたパネル.
    public Text teamNameList;              //チームのID・数・生存状態を表示用.
    TeamID WinId;                          //勝者のID.    
    #endregion

    [SerializeField] GameObject EndGamePanel;//ゲーム終了時に表示するパネル.

    public int player_IDnum;//Playerがリストの何番目なのかを確認.
    public int CPU_IDnum;   //CPUがリストの何番目なのかを確認.

    public GameObject testOBJ;

    #region 各チーム(陣営)のクラス(TeamInfo).
    /// <summary>
    /// 各チームの情報を入れるクラス
    /// このクラスで勝敗を判定したりアクティブメンバーの数を数える.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        bool isActive;//生存状態.
        public List<Rajikon> tankList;
        public int memberNum;//チームの生存人数をカウント.

        #region コンストラクタ・デストラクタ
        public TeamInfo()
        {
            isActive = true;
            tankList = new List<Rajikon>();
        }
        public TeamInfo(TeamID iD)
        {
            isActive = true;
            ID = iD;
            tankList = new List<Rajikon>();
            AddMember();
        }
        public TeamInfo(TeamID iD,Rajikon rajikon)
        {
            isActive = true;
            ID = iD;
            tankList = new List<Rajikon>();
            tankList.Add(rajikon);
            AddMember();
        }
        ~TeamInfo(){}
        #endregion

        #region Tankのリストを操作する関数.
        public void PushTank(Rajikon tank)
        {
            tankList.Add(tank);
            memberNum++;
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
            GameManager.instance.CheckActive();
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

        public int ReturnActiveMember()
        {
            return memberNum;
        }
        #endregion
    }

    #endregion

    public List<TeamInfo> teamInfo = new List<TeamInfo>();

    #region Unityイベント(Awake・Start・Update)

    private void Awake()
    {
        instance = this;
        CPU_IDnum = 0;
        player_IDnum = 0;
    }
    private void Start()
    {
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
        else DebugPanel.SetActive(false);
    }
    #endregion

    #region Gameのステータス毎に動かすRoop関数
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
        ChangeInGameCanvs();
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
    #endregion

    #region 外部から呼び出す関数(Listに追加する・死亡関数).
    /// <summary>
    /// Player・CPUを生成した際にIDを参照、一致したらそのチームリストにTankを入れる.
    /// </summary>
    public void PushTank(TeamID teamID,Rajikon tank)
    {
        int cnt = 0;
        if (teamInfo.Count == 0)//リストがない状態ならループせずに追加して戻る.
        {
            teamInfo.Add(new TeamInfo(teamID,tank));
            if (teamID == TeamID.CPU) CPU_IDnum = 0;
            if (teamID == TeamID.player) player_IDnum = 0;
            return;
        }
        else
        {
            for (int i = 0; i < teamInfo.Count; i++)//リスト内を全検索して重複チェックする.
            {
                if (teamInfo[i].ReturnID() == teamID)//追加するIDが同じ場合、メンバーを追加する
                {
                    //teamInfo[i].AddMember();
                    teamInfo[i].PushTank(tank);
                    Debug.Log("メンバー追加完了");
                    return;//メンバー追加した時点で関数を抜ける.
                }
                cnt++;
            }
            //ループを抜けた=重複はないので新たに追加する.
            teamInfo.Add(new TeamInfo(teamID,tank));
            if (teamID == TeamID.CPU) CPU_IDnum = cnt;
            if (teamID == TeamID.player) player_IDnum = cnt;
            Debug.Log("リスト追加完了");
            return;
        }

        //for(int i = 0; i < teamInfo.Count; i++)
        //{
        //    if (teamInfo[i].ReturnID() == teamID)//IDが一致したらTankをPushする.
        //    {
        //        teamInfo[i].PushTank(tank);
        //    }
        //}
    }

    /// <summary>
    /// TeamListにタンクを追加する
    /// 引数にTeamIDを指定
    /// 追加する際にIDの重複がないか確認.
    /// </summary>
    public void PushTeamList(TeamID teamID)
    {
        if (teamInfo.Count == 0)//リストがない状態ならループせずに追加して戻る.
        {
            teamInfo.Add(new TeamInfo(teamID));
            return;
        }
        else
        {
            for (int i = 0; i < teamInfo.Count; i++)//リスト内を全検索して重複チェックする.
            {
                if (teamInfo[i].ReturnID() == teamID)//追加するIDが同じ場合、メンバーを追加する
                {
                    teamInfo[i].AddMember();
                    Debug.Log("メンバー追加完了");
                    return;//メンバー追加した時点で関数を抜ける.
                }

            }
            //ループを抜けた=重複はないので新たに追加する.
            teamInfo.Add(new TeamInfo(teamID));
            Debug.Log("リスト追加完了");
            return;
        }
    }

    /// <summary>
    /// タンクが死亡した際に呼び出す関数.
    /// </summary>
    public void DeathTank(TeamID teamID)
    {
        for(int i = 0; i < teamInfo.Count; i++)//リスト内を全検索して重複チェックする.
            {
            if (teamInfo[i].ReturnID() == teamID)//IDが同じ場合、メンバーを減少(死亡)する.
            {
                teamInfo[i].MemberDeath();
                Debug.Log("メンバー死亡完了");
                return;//メンバー追加した時点で関数を抜ける.
            }

        }
    }

    #endregion

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
    /// InGameCanvasの値を変更する関数
    /// 敵の残機数・残弾数を変更する.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        InGameCanvas.transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ":" + teamInfo[CPU_IDnum].ReturnActiveMember();
        Debug.Log(teamInfo[CPU_IDnum].ReturnActiveMember());
        InGameCanvas.transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
    }


    #region リスト初期化・削除関数
    public void PushInitListButton()
    {
        teamInfo = new List<TeamInfo>();
    }
    #endregion

    #region デバック用関数

    /// <summary>
    /// DebugFlgがTrueの時にインスペクター上,Debug.Logで数値を確認できる
    /// </summary>
    private void CheckDebug()
    {
        DebugPanel.SetActive(true);
        teamNameList.text = teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActiveMember() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActiveMember() + ":" + teamInfo[1].ReturnActive() + "\n";
        //Debug.Log("CPUIDNUM" + CPU_IDnum);
    }

    public void TestDeathplayer()
    {
        teamInfo[0].MemberDeath();
    }
    public void TestDeathPlayer2()
    {
        teamInfo[1].MemberDeath();
    }
    public void TestDeathCPU()
    {
        teamInfo[1].MemberDeath();
    }
    #endregion
}