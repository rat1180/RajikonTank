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

    #region �f�o�b�N�m�F�p�ꗗ
    [Header("�f�o�b�O�m�F�t���O")]
    public bool DebugFlg;
    public Text teamNameList;
    [SerializeField] GameObject EndGamePanel;
    TeamID WinId;
    #endregion

    #region �e�`�[��(�w�c)�̃N���X(TeamInfo).
    /// <summary>
    /// �e�`�[���̏�������N���X
    /// ���̃N���X�ŏ��s�𔻒肵����A�N�e�B�u�����o�[�̐��𐔂���.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        bool isActive;//�������.
        public List<Tank> tankList;
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
            AddMember();
        }
        ~TeamInfo(){}
        #endregion

        #region Tank�̃��X�g�𑀍삷��֐�.
        public void PushTank(Tank tank)
        {
            tankList.Add(tank);
        }

        /// <summary>
        /// �v���C���[�̃^���N���i�[�������X�g�Ŏg�p
        /// ID�ԍ��������ɔ�A�N�e�B�u�ɂ���^���N���w��
        /// </summary>
        public void NotActiveTank(int iD)
        {
            //�^���N���A�N�e�B�u�ɂ���.
            //tankList[iD].
        }

        #endregion

        /// <summary>
        /// ���S�����ꍇ�Ăяo���֐�.
        /// </summary>
        public void Death()
        {
            isActive = false;
            GameManager.instance.CheckActive();
        }
        /// <summary>
        /// �P����int�^�Ő����Ǘ��A0�ɂȂ����玀�S�֐����Ăяo��.
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
        #region ID�ύX�Ereturn�֐�
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
        NowGameState = GAMESTATUS.INGAME;
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
            case GAMESTATUS.ENDGAME:
                EndGameRoop();
                break;
            default:
                Debug.Log("�G���[:�\�����ʃQ�[�����[�h");
                break;
        }

        if (DebugFlg) CheckDebug();
    }
    #endregion

    #region Game�̃X�e�[�^�X���ɓ�����Roop�֐�
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
        //CheckActive();
    }

    /// <summary>
    /// EndGame�̎��ɓ������֐�.
    /// </summary>
    private void EndGameRoop()
    {
        EndGamePanel.SetActive(true);
        //���������`�[����ID��\��.
        EndGamePanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "���������`�[���F" + WinId;
    }
    #endregion

    #region �O������Ăяo���֐�(List�ɒǉ�����֐�).
    /// <summary>
    /// Player�ECPU�𐶐������ۂ�ID���Q�ƁA��v�����炻�̃`�[�����X�g��Tank������.
    /// </summary>
    public void PushTank(TeamID teamID,Tank tank)
    {
        for(int i = 0; i < teamInfo.Count; i++)
        {
            if (teamInfo[i].ReturnID() == teamID)//ID����v������Tank��Push����.
            {
                teamInfo[i].PushTank(tank);
            }
        }
    }

    /// <summary>
    /// TeamList�Ƀ^���N��ǉ�����
    /// ������TeamID���w��
    /// �ǉ�����ۂ�ID�̏d�����Ȃ����m�F.
    /// </summary>
    public void PushTeamList(TeamID teamID)
    {
        if (teamInfo.Count == 0)//���X�g���Ȃ���ԂȂ烋�[�v�����ɒǉ����Ė߂�.
        {
            teamInfo.Add(new TeamInfo(teamID));
            return;
        }
        else
        {
            for (int i = 0; i < teamInfo.Count; i++)//���X�g����S�������ďd���`�F�b�N����.
            {
                if (teamInfo[i].ReturnID() == teamID)//�ǉ�����ID�������ꍇ�A�����o�[��ǉ�����
                {
                    teamInfo[i].AddMember();
                    Debug.Log("�����o�[�ǉ�����");
                    return;//�����o�[�ǉ��������_�Ŋ֐��𔲂���.
                }

            }
            //���[�v�𔲂���=�d���͂Ȃ��̂ŐV���ɒǉ�����.
            teamInfo.Add(new TeamInfo(teamID));
            Debug.Log("���X�g�ǉ�����");
            return;
        }
    }

    #endregion

    /// <summary>
    /// Tank���i�[���Ă��郊�X�g�̃A�N�e�B�u��Ԃ��Q��.
    /// �c���Ă���w�c��1�݂̂Ȃ�Q�[�����I������
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
            NowGameState = GAMESTATUS.ENDGAME;
        }
        Debug.Log("�A�N�e�B�u��" + activeNum);
    }

    #region ���X�g�������E�폜�֐�
    public void PushInitListButton()
    {
        teamInfo = new List<TeamInfo>();
    }
    #endregion

    #region �f�o�b�N�p�֐�

    /// <summary>
    /// DebugFlg��True�̎��ɃC���X�y�N�^�[��,Debug.Log�Ő��l���m�F�ł���
    /// </summary>
    private void CheckDebug()
    {
        teamNameList.text = teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActiveMember() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActiveMember() + ":" + teamInfo[1].ReturnActive() + "\n";
    }

    public void TestDeathplayer()
    {
        teamInfo[0].MemberDeath();
    }
    public void TestDeathPlayer2()
    {
        teamInfo[1].MemberDeath();
    }
    public void TestDeathCPU()
    {
        teamInfo[1].MemberDeath();
    }
    #endregion
}