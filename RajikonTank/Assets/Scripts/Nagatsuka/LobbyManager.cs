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
    const string RoomHead = "�}���`�v���C ";
    const string RoomMidle =  "\n�Q���l��\n";
    const string RoomOK = "�Q���\";
    const string RoomNotOK = "�Q���s��";

    [SerializeField, Tooltip("1���[���̎Q���l��")] int MaxPlayer;
    [SerializeField, Tooltip("�Q���{�^�����X�g")] CanvasGroup ButtonRoot;
    [SerializeField, Tooltip("���[�h�I���p�l��")] GameObject ModeSelectPanel;
    //���r�[�Q���ς݂�
    private bool isInLobby;

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
            Buttons.Add(ButtonRoot.gameObject.transform.GetChild(i).gameObject);//�����Ń{�^���̗v�f��ǉ����Ă���.
        }

        GetComponent<SelectButton>().AddButton(Buttons);
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

            //�����ł��郋�[���̂݃{�^����������悤�ɂ���.
            if (room.IsOpen)
            {
                button.GetComponent<Button>().interactable = true;//�{�^�������\��Ԃ�.
            }
            else
            {
                button.GetComponent<Button>().interactable = false;//�{�^�������s��Ԃ�.
            }
        }
    }

    /// <summary>
    /// ���[���̃{�^������������Ăׂ�悤�ɂ���
    /// </summary>
    public void JoinRoom(int RoomNm)
    {
        ButtonRoot.interactable = false;                          //���̃��[���̃{�^���������s�ɂ���.
        ConectServer.RoomProperties.RoomName = RoomNm.ToString(); //�������郋�[���̖��O��ݒ�.
        ConectServer.RoomProperties.MaxPlayer = MaxPlayer;        //���[���ɎQ���ł���l���̐ݒ�.
        SceneManager.LoadScene("WaitRoom");                       //�Q�[���ҋ@�V�[���Ɉړ�.
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
        PhotonNetwork.Disconnect();      //Photon�̃T�[�o�[����ؒf.
        while (PhotonNetwork.IsConnected)//�ڑ����Ă�����ؒf����܂Ń��[�v����.
        {
            yield return null;
        }
        SceneManager.LoadScene(SceanNames.STARTTITLE.ToString());

    }

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        isInLobby = false;
        //�Q�������������r�[�Q���O�͉����Ȃ�����
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
    /// �����l�Ńv���C����{�^�����������ۂɃp�l����\������֐�.
    /// </summary>
    public void PushMultiButton()
    {
        ModeSelectPanel.SetActive(false);
    }

    /// <summary>
    /// �l���I����ʂɖ߂�{�^�����������Ƃ��ɌĂяo���֐�.
    /// </summary>
    public void PushModeBackButton()
    {
        ModeSelectPanel.SetActive(true);
    }
}
