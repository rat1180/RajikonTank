using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ConstList;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    const string RoomHead = "マルチプレイ ";
    const string RoomMidle =  "\n参加人数\n";
    const string RoomOK = "参加可能";
    const string RoomNotOK = "参加不可";

    [SerializeField, Tooltip("1ルームの参加人数")] int MaxPlayer;
    [SerializeField, Tooltip("参加ボタンリスト")] CanvasGroup ButtonRoot;
    [SerializeField, Tooltip("モード選択パネル")] GameObject ModeSelectPanel;
    //ロビー参加済みか
    private bool isInLobby;

    private List<RoomInfo> RoomInfos;

    /// <summary>
    /// コネクトサーバーから呼ばれ、
    /// ロビーへの参加を試行する
    /// </summary>
    public void TryRobyJoin()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ロビー接続時に呼ばれる
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        isInLobby = true;

        ButtonRoot.interactable = true;

        //ボタンを追加
        List<GameObject> Buttons = new List<GameObject>();
        for(int i=0;i< ButtonRoot.gameObject.transform.childCount;  i++)
        {
            Buttons.Add(ButtonRoot.gameObject.transform.GetChild(i).gameObject);//ここでボタンの要素を追加している.
        }

        GetComponent<SelectButton>().AddButton(Buttons);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        foreach(var room in roomList)
        {
            if (int.Parse(room.Name) < 0 || int.Parse(room.Name) > ButtonRoot.transform.childCount) continue;

            //ルーム番号に応じた情報を更新
            var button = ButtonRoot.transform.GetChild(int.Parse(room.Name));
            button.transform.GetChild(0).GetComponent<Text>().text = RoomHead+room.Name+RoomMidle + room.PlayerCount + "/" + room.MaxPlayers 
                                                                     + "\n"+ (room.IsOpen ? RoomOK : RoomNotOK);

            //入室できるルームのみボタンを押せるようにする.
            if (room.IsOpen)
            {
                button.GetComponent<Button>().interactable = true;//ボタン押下可能状態に.
            }
            else
            {
                button.GetComponent<Button>().interactable = false;//ボタン押下不可状態に.
            }
        }
    }

    /// <summary>
    /// ルームのボタンを押したら呼べるようにする
    /// </summary>
    public void JoinRoom(int RoomNm)
    {
        ButtonRoot.interactable = false;                          //他のルームのボタンを押下不可にする.
        ConectServer.RoomProperties.RoomName = RoomNm.ToString(); //入室するルームの名前を設定.
        ConectServer.RoomProperties.MaxPlayer = MaxPlayer;        //ルームに参加できる人数の設定.
        SceneManager.LoadScene("WaitRoom");                       //ゲーム待機シーンに移動.
    }

    public void SoloMode()
    {
        ButtonRoot.interactable = false;
        ConectServer.RoomProperties.RoomName = "Offline";
        SceneManager.LoadScene("WaitRoom");
    }

    public void TitleBack()
    {
        StartCoroutine(WaitDisConect()); 
    }

    IEnumerator WaitDisConect()
    {
        PhotonNetwork.Disconnect();      //Photonのサーバーから切断.
        while (PhotonNetwork.IsConnected)//接続していたら切断するまでループする.
        {
            yield return null;
        }
        SceneManager.LoadScene(SceanNames.STARTTITLE.ToString());

    }

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        isInLobby = false;
        //参加処理中かロビー参加前は押せなくする
        ButtonRoot.interactable = false;

        //BGMManager.Instance.SetBGM(BGMid.TITLE);

        PhotonNetwork.LocalPlayer.SetGameStatus((int)GAMESTATUS.NONE);

    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    /// <summary>
    /// 複数人でプレイするボタンを押した際にパネルを表示する関数.
    /// </summary>
    public void PushMultiButton()
    {
        ModeSelectPanel.SetActive(false);
    }

    /// <summary>
    /// 人数選択画面に戻るボタンを押したときに呼び出す関数.
    /// </summary>
    public void PushModeBackButton()
    {
        ModeSelectPanel.SetActive(true);
    }
}
