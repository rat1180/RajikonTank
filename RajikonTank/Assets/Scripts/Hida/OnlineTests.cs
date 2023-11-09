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
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PlayerID = PhotonNetwork.LocalPlayer.ActorNumber-1;

        Debug.Log("����" + SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.forward);
        Quaternion quaternion;
        quaternion = SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.rotation;
        CreateTank(SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.position,
                   SpawnPoints.transform.GetChild(PlayerID).gameObject.transform.GetChild(0).gameObject);    //�^���N�����֐�.
    }

    void CreateTank(Vector3 position, GameObject child)
    {
        GameObject tank;
        tank = Instantiate(FolderObjectFinder.GetResorceGameObject("Player"), position, Quaternion.identity);
        //tank.transform.LookAt(child.transform.position);

        tank.GetComponent<PlayerClass>().InitPlayer(PlayerClass.InitMode.DEBUG);
    }
}