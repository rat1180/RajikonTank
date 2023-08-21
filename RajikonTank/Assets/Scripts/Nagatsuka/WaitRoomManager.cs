using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ConstList;

/*
マッチング中に待機するロビー内の管理マネージャー
現在のロビー状態の表示と、ロビー情報のコントロールを行う
マッチング後はゲーム開始に合わせてシーンの移動を行う
 */

public class WaitRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Tooltip("情報を表示するメッセージ")] Text MessageText;
    [SerializeField, Tooltip("開始ボタン")] Button SceanMoveButton;
    [SerializeField, Tooltip("ルーム名を表示するテキスト")] Text RoomNameText;
    //マスターかどうか
    private bool isMaster;
    //ルーム内で接続できているか
    private bool isInRoom;
    //スタートしたかどうか
    private bool isStart;

    //ルームのカスタムプロパティを設定する為の宣言.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        isMaster = false;
        isInRoom = false;
        isStart = false;
        TryRoomJoin();
    }

    // Update is called once per frame
    void Update()
    {
        RoomNameText.text = "RoomName:" + ConectServer.RoomProperties.RoomName;
        if (isInRoom)
        {
            RoomStatusUpDate();
            if (Input.GetKeyDown(KeyCode.Space) && SceanMoveButton.IsInteractable())
            {
                MoveGameScean();
            }
        }
    }
    #endregion

    /// <summary>
    /// 現在のメンバーでゲームシーンに移動する
    /// </summary>
    public void MoveGameScean()
    {
        if (isStart) return;
        if (isMaster)
        {
            isStart = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(/*"ProttypeSeacn");*/SceanNames.GAME.ToString());
        }
    }

    /// <summary>
    /// このシーン移動後に改めてルーム接続を試行する
    /// </summary>
    void TryRoomJoin()
    {
        Debug.Log("RoomName:"+ConectServer.RoomProperties.RoomName);
        //オフライン以外の時に接続
        if(ConectServer.RoomProperties.RoomName != "Offline")
        {
            
            //タイトルで確立した情報で接続
            PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, new RoomOptions(), TypedLobby.Default);
        }
        else
        {
            StartCoroutine(WaitDisconect());
        }
        
    }

    /// <summary>
    /// 切断を待ってからオフラインモードを開始する
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitDisconect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.OfflineMode = true;
        //タイトルで確立した情報で接続
        PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, new RoomOptions(), TypedLobby.Default);
    }

    /// <summary>
    /// 接続時に呼ばれる関数
    /// マスターの場合、各種設定を行う
    /// </summary>
    public override void OnJoinedRoom()
    {
        isMaster = PhotonNetwork.IsMasterClient;
        isInRoom = true;
        if (isMaster)
        {
            PhotonNetwork.CurrentRoom.MaxPlayers = (byte)ConectServer.RoomProperties.MaxPlayer;
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("OnJoin");

        //カスタムプロパティの設定(GAMESTATUS).
        GAMESTATUS status = GAMESTATUS.NONE;

        customProperties["GameStatus"] = status;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        MessageText.text = message + " によって接続出来ません";
    }

    /// <summary>
    /// ルーム内の情報を更新し、表示する
    /// </summary>
    void RoomStatusUpDate()
    {
        //RoomNameText.text = ConectServer.RoomProperties.RoomName.ToString();
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        if (!isInRoom)
        {
            SceanMoveButton.interactable = false;
        }
        else
        {
            if (!isMaster)
            {
                SceanMoveButton.interactable = false;
                MessageText.text = "開始をまっています...\n下のキャラクターに触れると色を変更出来ます";
            }
            else
            {
                SceanMoveButton.interactable = true;
                MessageText.text = "スペースキーを押すとゲームが始まります\n下のキャラクターに触れると色を変更出来ます";
            }
            SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
            = "開始(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

            //メンバリストを表示
        }
    }
    
}
