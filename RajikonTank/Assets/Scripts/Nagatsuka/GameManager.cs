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

    [Header("�Q�[�����")]
    public GAMESTATUS NowGameState;//���݂̃Q�[�����.

    /// <summary>
    /// TeamInfo�N���X�ɓ����ID�ꗗ.
    /// </summary>
    public enum TeamID { 
        player1,
        player2,
        player3,
        player4,
        CPU
    }


    #region �f�o�b�N�m�F�p�ꗗ
    [Header("�f�o�b�O�m�F�t���O")]
    public bool DebugFlg;
    public List<TeamInfo> TestteamInfo = new List<TeamInfo>();
    public bool chackflg;
    #endregion

    GameObject modeManager;

    #region �e�`�[��(�w�c)�̃N���X(TeamInfo).
    /// <summary>
    /// �e�`�[���̏�������N���X
    /// ���̃N���X�ŏ��s�𔻒肵����A�N�e�B�u�����o�[�̐��𐔂���.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        public bool isActive;//�������.
        public int memberNum;//�`�[���̐����l�����J�E���g.

        #region �R���X�g���N�^�E�f�X�g���N�^
        public TeamInfo()
        {
            isActive = true;
        }
        public TeamInfo(TeamID iD)
        {
            isActive = true;
            ID = iD;
        }
        ~TeamInfo(){}
        #endregion

        /// <summary>
        /// ���S�����ꍇ�Ăяo���֐�.
        /// </summary>
        public void Death()
        {
            isActive = false;
        }

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
        #region ID�ύX�Ereturn�֐�
        public void ChangeID(TeamID iD)
        {
            ID = iD;
        }
        public TeamID ReturnID()
        {
            return ID;
        }
        #endregion
    }

    #endregion

    public List<TeamInfo> teamInfo = new List<TeamInfo>();

    #region Unity�C�x���g(Awake�EStart�EUpdate)

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        TestteamInfo.Add(new TeamInfo(TeamID.player1));
        //TestteamInfo[0].Death();
    }

    private void Update()
    {
        //NowGameState = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        switch (NowGameState)//�Q�[�����[�h�ɂ���ď����𕪊򂷂�.
        {
            case GAMESTATUS.READY:
                ReadyRoop();
                break;
            case GAMESTATUS.INGAME:
                InGameRoop();
                break;
            default:
                Debug.Log("�G���[:�\�����ʃQ�[�����[�h");
                break;
        }

        if (DebugFlg) CheckDebug();
    }
    #endregion

    /// <summary>
    /// Ready�̎��ɓ������֐�.
    /// </summary>
    private void ReadyRoop()
    {

    }

    /// <summary>
    /// InGame�̎��ɓ������֐�.
    /// </summary>
    private void InGameRoop()
    {

    }

    /// <summary>
    /// DebugFlg��True�̎��ɃC���X�y�N�^�[��,Debug.Log�Ő��l���m�F�ł���
    /// </summary>
    private void CheckDebug()
    {
        chackflg = TestteamInfo[0].isActive;
        Debug.Log(TestteamInfo[0].ReturnID());
    }
}
