using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ConstList;

public class OnlineTests : MonoBehaviourPunCallbacks
{
    public GameObject SpawnPoints;
    public int PlayerID;
    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PlayerID = PhotonNetwork.LocalPlayer.ActorNumber-1;

        Debug.Log("向き" + SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.forward);
        Quaternion quaternion;
        quaternion = SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.rotation;
        CreateTank(SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.position,
                   SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.GetChild(0).gameObject);    //タンク生成関数.
    }

    void CreateTank(Vector3 position, GameObject child)
    {
        GameObject tank;
        tank = Instantiate(FolderObjectFinder.GetResorceGameObject("Player"), position, Quaternion.identity);
        //tank.transform.LookAt(child.transform.position);

        tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.DEBUG);
    }
}