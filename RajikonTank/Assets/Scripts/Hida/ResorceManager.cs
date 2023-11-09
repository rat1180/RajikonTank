using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ConstList;

public class ResorceManager : MonoBehaviour
{
    //このマネージャーのインスタンス
    public static ResorceManager Instance;

    //ロードしたオブジェクトのリスト(実体を持たない)
    Dictionary<BulletPrefabNames, GameObject> Bullets;
    Dictionary<TankPrefabNames, GameObject> Tanks;
    Dictionary<SE_ID, AudioClip> SEs;
    Dictionary<EffectNames, GameObject> Effects;
    Dictionary<OtherPrefabNames, GameObject> Others;

    //Bullet用生成フォルダへのパス
    const string BulletGenerateFolderName = "Bullets/";
    //Tank用フォルダへのパス
    const string TankGenerateFolderName = "Tanks/";
    //SE用フォルダへのパス
    const string SEGenerateFolderName = "Tanks/";
    //エフェクト用フォルダへのパス
    const string EffectObjectFolderName = "Effects/";
    //その他用フォルダへのパス
    const string OtherGenerateFolderName = "Others/";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

            LoadResorceObjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 必要なリソースをロードする
    /// ロードするリソースはConstListに登録されているリストから参照
    /// ロードする種類それぞれリストを用意する
    /// </summary>
    public void LoadResorceObjects()
    {
        LoadBullets();
        LoadTanks();
        LoadSEs();
    }

    #region ロード関数

    void LoadBullets()
    {
        //初期化
        Bullets = new Dictionary<BulletPrefabNames, GameObject>();

        //弾の名前分繰り返し
        foreach(BulletPrefabNames name in Enum.GetValues(typeof(BulletPrefabNames)))
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceGameObject(BulletGenerateFolderName + name.ToString());

            Bullets.Add(name,prefabobj);
        }
    }

    void LoadTanks()
    {
        //初期化
        Tanks = new Dictionary<TankPrefabNames, GameObject>();

        //弾の名前分繰り返し
        foreach (TankPrefabNames name in Enum.GetValues(typeof(TankPrefabNames)))
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceGameObject(TankGenerateFolderName + name.ToString());

            Tanks.Add(name, prefabobj);
        }
    }

    void LoadSEs()
    {
        //初期化
        SEs = new Dictionary<SE_ID, AudioClip>();

        //弾の名前分繰り返し
        foreach (SE_ID name in Enum.GetValues(typeof(SE_ID)))
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.GetResorceObject(SEGenerateFolderName + name.ToString());

            SEs.Add(name, (AudioClip)prefabobj);
        }
    }

    #endregion

    #region ゲット関数

    public GameObject GetBulletResorce(BulletPrefabNames name)
    {
        if (Bullets.ContainsKey(name))
        {
            return Bullets[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたBulletPrefabNameがリストにありません");
            return new GameObject();
        }
    }

    public GameObject GetTankResorce(TankPrefabNames name)
    {
        if (Tanks.ContainsKey(name))
        {
            return Tanks[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたTankPrefabNameがリストにありません");
            return new GameObject();
        }
    }

    public AudioClip GetSEResorce(SE_ID id)
    {
        if (SEs.ContainsKey(id))
        {
            return SEs[id];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたSE_IDがリストにありません");
            return null;
        }
    }

    public GameObject GetEffectResorce(EffectNames name)
    {
        if (Effects.ContainsKey(name))
        {
            return Effects[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたEffectNameがリストにありません");
            return new GameObject();
        }
    }

    public GameObject GetOtherResorce(OtherPrefabNames name)
    {
        if (Others.ContainsKey(name))
        {
            return Others[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたOtherPrefabNameがリストにありません");
            return new GameObject();
        }
    }

    #endregion

}
