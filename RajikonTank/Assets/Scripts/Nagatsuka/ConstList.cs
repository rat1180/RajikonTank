using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace ConstList
{

    /// <summary>
    /// ゲームの現在の状態を表す列挙体
    /// </summary>
    public enum GAMESTATUS
    {
        NONE,         //ゲームシーン外、もしくはセットされていない
        READY,        //ゲーム開始前
        INGAME,       //ゲーム中
        ENDGAME,
        ENDGAME_WIN,  //ゲーム勝利
        ENDGAME_LOSE, //ゲーム敗北
        COUNT         //この列挙体の数
    }

    /// <summary>
    /// ゲームの進行具合を表す列挙体
    /// </summary>
    enum GAMEFAZES
    {
        EXPLORE,  //探索中。最終部屋前までの状態
        LAST,     //最終部屋
        COUNT     //この列挙体の数
    }

    /// <summary>
    /// アニメーション用列挙体.
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
    /// SE(サウンド)用列挙体.
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
    /// BGM用列挙体.
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

    #region プリズンブレイクで使用していた列挙体.
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
    /// 探索ポイントから出現するアイテムのID
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
    /// Photonのカスタムプロパティ拡張メソッド用クラス
    /// 使用する場合は Playerクラス.
    /// で使える
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
        /// 引数でPhotonのプレイヤーを渡すことで
        /// 戻り値でGameStatusが返ってくる。int型で返るので、キャストする
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetGameStatus(this Player player)
        {
            return (player.CustomProperties[GameStatusKey] is int status) ? status : 0;
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーとGameStatusを渡すことで
        /// 他プレイヤーに送信する
        /// </summary>
        /// <param name="player"></param>
        public static void SetGameStatus(this Player player, int status)
        {
            propsToSet[GameStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーを渡すことで
        /// 戻り値でそのプレイヤーの初期化情報が返る
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool GetInitStatus(this Player player)
        {
            return (player.CustomProperties[InitStatusKey] is bool status) ? status : false;
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーと初期化状態を渡すことで
        /// 他プレイヤーに送信する
        /// </summary>
        /// <param name="player"></param>
        public static void SetInitStatus(this Player player, bool status)
        {
            propsToSet[InitStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーを渡すことで
        /// 戻り値でそのプレイヤーの捕縛情報が返る
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool GetArrestStatus(this Player player)
        {
            return (player.CustomProperties[ArrestStatusKey] is bool status) ? status : false;
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーと捕縛状態を渡すことで
        /// 他プレイヤーに送信する
        /// </summary>
        /// <param name="player"></param>
        public static void SetArrestStatus(this Player player, bool status)
        {
            propsToSet[ArrestStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーを渡すことで
        /// 戻り値でそのプレイヤーの捕縛情報が返る
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static float GetArrestCntStatus(this Player player)
        {
            return (player.CustomProperties[ArrestCntStatusKey] is int cnt) ? cnt : 0;
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーと捕縛状態を渡すことで
        /// 他プレイヤーに送信する
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
