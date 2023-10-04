using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace ConstList_old
{

    /// <summary>
    /// �Q�[���̌��݂̏�Ԃ�\���񋓑�
    /// </summary>
    public enum GAMESTATUS
    {
        NONE,         //�Q�[���V�[���O�A�������̓Z�b�g����Ă��Ȃ�
        READY,        //�Q�[���J�n�O
        INGAME,       //�Q�[����
        ENDGAME,
        ENDGAME_WIN,  //�Q�[������
        ENDGAME_LOSE, //�Q�[���s�k
        COUNT         //���̗񋓑̂̐�
    }

    /// <summary>
    /// �`�[��ID�ꗗ�񋓑�.
    /// </summary>
    public enum TeamID
    {
        player,
        player2,
        player3,
        player4,
        CPU
    }

    /// <summary>
    /// �Q�[���̐i�s���\���񋓑�
    /// </summary>
    enum GAMEFAZES
    {
        EXPLORE,  //�T�����B�ŏI�����O�܂ł̏��
        LAST,     //�ŏI����
        COUNT     //���̗񋓑̂̐�
    }

    /// <summary>
    /// �A�j���[�V�����p�񋓑�.
    /// </summary>
    public enum AnimCode
    {
        None,
        Idel,
        Walk,
        Run,
        Search
    }

    /// <summary>
    /// SE(�T�E���h)�p�񋓑�.
    /// </summary>
    public enum SEid
    {
        None,
        Search,
        SearchHit,
        SearchNull,
        DoorOpen,
        JailerWalk,
        Arrest,
        Discover,
        EscapeItemSet,
        DoorClose,
        PlayerWalk
    }
    /// <summary>
    /// BGM�p�񋓑�.
    /// </summary>
    public enum BGMid
    {
        NONE,
        TITLE,
        DEFALTGAME,
        CHASE,
        TUTORIALEND,
        ENDING
    }

    public enum SceanNames
    {
        STARTTITLE,
        TUTORIAL,
        LOBBY,
        WAITROOM,
        GAME,
        ENDGAME,
        COUNT
    }

    #region �v���Y���u���C�N�Ŏg�p���Ă����񋓑�.
    public enum InteractObjs
    {
        None,
        Key1,
        Key2,
        Key3,
        Door,
        Prison,
        NullDrop,
        Search,
        EscapeItem1,
        EscapeItem2,
        EscapeObj,
        OpenBearTrap,
        CloseBearTrap,
        Map,
    }

    /// <summary>
    /// �T���|�C���g����o������A�C�e����ID
    /// </summary>
    public enum ItemID
    {
        None,
        Key1,
        Key2,
        Key3,
        EscapeItem1,
        EscapeItem2,
        OpenBearTrap,
        Count
    }
    public enum PlayerColors
    {
        RED,
        GREAN,
        BLUE,
        WHITE,
        COUNT
    }
    #endregion

    /// <summary>
    /// Photon�̃J�X�^���v���p�e�B�g�����\�b�h�p�N���X
    /// �g�p����ꍇ�� Player�N���X.
    /// �Ŏg����
    /// </summary>
    public static class PhotonCustumPropertie
    {
        private const string GameStatusKey = "Gs";
        private const string InitStatusKey = "Is";
        private const string ArrestStatusKey = "As";
        private const string ArrestCntStatusKey = "ACs";
        private const string PlayerColorStatusKey = "PCs";

        private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();

        /// <summary>
        /// ������Photon�̃v���C���[��n�����Ƃ�
        /// �߂�l��GameStatus���Ԃ��Ă���Bint�^�ŕԂ�̂ŁA�L���X�g����
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetGameStatus(this Player player)
        {
            return (player.CustomProperties[GameStatusKey] is int status) ? status : 0;
        }

        /// <summary>
        /// ������Photon�̃v���C���[��GameStatus��n�����Ƃ�
        /// ���v���C���[�ɑ��M����
        /// </summary>
        /// <param name="player"></param>
        public static void SetGameStatus(this Player player, int status)
        {
            propsToSet[GameStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// ������Photon�̃v���C���[��n�����Ƃ�
        /// �߂�l�ł��̃v���C���[�̏�������񂪕Ԃ�
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool GetInitStatus(this Player player)
        {
            return (player.CustomProperties[InitStatusKey] is bool status) ? status : false;
        }

        /// <summary>
        /// ������Photon�̃v���C���[�Ə�������Ԃ�n�����Ƃ�
        /// ���v���C���[�ɑ��M����
        /// </summary>
        /// <param name="player"></param>
        public static void SetInitStatus(this Player player, bool status)
        {
            propsToSet[InitStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// ������Photon�̃v���C���[��n�����Ƃ�
        /// �߂�l�ł��̃v���C���[�̕ߔ���񂪕Ԃ�
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool GetArrestStatus(this Player player)
        {
            return (player.CustomProperties[ArrestStatusKey] is bool status) ? status : false;
        }

        /// <summary>
        /// ������Photon�̃v���C���[�ƕߔ���Ԃ�n�����Ƃ�
        /// ���v���C���[�ɑ��M����
        /// </summary>
        /// <param name="player"></param>
        public static void SetArrestStatus(this Player player, bool status)
        {
            propsToSet[ArrestStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// ������Photon�̃v���C���[��n�����Ƃ�
        /// �߂�l�ł��̃v���C���[�̕ߔ���񂪕Ԃ�
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static float GetArrestCntStatus(this Player player)
        {
            return (player.CustomProperties[ArrestCntStatusKey] is int cnt) ? cnt : 0;
        }

        /// <summary>
        /// ������Photon�̃v���C���[�ƕߔ���Ԃ�n�����Ƃ�
        /// ���v���C���[�ɑ��M����
        /// </summary>
        /// <param name="player"></param>
        public static void SetArrestCntStatus(this Player player, float cnt)
        {
            propsToSet[ArrestCntStatusKey] = cnt;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static int GetPlayerColorStatus(this Player player)
        {
            return (player.CustomProperties[PlayerColorStatusKey] is int cnt) ? cnt : 0;
        }

        public static void SetPlayerColorStatus(this Player player,int color)
        {
            propsToSet[PlayerColorStatusKey] = color;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}