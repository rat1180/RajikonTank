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
    [SerializeField] SoundManager soundManager;
    #region 定数宣言
    //InGameCanvas使用定数.
    const int OPERATION_IMAGE = 0;
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    const int STATE_STAGE_PANEL = 0;
    const int STAGE_NAME = 1;
    const int INITIAL_ENEMY_NUM = 2;

    const int READYGAMEPANEL = 0; 
    const int INGAMEPANEL = 1;
    const int WINPANEL = 2;
    const int ENDGAMEPANEL = 3;
    const int DEBUGPANEL = 4;//デバック情報をまとめたパネル.
    #endregion

    [Header("ゲーム状態")]
    public GAMESTATUS NowGameState;//現在のゲーム状態.

    [Header("ゲーム中のキャンバス")]
    [SerializeField] GameObject GameCanvas;
    [SerializeField] List<GameObject> GamePanel;

    

    public int RestBullets;                  //残弾数.

    [Header("ステージ番号")]
    public int NowStage;

    public int player_IDnum;//Playerがリストの何番目なのかを確認.
    public int CPU_IDnum;   //CPUがリストの何番目なのかを確認.
    public int[] DestroyCPU = new int[(int)EnemyName.COUNT];//撃破したCPUをカウント.
    GameObject EnemysImage;                               //表示用.
    TeamID WinId;                          //勝利したチームのID.

    [Header("使用画像")]
    [SerializeField] Sprite[] BulletsImage;  //残弾数表示画像.

    #region デバック確認用一覧
    [Header("デバッグパネル確認フラグ")]
    public bool DebugFlg;                  //ONOFFでデバックの表示を切り換える.
    #endregion

    

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
        }

        public void Active()
        {
            isActive = true;
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

        public void SetPosition(int id,Vector3 pos)
        {
            tankList[id].gameObject.transform.position = pos;
            Debug.Log("SetPosition起動");
        }
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
        GamePanel.Add(GameCanvas.transform.GetChild(READYGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(INGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(WINPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(ENDGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(DEBUGPANEL).gameObject);
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().ActiveStage(NowStage);

        EnemysImage = GameCanvas.transform.GetChild(ENDGAMEPANEL).gameObject.transform.GetChild(3).gameObject;
    }

    private void Update()
    {
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
            case GAMESTATUS.ENDGAME_WIN:
                EndGameWinRoop();
                break;
            default:
                Debug.Log("エラー:予期せぬゲームモード");
                break;
        }

        if (DebugFlg) CheckDebug();
        else GamePanel[DEBUGPANEL].SetActive(false);
    }
    #endregion

    #region Gameのステータス毎に動かすRoop関数
    /// <summary>
    /// Readyの時に動かす関数.
    /// </summary>
    private void ReadyRoop()
    {
        ActiveGamePanel(READYGAMEPANEL);
        DrawStateStagePanel();
        //Debug.Log("CPU数:" + teamInfo[CPU_IDnum].ReturnActiveMember());
    }

    /// <summary>
    /// InGameの時に動かす関数.
    /// </summary>
    private void InGameRoop()
    {
        ChangeInGameCanvs();
    }

    /// <summary>
    /// EndGameの時に動かす関数.
    /// </summary>
    private void EndGameRoop()
    {
        
       // Debug.Log("ACTOVEEND");
        //勝利したチームのIDを表示.
        //GamePanel[ENDGAMEPANEL].transform.GetChild(0).gameObject.GetComponent<Text>().text = "勝利したチーム：" + WinId;
    }

    void EndGameWinRoop()
    {
        GamePanel[WINPANEL].transform.GetChild(1).GetComponent<Text>().text = 
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().Stages[NowStage].name+" Clear!!!";       
    }
    #endregion

    /// <summary>
    /// ゲームモードによって表示するパネル1つだけをアクティブにする関数
    /// アクティブにするパネルを引数に指定.
    /// </summary>
    private void ActiveGamePanel(int mode)
    {
        for(int i = 0; i < GamePanel.Count -1; i++)
        {
            if(i == mode)
            {
                GamePanel[i].SetActive(true);
            }
            else
            {
                GamePanel[i].SetActive(false);
            }
        }
    }

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
            if (teamInfo[i].ReturnID() == teamID)//IDが同じ場合、メンバーを減少(死亡)させる.
            {
                teamInfo[i].MemberDeath();
                CheckActive();
                Debug.Log("メンバー死亡完了");
                return;//メンバー追加した時点で関数を抜ける.
            }
        }
    }
    /// <summary>
    /// タンクが死亡した際に呼び出す関数
    /// CPUのIDも一緒に引数に指定、撃破したCPUの種類をカウントする.
    /// </summary>
    public void DeathTank(TeamID teamID,EnemyName name)
    {
        for (int i = 0; i < teamInfo.Count; i++)//リスト内を全検索して重複チェックする.
        {
            if (teamInfo[i].ReturnID() == teamID)//IDが同じ場合、メンバーを減少(死亡)させる.
            {
                teamInfo[i].MemberDeath();
                for(int j = 0; j < DestroyCPU.Length; j++)//CPUの種類を判断する.
                {
                    if (j == (int)name)
                    {
                        DestroyCPU[j]++;
                    }
                }
                CheckActive();
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
            JudgeEndGame();
        }
    }

    private void ChangeWinMode()
    {
        NowGameState = GAMESTATUS.ENDGAME_WIN;
        ActiveGamePanel(WINPANEL);
    }

    /// <summary>
    /// InGameCanvasの値を変更する関数
    /// 敵の残機数・残弾数を変更する.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        ActiveGamePanel(INGAMEPANEL);
        GamePanel[INGAMEPANEL].transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ":" + teamInfo[CPU_IDnum].ReturnActiveMember();
        GamePanel[INGAMEPANEL].transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
    }

    private void DrawStateStagePanel()
    {
        
        GamePanel[READYGAMEPANEL].SetActive(true);
        GamePanel[READYGAMEPANEL].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(STAGE_NAME).GetComponent<Text>().text = 
            this.transform.GetChild(0).gameObject.GetComponent<StageManager>().Stages[NowStage].name;
        GamePanel[READYGAMEPANEL].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(INITIAL_ENEMY_NUM).GetComponent<Text>().text = "敵戦車数:" + teamInfo[CPU_IDnum].ReturnActiveMember() + "台";
    }

    #region SoundManager関数
    public void PlaySE(SE_ID id)
    {
        soundManager.PlaySE(id);
    }
    public void PlaySE(AudioClip audioClip)
    {
        soundManager.PlaySE(audioClip);
    }
    public AudioClip ReturnSE(SE_ID id)
    {
        AudioClip audioClip;
        audioClip = soundManager.ReturnSE(id);
        return audioClip;
    }
    #endregion

    public void ChangeReadyMode()
    {
        NowStage++;
        NowGameState = GAMESTATUS.READY;
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().ActiveStage(NowStage);
        DrawStateStagePanel();
        for (int i = 0; i < teamInfo.Count; i++)//リスト内を全検索して重複チェックする.
        {
            teamInfo[i].Active();
        }
    }

    /// <summary>
    /// GameEnd(終了)かGameWinか判定.
    /// </summary>
    private void JudgeEndGame()
    {
        if (teamInfo[player_IDnum].ReturnActive() == false)
        {
            ChangeGameEnd();
            return;
        }
        ChangeInGameCanvs();//CPUが0になったら反映.
        ChangeWinMode();
    }

    private void ChangeGameEnd()
    {
        NowGameState = GAMESTATUS.ENDGAME;
        ActiveGamePanel(ENDGAMEPANEL);
        int cnt = 0;//死亡敵の種類をカウントする用.
        for(int i=0;i< DestroyCPU.Length; i++)
        {
            if (DestroyCPU[i] == 0)
            {
                cnt++;
            }
        }
        StartCoroutine(ActiveEnemysImage(cnt));
    }

    IEnumerator ActiveEnemysImage(int cnt)
    {
        Debug.Log("CNT" + cnt);
        int sum = 0;//敵の総撃破数カウント用.
        for (int i = 0; i < cnt; i++)
        {
            EnemysImage.transform.GetChild(i).gameObject.SetActive(true);
            EnemysImage.transform.GetChild(i).gameObject.transform.GetChild(0).
                gameObject.GetComponent<Text>().text = DestroyCPU[i].ToString();
            yield return new WaitForSeconds(1f);
            sum += DestroyCPU[i];
        }
        GamePanel[ENDGAMEPANEL].transform.GetChild(4).gameObject.SetActive(true);
        GamePanel[ENDGAMEPANEL].transform.GetChild(4).gameObject.GetComponent<Text>().text = "総撃破数:" + sum;
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
        GamePanel[DEBUGPANEL].SetActive(true);
        GamePanel[DEBUGPANEL].transform.GetChild(0).gameObject.GetComponent<Text>().text = 
                            teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActiveMember() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActiveMember() + ":" + teamInfo[1].ReturnActive() + "\n";
    }

    /// <summary>
    /// ゲーム終了時にシーンを初期化する(Reload)関数
    /// </summary>
    public void ResetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        NowStage++;
        NowGameState = GAMESTATUS.READY;
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().ActiveStage(NowStage);
        DrawStateStagePanel();
    }

    public void TestDeathplayer()
    {
        //teamInfo[0].MemberDeath();
        PlaySE(SE_ID.PlayerDeath);
    }
    public void TestDeathCPU()
    {
        //teamInfo[1].MemberDeath();
        //PlaySE(SE_ID.Move);
        soundManager.PlaySE(soundManager.ReturnSE(SE_ID.Move));
    }
    #endregion
}