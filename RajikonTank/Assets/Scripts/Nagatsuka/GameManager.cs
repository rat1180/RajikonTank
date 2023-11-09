using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] SoundManager soundManager;

    #region �萔�錾
    //InGameCanvas�g�p�萔.
    const int ENEMY_NUM_GROUP = 1;
    const int ENEMY_NUM = 0;
    const int REST_BULLETS_IMAGE = 2;
    const int STATE_STAGE_PANEL = 0;
    const int STAGE_NAME = 1;
    const int INITIAL_ENEMY_NUM = 2;

    //�e�Q�[�����[�h�̃p�l��.
    enum Panels {
        READYGAME,
        INGAME,
        WIN,
        ENDGAME,
        TUTORIAL,
        DEBUG,
        Count,
    }

    #endregion

    #region private�ϐ�
    private StageManager stageManager;
    private bool perfectClearFlg;                            //���S���e�m�F�t���O.
    private int RestBullets;                                 //�v���C���[�̎c�e��.
    private int[] DestroyCPU = new int[(int)EnemyName.COUNT];//���j����CPU�̎�ރJ�E���g�p.
    #endregion

    [Header("�Q�[�����")]
    public GAMESTATUS NowGameState;//���݂̃Q�[�����.

    [Header("�Q�[�����̃L�����o�X")]
    [SerializeField] GameObject GameCanvas;
    [SerializeField] List<GameObject> GamePanel;
        
    [Tooltip("���g�̃v���C���[ID(�}���`������Ƃ��ɂ�����ύX����Ηǂ�)")]
    public int OWN_playerID;

    [Tooltip("���ʕ\���̍ۂɓG�̐����o���Ԋu�𒲐�����p")]
    public float WaitEnemyImage;

    [Header("�X�e�[�W�ԍ�")]
    public int NowStage;

    public int player_IDnum;//Player�����X�g�̉��ԖڂȂ̂����m�F.
    public int CPU_IDnum;   //CPU�����X�g�̉��ԖڂȂ̂����m�F.

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

        public void Active()
        {
            isActive = true;
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
            tankList[id].gameObject.transform.GetChild(GameManager.instance.OWN_playerID).gameObject.transform.position = pos;
        }
        public void SetRotation(int id, GameObject child)
        {
            //tankList[id].gameObject.transform.LookAt(child.transform.position);
            //tankList[id].gameObject.transform.GetChild(GameManager.instance.OWN_playerID).gameObject.transform.LookAt(child.transform.position);
        }
    }

    #endregion

    public List<TeamInfo> teamInfo = new List<TeamInfo>();//�v���C���[�ECPU�̏�Ԃ����郊�X�g.

    #region Unity�C�x���g(Awake�EStart�EUpdate)

    private void Awake()
    {
        instance = this;
        CPU_IDnum = 0;
        player_IDnum = 0;
    }

    private void Start()
    {
        for(int i = 0; i < (int)Panels.Count; i++)//GamePanel�̐������[�v���ă��X�g�ɒǉ�����.
        {
            GamePanel.Add(GameCanvas.transform.GetChild(i).gameObject);
        }

        stageManager = this.transform.GetChild(0).gameObject.GetComponent<StageManager>();
        stageManager.ActiveStage(NowStage);

        perfectClearFlg = false;
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
            case GAMESTATUS.ENDGAME_WIN:
                EndGameWinRoop();
                break;
            default:
                //Debug.Log("�G���[:�\�����ʃQ�[�����[�h");
                break;
        }

        ErrorFunction();

        if (DebugFlg) CheckDebug();
        else GamePanel[(int)Panels.DEBUG].SetActive(false);
    }
    #endregion

    /// <summary>
    /// �Ȃ�炩�̃G���[�����������ۂ�esc�L�[�ŋ����I���ł���悤�ɂ���֐�.
    /// </summary>
    void ErrorFunction()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR //UnityEditor�ŋN�����Ă���Ƃ�.

            UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else //�r���h��.
    Application.Quit();//�Q�[���v���C�I��
#endif
        }
    }

    #region Game�̃X�e�[�^�X���ɓ�����Roop�֐�
    /// <summary>
    /// Ready�̎��ɓ������֐�.
    /// </summary>
    private void ReadyRoop()
    {
        DrawStartStagePanel();
    }

    /// <summary>
    /// InGame�̎��ɓ������֐�.
    /// </summary>
    private void InGameRoop()
    {
        ChangeInGameCanvs();
    }

    /// <summary>
    /// EndGame�̎��ɓ������֐�.
    /// </summary>
    private void EndGameRoop()
    {
        
    }

    void EndGameWinRoop()
    {
        GamePanel[(int)Panels.WIN].transform.GetChild(1).GetComponent<Text>().text =
        stageManager.Stage[NowStage].name + " Clear!!!";    
    }
    #endregion

    /// <summary>
    /// �Q�[�����[�h�ɂ���ĕ\������p�l��1�������A�N�e�B�u�ɂ���֐�
    /// �A�N�e�B�u�ɂ���p�l���������Ɏw��.
    /// </summary>
    private void ActiveGamePanel(int mode)
    {
        for(int i = 0; i < GamePanel.Count -2; i++)//�f�o�b�O�p�l���ƃ`���[�g���A���p�l��������(-2)
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
    public void ActiveTutorial()
    {
        GamePanel[(int)Panels.TUTORIAL].SetActive(true);
    }
    public void CloseTutorial()
    {
        GamePanel[(int)Panels.TUTORIAL].SetActive(false);
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
            }
        }
        if (activeNum == 1)
        {
            JudgeEndGame();
        }
    }

    /// <summary>
    /// �Q�[�����[�h��ENDGAME_WIN�ɕύX���A�p�l�����A�N�e�B�u�ɂ���֐�.
    /// </summary>
    private void ChangeWinMode()
    {
        NowGameState = GAMESTATUS.ENDGAME_WIN;
        StopBGM();
        if (NowStage == 0) CloseTutorial();
        PlaySE(SE_ID.Clear);
        
        ActiveGamePanel((int)Panels.WIN);
    }

    /// <summary>
    /// InGameCanvas�̒l��ύX����֐�
    /// �G�̎c�@���E�c�e����ύX����.
    /// </summary>
    private void ChangeInGameCanvs()
    {
        ActiveGamePanel((int)Panels.INGAME);
        GamePanel[(int)Panels.INGAME].transform.GetChild(ENEMY_NUM_GROUP).gameObject.
            transform.GetChild(ENEMY_NUM).GetComponent<Text>().text =
                                        ": " + teamInfo[CPU_IDnum].ReturnActiveMember();
       RestBullets =teamInfo[player_IDnum].tankList[OWN_playerID].GetRestBullet();
        GamePanel[(int)Panels.INGAME].transform.GetChild(REST_BULLETS_IMAGE).gameObject.GetComponent<Image>().sprite =
            BulletsImage[RestBullets];
    }

    private void DrawStartStagePanel()
    {
        ActiveGamePanel((int)Panels.READYGAME);
        GamePanel[(int)Panels.READYGAME].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(STAGE_NAME).GetComponent<Text>().text = 
            stageManager.Stage[NowStage].name;
        GamePanel[(int)Panels.READYGAME].transform.GetChild(STATE_STAGE_PANEL).gameObject.
            transform.GetChild(INITIAL_ENEMY_NUM).GetComponent<Text>().text = "�G��Ԑ� �~ " + teamInfo[CPU_IDnum].ReturnActiveMember();
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

    public void PlayBGM(BGM_ID id)
    {
        soundManager.PlayBGM(id);
    }
    public void StopBGM()
    {
        soundManager.StopBGM();
    }
    #endregion

    /// <summary>
    /// GameMode��Ready�ɕύX����ۂɌĂяo���֐�.
    /// </summary>
    public void ChangeReadyMode()
    {
        if (stageManager.Stage.Count  == NowStage + 1)//�X�e�[�W���ŏI�X�e�[�W�������ꍇ�A�Q�[�����I������.
        {
            perfectClearFlg = true;
            ChangeGameEnd();
        }
        else
        {
            NowStage++;
            NowGameState = GAMESTATUS.READY;
            stageManager.ActiveStage(NowStage);
            DrawStartStagePanel();
            for (int i = 0; i < teamInfo.Count; i++)//�S�`�[���̎�ޕ����[�v����.
            {
                teamInfo[i].Active();
            }
        }
    }

    /// <summary>
    /// GameEnd(�I��)��GameWin������.
    /// </summary>
    private void JudgeEndGame()
    {
        if (teamInfo[player_IDnum].ReturnActive() == false)
        {
            ChangeGameEnd();
            return;
        }
        ChangeInGameCanvs();//CPU��0�ɂȂ����甽�f.
        ChangeWinMode();
    }

    /// <summary>
    /// Player�����S�A�������͊��S���e�����Ƃ��ɌĂяo�����֐�
    /// BGM�Đ��E���[�h�ύX�����s��.
    /// </summary>
    private void ChangeGameEnd()
    {
        NowGameState = GAMESTATUS.ENDGAME;
        PlayBGM(BGM_ID.Result);
        ActiveGamePanel((int)Panels.ENDGAME);
        if (NowStage == 0) CloseTutorial();
        if (perfectClearFlg)
        {
            GamePanel[(int)Panels.ENDGAME].transform.GetChild(1).gameObject.transform.GetChild(0).
            gameObject.GetComponent<Text>().text = "���S���e�I";
        }
        int cnt = 0;//���S�G�̎�ނ��J�E���g����p.
        for(int i=0;i< DestroyCPU.Length; i++)
        {
            if (DestroyCPU[i] != 0)//��ނ��Ƃ�1�̈ȏ�|���Ă�����J�E���g�𑝂₷.
            {
                cnt++;
            }
        }
        StartCoroutine(ActiveEnemysImage(cnt));
    }

    /// <summary>
    /// CPU�̎��1��1��\�����Ă������߂̃R���[�`��
    /// cnt��������cnt�̐������[�v����
    /// </summary>
    /// <param name="cnt"></param>
    /// <returns></returns>
    IEnumerator ActiveEnemysImage(int cnt)
    {
        int sum = 0;            //�G�̑����j���J�E���g�p.
        GameObject EnemysImage; //�G�̃^���N�摜�\���p.
        EnemysImage = GameCanvas.transform.GetChild((int)Panels.ENDGAME).gameObject.transform.GetChild(3).gameObject;
        for (int i = 0; i < cnt; i++)
        {
            EnemysImage.transform.GetChild(i).gameObject.SetActive(true);
            EnemysImage.transform.GetChild(i).gameObject.transform.GetChild(0).
                gameObject.GetComponent<Text>().text = DestroyCPU[i].ToString();
            yield return new WaitForSeconds(WaitEnemyImage);//�C���X�y�N�^�[��Ŏw�肵�����ԑ҂��Ă��玟��\��.
            sum += DestroyCPU[i];//�����j�������Z.
        }
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(4).gameObject.SetActive(true);
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(2).gameObject.SetActive(true);//�^�C�g���o�b�N�{�^��.
        GamePanel[(int)Panels.ENDGAME].transform.GetChild(4).gameObject.GetComponent<Text>().text = "�����j��:" + sum;
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
        GamePanel[(int)Panels.DEBUG].SetActive(true);
        GamePanel[(int)Panels.DEBUG].transform.GetChild(0).gameObject.GetComponent<Text>().text = 
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
        stageManager.ActiveStage(NowStage);
        DrawStartStagePanel();
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