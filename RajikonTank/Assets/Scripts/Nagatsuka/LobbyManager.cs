using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
<<<<<<< HEAD
using ConstList;
=======
//using ConstList;
>>>>>>> main

public class LobbyManager : MonoBehaviourPunCallbacks
{
    const string RoomHead = "�}���`�v���C ";
    const string RoomMidle =  "\n�Q���l��\n";
    const string RoomOK = "�Q���\";
    const string RoomNotOK = "�Q���s��";

    [SerializeField, Tooltip("1���[���̎Q���l��")] int MaxPlayer;
    [SerializeField, Tooltip("�Q���{�^�����X�g")] CanvasGroup ButtonRoot;
    [SerializeField, Tooltip("�A�C�e�������p�l��")] GameObject ItemPanel;
    //���r�[�Q���ς݂�
    private bool isInLobby;
    //�A�C�e������������
    private bool isItemInf;

    private List<RoomInfo> RoomInfos;

    /// <summary>
    /// �R�l�N�g�T�[�o�[����Ă΂�A
    /// ���r�[�ւ̎Q�������s����
    /// </summary>
    public void TryRobyJoin()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ���r�[�ڑ����ɌĂ΂��
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        isInLobby = true;

        ButtonRoot.interactable = true;

        //�{�^����ǉ�
        List<GameObject> Buttons = new List<GameObject>();
        for(int i=0;i< ButtonRoot.gameObject.transform.childCount;  i++)
        {
            Buttons.Add(ButtonRoot.gameObject.transform.GetChild(i).gameObject);
        }
<<<<<<< HEAD
        GetComponent<SelectButton>().AddButton(Buttons);
=======
        //GetComponent<SelectButton>().AddButton(Buttons);
>>>>>>> main
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        foreach(var room in roomList)
        {
            if (int.Parse(room.Name) < 0 || int.Parse(room.Name) > ButtonRoot.transform.childCount) continue;

            //���[���ԍ��ɉ����������X�V
            var button = ButtonRoot.transform.GetChild(int.Parse(room.Name));
            button.transform.GetChild(0).GetComponent<Text>().text = RoomHead+room.Name+RoomMidle + room.PlayerCount + "/" + room.MaxPlayers 
                                                                     + "\n"+ (room.IsOpen ? RoomOK : RoomNotOK);
            if (room.IsOpen)
            {
                button.GetComponent<Button>().interactable = true;
            }
            else
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    /// <summary>
    /// �{�^������������Ăׂ�悤�ɂ���
    /// </summary>
    public void JoinRoom(int RoomNm)
    {
        ButtonRoot.interactable = false;
        ConectServer.RoomProperties.RoomName = RoomNm.ToString();
        ConectServer.RoomProperties.MaxPlayer = MaxPlayer;
        SceneManager.LoadScene("WaitRoom");
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
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
<<<<<<< HEAD
        SceneManager.LoadScene(SceanNames.STARTTITLE.ToString());
=======
        //SceneManager.LoadScene(SceanNames.STARTTITLE.ToString());
>>>>>>> main
    }

    public void ItemInfbuttonPush()
    {
        isItemInf = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        isInLobby = false;
        isItemInf = false;
        //�Q�������������r�[�Q���O�͉����Ȃ�����
        ButtonRoot.interactable = false;

        //BGMManager.Instance.SetBGM(BGMid.TITLE);

<<<<<<< HEAD
        PhotonNetwork.LocalPlayer.SetGameStatus((int)GAMESTATUS.NONE);
=======
        //PhotonNetwork.LocalPlayer.SetGameStatus((int)GAMESTATUS.NONE);
>>>>>>> main
    }

    // Update is called once per frame
    void Update()
    {
        if (isItemInf)
        {
<<<<<<< HEAD
            GetComponent<SelectButton>().enabled = false;
=======
            //GetComponent<SelectButton>().enabled = false;
>>>>>>> main
            ItemPanel.SetActive(true);
            if (Input.anyKeyDown)
            {
                isItemInf = false;
<<<<<<< HEAD
                GetComponent<SelectButton>().enabled = true;
=======
                //GetComponent<SelectButton>().enabled = true;
>>>>>>> main
                ItemPanel.SetActive(false);
            }
        }
    }
}
