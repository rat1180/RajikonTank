using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ConstList;

/// <summary>
/// エフェクトの管理を行うクラス
/// エフェクトはこのクラスで一括管理され、
/// エフェクトを発生させる場合はこのクラスに要求する。
/// 
/// このオブジェクトの子に各エフェクトが収納される
/// </summary>
public class EffectManager : MonoBehaviour
{

    #region 変数

    [SerializeField, Header("ゲームマネージャーへのインスタンス")] GameManager GameManagerInstance;
    [SerializeField, Header("使用するエフェクトのリスト")] List<GameObject> EffectList = new List<GameObject>();
    [SerializeField, Header("エフェクトをまとめている子オブジェクト")] GameObject EffectParent;

    #endregion

    //テスト
    public static EffectManager instance;

    #region Unityイベント

    // Start is called before the first frame update
    void Start()
    {
        Init();

        //テスト
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region 関数

    #region 初期化関数

    /// <summary>
    /// このマネージャーのインスタンスをゲームマネージャーにセットし、
    /// 各エフェクトの検索を行い、リストにセットする
    /// 何らかのタイミングで再度設定を行いたい場合、外部からこの関数を呼ぶ
    /// </summary>
    public void Init()
    {
        //ゲームマネージャーのインスタンスを取得
        SetGameManagerInstance();

        //自身のエフェクトリストをクリアし、セットする
        InitEffectList();
    }

    private void SetGameManagerInstance()
    {
        GameManagerInstance = GameManager.instance;
        if(GameManagerInstance == null)
        {
            Debug.LogWarning("GameManagerが見つかりません");
        }
    }

    /// <summary>
    /// エフェクトリストのクリアとセットを行う
    /// </summary>
    private void InitEffectList()
    {
        //エフェクトまとめオブジェクトのクリア
        ClearEffects();

        //エフェクトリストをクリアする
        EffectList.Clear();

        //列挙体の名前基準でフォルダーを探索し、プレファブを返す
        foreach(var effectname in Enum.GetValues(typeof(EffectNames))){
            //EffectList.Add(FolderObjectFinder.GetResorceGameObject(EffectObjectFolderName + effectname));
            EffectList.Add(ResorceManager.Instance.GetEffectResorce((EffectNames)effectname));
        }
    }

    #endregion

    /// <summary>
    /// エフェクトをすべてクリアする
    /// </summary>
    public void ClearEffects()
    {
        if(EffectParent != null)
        {
            Destroy(EffectParent);
        }
        EffectParent = new GameObject();
        EffectParent.name = "EffectParent";
        EffectParent.transform.parent = gameObject.transform;
        EffectParent.transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// エフェクトを再生する
    /// </summary>
    /// <param name="effect"></param>
    public void PlayEffect(EffectNames effect,Vector3 pos)
    {
        var obj = Instantiate(EffectList[(int)effect]);
        obj.transform.parent = EffectParent.transform;
        obj.transform.position = pos;
    }

    #endregion
}
