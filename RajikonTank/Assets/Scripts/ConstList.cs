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
        UPARROW,
        RIGHTARROW,
        LEFTARROW,
        DOWNARROW,
        SPACE,
    }

    //弾を生成する関数を持つstaticクラス
    public static class BulletGenerateClass
    {
        //生成用フォルダへのパス
        const string GenerateFolderName = "";

        /// <summary>
        /// 生成用フォルダからオブジェクトを探し、返す。
        /// もし見つからなければ空のゲームオブジェクトを返す
        /// </summary>
        /// <param name="objectname"></param>
        /// <returns></returns>
        public static GameObject GetResorceObject(string objectname)
        {
            var obj = (GameObject)Resources.Load(objectname);

            if (obj != null) return obj;
            else
            {
                Debug.LogError("リソースファイルに\"" + objectname + "\"が見つかりませんでした");
                //返した先でエラーが起こらないように中身が空のオブジェクトを返す
                return new GameObject();
            }
        }

        /// <summary>
        /// 弾を生成する
        /// 引数：parentobject　生成した弾の、親にしたいオブジェクト
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
            var prefabobj = GetResorceObject(GenerateFolderName + bulletname);

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

    }
}