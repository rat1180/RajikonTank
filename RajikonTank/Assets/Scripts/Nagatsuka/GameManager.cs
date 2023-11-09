using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] SoundManager soundManager;

    #region 定数宣言
    //InGameCanvas使用定数.
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    const int STATE_STAGE_PANEL = 0;
    const int STAGE_NAME = 1;
    const int INITIAL_ENEMY_NUM = 2;

    //各ゲームモードのパネル.
    enum Panels {
        READYGAME,
        INGAME,
        WIN,
        ENDGAME,
        TUTORIAL,
        DEBUG,
        Count,
    }

    #endregion

    #region private変数
    private StageManager stageManager;
    private bool perfectClearFlg;                            //完全制覇確認フラグ.
    private int RestBullets;                                 //プレイヤーの残弾数.
    private int[] DestroyCPU = new int[(int)EnemyName.COUNT];//撃破したCPUの種類カウント用.
    #endregion

    [Header("ゲーム状態")]
    public GAMESTATUS NowGameState;//現在のゲーム状態.

    [Header("ゲーム中のキャンバス")]
    [SerializeField] GameObject GameCanvas;
    [SerializeField] List<GameObject> GamePanel;
        
    [Tooltip("自身のプレイヤーID(マルチをするときにここを変更すれば良い)")]
    public int OWN_playerID;

    [Tooltip("結果表示の際に敵の数を出す間隔を調整する用")]
    public float WaitEnemyImage;

    [Header("ステージ番号")]
    public int NowStage;

    public int player_IDnum;//Playerがリストの何番目なのかを確認.
    public int CPU_IDnum;   //CPUがリストの何番目なのかを確認.

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
            tankList[id].gameObject.transform.GetChild(GameManager.instance.OWN_playerID).gameObject.transform.position = pos;
        }
        public void SetRotation(int id, GameObject child)
        {
            //tankList[id].gameObject.transform.LookAt(child.transform.position);
            //tankList[id].gameObject.transform.GetChild(GameManager.instance.OWN_playerID).gameObject.transform.LookAt(child.transform.position);
        }
    }

    #endregion

    public List<TeamInfo> teamInfo = new List<TeamInfo>();//プレイヤー・CPUの状態を入れるリスト.

    #region Unityイベント(Awake・Start・Update)

    private void Awake()
    {
        instance = this;
        CPU_IDnum = 0;
        player_IDnum = 0;
    }

    private void Start()
    {
        for(int i = 0; i < (int)Panels.Count; i++)//GamePanelの数分ループしてリストに追加する.
        {
            GamePanel.Add(GameCanvas.transform.GetChild(i).gameObject);
        }

        stageManager = this.transform.GetChild(0).gameObject.GetComponent<StageManager>();
        stageManager.ActiveStage(NowStage);

        perfectClearFlg = false;
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
                //Debug.Log("エラー:予期せぬゲームモード");
                break;
        }

        ErrorFunction();

        if (DebugFlg) CheckDebug();
        else GamePanel[(int)Panels.DEBUG].SetActive(false);
    }
    #endregion

    /// <summary>
    /// なんらかのエラーが発生した際にescキーで強制終了できるようにする関数.
    /// </summary>
    void ErrorFunction()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR //UnityEditorで起動しているとき.

            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else //ビルド環境.
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }

    #region Gameのステータス毎に動かすRoop関数
    /// <summary>
    /// Readyの時に動かす関数.
    /// </summary>
    private void ReadyRoop()
    {
        DrawStartStagePanel();
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
        
    }

    void EndGameWinRoop()
    {
        GamePanel[(int)Panels.WIN].transform.GetChild(1).GetComponent<Text>().text =
        stageManager.Stage[NowStage].name + " Clear!!!";    
    }
    #endregion

    /// <summary>
    /// ゲームモードによって表示するパネル1つだけをアクティブにする関数
    /// アクティブにするパネルを引数に指定.
    /// </summary>
    private void ActiveGamePanel(int mode)
    {
        for(int i = 0; i < GamePanel.Count -2; i++)//デバッグパネルとチュートリアルパネルを除く(-2)
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
    public void ActiveTutorial()
    {
        GamePanel[(int)Panels.TUTORIAL].SetActive(true);
    }
    public void CloseTutorial()
    {
        GamePanel[(int)Panels.TUTORIAL].SetActive(false);
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
            }
        }
        if (activeNum == 1)
        {
            JudgeEndGame();
        }
    }

    /// <summary>
    /// ゲームモードをENDGAME_WINに変更し、パネルをアクティブにする関数.
    /// </summary>
    private void ChangeWinMode()
    {
        NowGameState = GAMESTATUS.ENDGAME_WIN;
        StopBGM();
        if (NowStage == 0) CloseTutorial();
        PlaySE(SE_ID.Clear);
        
        ActiveGamePanel((int)Panels.WIN);
    }

    /// <summary>
    /// InGameCanvasの値を変更する関数
    /// 敵の残機数・残弾数を変更する.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        ActiveGamePanel((int)Panels.INGAME);
        GamePanel[(int)Panels.INGAME].transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ": " + teamInfo[CPU_IDnum].ReturnActiveMember();
       RestBullets =teamInfo[player_IDnum].tankList[OWN_playerID].GetRestBullet();
        GamePanel[(int)Panels.INGAME].transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
    }

    private void DrawStartStagePanel()
    {
        ActiveGamePanel((int)Panels.READYGAME);
        GamePanel[(int)Panels.READYGAME].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(STAGE_NAME).GetComponent<Text>().text = 
            stageManager.Stage[NowStage].name;
        GamePanel[(int)Panels.READYGAME].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(INITIAL_ENEMY_NUM).GetComponent<Text>().text = "敵戦車数 × " + teamInfo[CPU_IDnum].ReturnActiveMember();
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

    public void PlayBGM(BGM_ID id)
    {
        soundManager.PlayBGM(id);
    }
    public void StopBGM()
    {
        soundManager.StopBGM();
    }
    #endregion

    /// <summary>
    /// GameModeをReadyに変更する際に呼び出す関数.
    /// </summary>
    public void ChangeReadyMode()
    {
        if (stageManager.Stage.Count  == NowStage + 1)//ステージが最終ステージだった場合、ゲームを終了する.
        {
            perfectClearFlg = true;
            ChangeGameEnd();
        }
        else
        {
            NowStage++;
            NowGameState = GAMESTATUS.READY;
            stageManager.ActiveStage(NowStage);
            DrawStartStagePanel();
            for (int i = 0; i < teamInfo.Count; i++)//全チームの種類分ループする.
            {
                teamInfo[i].Active();
            }
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

    /// <summary>
    /// Playerが死亡、もしくは完全制覇したときに呼び出される関数
    /// BGM再生・モード変更等を行う.
    /// </summary>
    private void ChangeGameEnd()
    {
        NowGameState = GAMESTATUS.ENDGAME;
        PlayBGM(BGM_ID.Result);
        ActiveGamePanel((int)Panels.ENDGAME);
        if (NowStage == 0) CloseTutorial();
        if (perfectClearFlg)
        {
            GamePanel[(int)Panels.ENDGAME].transform.GetChild(1).gameObject.transform.GetChild(0).
            gameObject.GetComponent<Text>().text = "完全制覇！";
        }
        int cnt = 0;//死亡敵の種類をカウントする用.
        for(int i=0;i< DestroyCPU.Length; i++)
        {
            if (DestroyCPU[i] != 0)//種類ごとに1体以上倒していたらカウントを増やす.
            {
                cnt++;
            }
        }
        StartCoroutine(ActiveEnemysImage(cnt));
    }

    /// <summary>
    /// CPUの種類1つ1つを表示していくためのコルーチン
    /// cntを引数にcntの数分ループする
    /// </summary>
    /// <param name="cnt"></param>
    /// <returns></returns>
    IEnumerator ActiveEnemysImage(int cnt)
    {
        int sum = 0;            //敵の総撃破数カウント用.
        GameObject EnemysImage; //敵のタンク画像表示用.
        EnemysImage = GameCanvas.transform.GetChild((int)Panels.ENDGAME).gameObject.transform.GetChild(3).gameObject;
        for (int i = 0; i < cnt; i++)
        {
            EnemysImage.transform.GetChild(i).gameObject.SetActive(true);
            EnemysImage.transform.GetChild(i).gameObject.transform.GetChild(0).
                gameObject.GetComponent<Text>().text = DestroyCPU[i].ToString();
            yield return new WaitForSeconds(WaitEnemyImage);//インスペクター上で指定した時間待ってから次を表示.
            sum += DestroyCPU[i];//総撃破数を加算.
        }
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(4).gameObject.SetActive(true);
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(2).gameObject.SetActive(true);//タイトルバックボタン.
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(4).gameObject.GetComponent<Text>().text = "総撃破数:" + sum;
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
        GamePanel[(int)Panels.DEBUG].SetActive(true);
        GamePanel[(int)Panels.DEBUG].transform.GetChild(0).gameObject.GetComponent<Text>().text = 
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
        stageManager.ActiveStage(NowStage);
        DrawStartStagePanel();
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