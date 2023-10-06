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

    #region �萔�錾
    //InGameCanvas�g�p�萔.
    const int OPERATION_IMAGE = 0;
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    #endregion

    [Header("�Q�[�����")]
    public GAMESTATUS NowGameState;//���݂̃Q�[�����.

    [Header("InGame���̃L�����o�X�֘A")]
    [SerializeField] GameObject InGameCanvas;//InGame���ɕ\�����Ă���L�����o�X(UI).
    public int RestBullets;                  //�c�e��.

    [Header("�g�p�摜")]
    [SerializeField] Sprite[] BulletsImage;  //�c�e���\���摜.

    #region �f�o�b�N�m�F�p�ꗗ
    [Header("�f�o�b�O�m�F�t���O")]
    public bool DebugFlg;                  //ONOFF�Ńf�o�b�N�̕\����؂芷����.
    [SerializeField] GameObject DebugPanel;//�f�o�b�N�����܂Ƃ߂��p�l��.
    public Text teamNameList;              //�`�[����ID�E���E������Ԃ�\���p.
    TeamID WinId;                          //���҂�ID.    
    #endregion

    [SerializeField] GameObject EndGamePanel;//�Q�[���I�����ɕ\������p�l��.

    public int player_IDnum;//Player�����X�g�̉��ԖڂȂ̂����m�F.
    public int CPU_IDnum;   //CPU�����X�g�̉��ԖڂȂ̂����m�F.

    public GameObject testOBJ;

    #region �e�`�[��(�w�c)�̃N���X(TeamInfo).
    /// <summary>
    /// �e�`�[���̏�������N���X
    /// ���̃N���X�ŏ��s�𔻒肵����A�N�e�B�u�����o�[�̐��𐔂���.
    /// </summary>
    public class TeamInfo {
        TeamID ID;
        bool isActive;//�������.
        public List<Rajikon> tankList;
        public int memberNum;//�`�[���̐����l�����J�E���g.

        #region �R���X�g���N�^�E�f�X�g���N�^
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

        #region Tank�̃��X�g�𑀍삷��֐�.
        public void PushTank(Rajikon tank)
        {
            tankList.Add(tank);
            memberNum++;
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
        CPU_IDnum = 0;
        player_IDnum = 0;
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
        else DebugPanel.SetActive(false);
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
        CheckActive();
        ChangeInGameCanvs();
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

    #region �O������Ăяo���֐�(List�ɒǉ�����E���S�֐�).
    /// <summary>
    /// Player�ECPU�𐶐������ۂ�ID���Q�ƁA��v�����炻�̃`�[�����X�g��Tank������.
    /// </summary>
    public void PushTank(TeamID teamID,Rajikon tank)
    {
        int cnt = 0;
        if (teamInfo.Count == 0)//���X�g���Ȃ���ԂȂ烋�[�v�����ɒǉ����Ė߂�.
        {
            teamInfo.Add(new TeamInfo(teamID,tank));
            if (teamID == TeamID.CPU) CPU_IDnum = 0;
            if (teamID == TeamID.player) player_IDnum = 0;
            return;
        }
        else
        {
            for (int i = 0; i < teamInfo.Count; i++)//���X�g����S�������ďd���`�F�b�N����.
            {
                if (teamInfo[i].ReturnID() == teamID)//�ǉ�����ID�������ꍇ�A�����o�[��ǉ�����
                {
                    //teamInfo[i].AddMember();
                    teamInfo[i].PushTank(tank);
                    Debug.Log("�����o�[�ǉ�����");
                    return;//�����o�[�ǉ��������_�Ŋ֐��𔲂���.
                }
                cnt++;
            }
            //���[�v�𔲂���=�d���͂Ȃ��̂ŐV���ɒǉ�����.
            teamInfo.Add(new TeamInfo(teamID,tank));
            if (teamID == TeamID.CPU) CPU_IDnum = cnt;
            if (teamID == TeamID.player) player_IDnum = cnt;
            Debug.Log("���X�g�ǉ�����");
            return;
        }

        //for(int i = 0; i < teamInfo.Count; i++)
        //{
        //    if (teamInfo[i].ReturnID() == teamID)//ID����v������Tank��Push����.
        //    {
        //        teamInfo[i].PushTank(tank);
        //    }
        //}
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

    /// <summary>
    /// �^���N�����S�����ۂɌĂяo���֐�.
    /// </summary>
    public void DeathTank(TeamID teamID)
    {
        for(int i = 0; i < teamInfo.Count; i++)//���X�g����S�������ďd���`�F�b�N����.
            {
            if (teamInfo[i].ReturnID() == teamID)//ID�������ꍇ�A�����o�[������(���S)����.
            {
                teamInfo[i].MemberDeath();
                Debug.Log("�����o�[���S����");
                return;//�����o�[�ǉ��������_�Ŋ֐��𔲂���.
            }

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

    /// <summary>
    /// InGameCanvas�̒l��ύX����֐�
    /// �G�̎c�@���E�c�e����ύX����.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        InGameCanvas.transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ":" + teamInfo[CPU_IDnum].ReturnActiveMember();
        Debug.Log(teamInfo[CPU_IDnum].ReturnActiveMember());
        InGameCanvas.transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
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
        DebugPanel.SetActive(true);
        teamNameList.text = teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActiveMember() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActiveMember() + ":" + teamInfo[1].ReturnActive() + "\n";
        //Debug.Log("CPUIDNUM" + CPU_IDnum);
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