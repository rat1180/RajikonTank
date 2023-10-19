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
    [SerializeField] SoundManager soundManager;
    #region �萔�錾
    //InGameCanvas�g�p�萔.
    const int OPERATION_IMAGE = 0;
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    const int STATE_STAGE_PANEL = 0;
    const int STAGE_NAME = 1;
    const int INITIAL_ENEMY_NUM = 2;

    const int READYGAMEPANEL = 0; 
    const int INGAMEPANEL = 1;
    const int ENDGAMEPANEL = 2;
    const int DEBUGPANEL = 3;//�f�o�b�N�����܂Ƃ߂��p�l��.
    #endregion

    [Header("�Q�[�����")]
    public GAMESTATUS NowGameState;//���݂̃Q�[�����.

    [Header("�Q�[�����̃L�����o�X")]
    [SerializeField] GameObject GameCanvas;
    [SerializeField] List<GameObject> GamePanel;

    public int RestBullets;                  //�c�e��.

    [Header("�X�e�[�W�ԍ�")]
    public int NowStage;

    public int player_IDnum;//Player�����X�g�̉��ԖڂȂ̂����m�F.
    public int CPU_IDnum;   //CPU�����X�g�̉��ԖڂȂ̂����m�F.
    public int[] DestroyCPU = new int[(int)EnemyName.COUNT];//���j����CPU���J�E���g.
    TeamID WinId;                          //���������`�[����ID.

    [Header("�g�p�摜")]
    [SerializeField] Sprite[] BulletsImage;  //�c�e���\���摜.

    #region �f�o�b�N�m�F�p�ꗗ
    [Header("�f�o�b�O�p�l���m�F�t���O")]
    public bool DebugFlg;                  //ONOFF�Ńf�o�b�N�̕\����؂芷����.
    #endregion

    

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

        public void SetPosition(int id,Vector3 pos)
        {
            tankList[id].gameObject.transform.position = pos;
            Debug.Log("SetPosition�N��");
        }
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
        GamePanel.Add(GameCanvas.transform.GetChild(READYGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(INGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(ENDGAMEPANEL).gameObject);
        GamePanel.Add(GameCanvas.transform.GetChild(DEBUGPANEL).gameObject);
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().ActiveStage(NowStage);
    }

    private void Update()
    {
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
        else GamePanel[DEBUGPANEL].SetActive(false);
    }
    #endregion

    #region Game�̃X�e�[�^�X���ɓ�����Roop�֐�
    /// <summary>
    /// Ready�̎��ɓ������֐�.
    /// </summary>
    private void ReadyRoop()
    {
        ActiveGamePanel(READYGAMEPANEL);
        DrawStateStagePanel();
        Debug.Log("CPU��:" + teamInfo[CPU_IDnum].ReturnActiveMember());
    }

    /// <summary>
    /// InGame�̎��ɓ������֐�.
    /// </summary>
    private void InGameRoop()
    {
        ChangeInGameCanvs();
        Debug.Log("teamInfo[CPU_IDnum].ReturnActiveNum()"+ teamInfo[CPU_IDnum].ReturnActiveMember());
    }

    /// <summary>
    /// EndGame�̎��ɓ������֐�.
    /// </summary>
    private void EndGameRoop()
    {
        ActiveGamePanel(ENDGAMEPANEL);
        //���������`�[����ID��\��.
        GamePanel[ENDGAMEPANEL].transform.GetChild(0).gameObject.GetComponent<Text>().text = "���������`�[���F" + WinId;
    }
    #endregion

    /// <summary>
    /// �Q�[�����[�h�ɂ���ĕ\������p�l��1�������A�N�e�B�u�ɂ���֐�
    /// �A�N�e�B�u�ɂ���p�l���������Ɏw��.
    /// </summary>
    private void ActiveGamePanel(int mode)
    {
        for(int i = 0; i < 3; i++)
        {
            if(i == mode)
            {
                GamePanel[i].SetActive(true);
            }
            else
            {
                GamePanel[i].SetActive(false);
            }
        }
    }

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
            if (teamInfo[i].ReturnID() == teamID)//ID�������ꍇ�A�����o�[������(���S)������.
            {
                teamInfo[i].MemberDeath();
                CheckActive();
                Debug.Log("�����o�[���S����");
                return;//�����o�[�ǉ��������_�Ŋ֐��𔲂���.
            }
        }
    }
    /// <summary>
    /// �^���N�����S�����ۂɌĂяo���֐�
    /// CPU��ID���ꏏ�Ɉ����Ɏw��A���j����CPU�̎�ނ��J�E���g����.
    /// </summary>
    public void DeathTank(TeamID teamID,EnemyName name)
    {
        for (int i = 0; i < teamInfo.Count; i++)//���X�g����S�������ďd���`�F�b�N����.
        {
            if (teamInfo[i].ReturnID() == teamID)//ID�������ꍇ�A�����o�[������(���S)������.
            {
                teamInfo[i].MemberDeath();
                for(int j = 0; j < DestroyCPU.Length; j++)//CPU�̎�ނ𔻒f����.
                {
                    if (j == (int)name)
                    {
                        DestroyCPU[j]++;
                    }
                }
                CheckActive();
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
            ChangeInGameCanvs();//CPU��0�ɂȂ����甽�f.
            NowGameState = GAMESTATUS.ENDGAME;
        }
    }

    /// <summary>
    /// InGameCanvas�̒l��ύX����֐�
    /// �G�̎c�@���E�c�e����ύX����.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        ActiveGamePanel(INGAMEPANEL);
        GamePanel[INGAMEPANEL].transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ":" + teamInfo[CPU_IDnum].ReturnActiveMember();
        GamePanel[INGAMEPANEL].transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
    }

    private void DrawStateStagePanel()
    {
        
        GamePanel[READYGAMEPANEL].SetActive(true);
        GamePanel[READYGAMEPANEL].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(STAGE_NAME).GetComponent<Text>().text = 
            this.transform.GetChild(0).gameObject.GetComponent<StageManager>().Stages[NowStage].name;
        GamePanel[READYGAMEPANEL].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(INITIAL_ENEMY_NUM).GetComponent<Text>().text = "�G��Ԑ�:" + teamInfo[CPU_IDnum].ReturnActiveMember() + "��";
    }

    #region SoundManager�֐�
    public void PlaySE(SE_ID id)
    {
        soundManager.PlaySE(id);
    }
    public void PlaySE(AudioClip audioClip)
    {
        soundManager.PlaySE(audioClip);
    }
    public AudioClip ReturnSE(SE_ID id)
    {
        AudioClip audioClip;
        audioClip = soundManager.ReturnSE(id);
        return audioClip;
    }
    #endregion

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
        GamePanel[DEBUGPANEL].SetActive(true);
        GamePanel[DEBUGPANEL].transform.GetChild(0).gameObject.GetComponent<Text>().text = 
                            teamInfo[0].ReturnID().ToString() + ":" + teamInfo[0].ReturnActiveMember() + ":" + teamInfo[0].ReturnActive() + "\n" +
                            teamInfo[1].ReturnID().ToString() + ":" + teamInfo[1].ReturnActiveMember() + ":" + teamInfo[1].ReturnActive() + "\n";
    }

    /// <summary>
    /// �Q�[���I�����ɃV�[��������������(Reload)�֐�
    /// </summary>
    public void ResetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        NowStage++;
        NowGameState = GAMESTATUS.READY;
        this.transform.GetChild(0).gameObject.GetComponent<StageManager>().ActiveStage(NowStage);
        DrawStateStagePanel();
    }

    public void TestDeathplayer()
    {
        //teamInfo[0].MemberDeath();
        PlaySE(SE_ID.PlayerDeath);
    }
    public void TestDeathCPU()
    {
        //teamInfo[1].MemberDeath();
        //PlaySE(SE_ID.Move);
        soundManager.PlaySE(soundManager.ReturnSE(SE_ID.Move));
    }
    #endregion
}