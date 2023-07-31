using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

/*
接続を行うマネージャー
接続後のことは関与せず、接続のみを行う
 */

public class ConectServer : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class RoomPropertie
    {
        [Tooltip("参加するルーム名")] public string RoomName;
        [Tooltip("参加可能人数")] public int MaxPlayer;
    }

    [SerializeField, Tooltip("デバッグなどでのオフラインモード")] bool isOffline;

    [Tooltip("サーバー接続時に呼ばれるイベント")] public UnityEvent OnConect;
    [Tooltip("ルーム参加時に呼ばれるイベント")] public UnityEvent OnJoinde;
    [SerializeField, Tooltip("ルームに関する情報")] public static RoomPropertie RoomProperties = new RoomPropertie();
    // Start is called before the first frame update
    void Start()
    {
        if (isOffline)
        {
            Debug.Log(PhotonNetwork.IsConnected);
            PhotonNetwork.OfflineMode = true;
        }
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        else PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        OnConect.Invoke();
        Debug.Log("OnConect");
        //テスト
        //RoomProperties.MaxPlayer = 2;
        //RoomProperties.RoomName = "1";
        //SceneManager.LoadScene("WaitRoom");
        //テスト
        //TryRoomJoin();
    }

    public void TryRoomJoin()
    {
        if(isOffline) { PhotonNetwork.JoinOrCreateRoom("5", new RoomOptions(), TypedLobby.Default); } else
        {
            //ルームに参加する（ルームが存在しなければ作成して参加する）
            PhotonNetwork.JoinOrCreateRoom(RoomProperties.RoomName, new RoomOptions(), TypedLobby.Default);
        }
        
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {

        OnJoinde.Invoke();
        Debug.Log("Onjoin");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
