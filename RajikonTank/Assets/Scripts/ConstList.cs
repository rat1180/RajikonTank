using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConstList
{
    public enum KeyList
    {
        NONE,
        A,
        D,
        S,
        W,
        WA,
        WD,
        SA,
        SD,
        UPARROW,
        LEFTARROW,
        RIGHTARROW,
        DOWNARROW,
        FIRE,
        ACCELE,
        BACK,
        LEFTROTATION,
        RIGHTROTATION,
        LEFTHIGHSPEEDROTATION,
        RIGHTHIGHSPEEDROTATION,
    }

    // ゲームパッドの操作モード.
    public enum Controller
    {
        ROOKIE,
        NORMAL,
        RAJICON,
    }

    public enum SE_ID
    {
        Start,
        Move,
        EnemyDeath,
        PlayerDeath,
        Shot,
        Reflect,
        BulletDestroy,
        Ready,
        Clear,
    }

    public enum BGM_ID
    {
        Title,
        Play,
        Result
    }

    public enum TankPrefabNames
    {
        NONE,
        TankBase,
        Enemy_Tutorial,
        Enemy_Normal,
        Enemy_Movement,
        Enemy_FastBullet,
        Enemy_FastAndMove,
        TankEnemy,
    }

    public enum SceneNames
    {
        Title,
        GameScene
    }

    public enum EffectNames
    {
        Effect_Tank_Deth,
        Effect_Bullet_Fire,
        Effect_Bullet_Hit
    }

    /// <summary>
    /// チームID一覧列挙体.
    /// </summary>
    public enum TeamID
    {
        player,
        player2,
        player3,
        player4,
        CPU
    }

    public enum EnemyName // 敵種類
    {
        TUTORIAL,         // 練習敵
        NORMAL,           // 通常敵
        MOVEMENT,         // 移動敵
        FASTBULLET,       // 高速弾敵
        FASTANDMOVE,      // 高速弾と移動敵
        COUNT             // 敵の種類数
    }

    public enum BulletPrefabNames
    {
        NONE,
        RealBullet,
        RealQuickBullet,
        Real3ReflectBullet
    }

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

    //オブジェクト名と生成用フォルダを指定することで
    //そのフォルダからプレファブを検索する
    public static class FolderObjectFinder
    {
        //生成用フォルダへのパス
        const string DefalutGenerateFolderName = "Prefabs/";

        /// <summary>
        /// 生成用フォルダからオブジェクトを探し、返す。
        /// もし見つからなければ空のゲームオブジェクトを返す
        /// デフォルトの生成フォルダが指定されているので、デフォルトからの相対参照で名前を入れる
        /// </summary>
        /// <param name="objectname"></param>
        /// <returns></returns>
        public static GameObject GetResorceObject(string objectname)
        {
            var obj = (GameObject)Resources.Load(DefalutGenerateFolderName+objectname);

            Debug.Log(DefalutGenerateFolderName + objectname);

            if (obj != null) return obj;
            else
            {
                Debug.LogError("リソースファイルに\"" + objectname + "\"が見つかりませんでした");
                //返した先でエラーが起こらないように中身が空のオブジェクトを返す
                return new GameObject();
            }
        }
    }

    //弾を生成する関数を持つstaticクラス
    public static class BulletGenerateClass
    {
        //Bullet用生成フォルダへのパス
        const string GenerateFolderName = "Bullets/";

        /// <summary>
        /// 弾を生成する
        /// 引数：tank          この弾の持ち主として設定するタンク
        ///       parentobject　生成した弾の、親にしたいオブジェクト
        /// 　　　bulletname　　生成する弾の名前
        /// 　　　bulletnm　　　弾の数
        /// 戻り値：弾が正常に生成されればtrueが返る
        /// </summary>
        /// <param name="parentobject"></param>
        /// <param name="bulletname"></param>
        /// <param name="bulletnm"></param>
        /// <returns></returns>
        public static bool BulletInstantiate(GameObject tank,GameObject parentobject,string bulletname,int bulletnm)
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + bulletname);

            //生成数まで繰り返す
            for (int i = 0; i < bulletnm; i++)
            {
                //弾を親オブジェクト基準で生成
                var obj = Object.Instantiate(prefabobj, parentobject.transform.position, parentobject.transform.rotation,parentobject.transform);

                //エラーチェック
                if (obj == null)
                {
                    Debug.LogError("弾の生成に失敗しました");
                    return false;
                }

                //弾にタンクをセット
                //obj.GetComponent<Bullet>().SetTank(tank);
            }
            return true;
        }

        /// <summary>
        /// 弾を一つだけ生成する関数
        /// 生成した弾に対して処理を行いたい場合はこの関数を使う
        /// </summary>
        /// <param name="parentobject"></param>
        /// <param name="buletname"></param>
        /// <returns></returns>
        public static GameObject BulletInstantiateOne(GameObject parentobject, BulletPrefabNames bulletname)
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + bulletname);

            //弾を親オブジェクト基準で生成
            var obj = Object.Instantiate(prefabobj, parentobject.transform.position, parentobject.transform.rotation, parentobject.transform);

            //エラーチェック
            if (obj == null)
            {
                Debug.LogError("弾の生成に失敗しました");
                return new GameObject();
            }
            return obj;
        }
    }

    public static class TankGenerateClass
    {
        //Tank用フォルダへのパス
        const string GenerateFolderName = "Tanks/";

        /// <summary>
        /// タンクを生成する
        /// タンクの種類をTankPrefabNamesの中から選んで引数に入れること
        /// </summary>
        /// <param name="tankname"></param>
        /// <returns></returns>
        public static GameObject TankInstantiate(TankPrefabNames tankname)
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + tankname);

            //タンクを生成
            var obj = Object.Instantiate(prefabobj);

            //ターゲットオブジェクトを生成
            //var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var target = new GameObject();
            target.name = "targetobject";
            obj.GetComponent<Rajikon>().SetTargetObject(target);

            if(obj == null)
            {
                Debug.LogError("タンクの生成に失敗しました");
                return new GameObject();
            }

            return obj;
        }
    }
}