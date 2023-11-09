using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime; // 爆発までの待機時間
    bool ColFlg;   // 爆弾とタンクの接触判定
    bool EffFlg;   // 爆発エフェクト一回で止めるフラグ

    public GameObject EXPZ; 　// 爆風の当たり判定オブジェクト
    public GameObject ExpObj; // エフェクトのオブジェクト
  
    /// <summary>
    /// 初期化
    /// </summary>
    void InitBom()
    {
        ExpTime = 20.0f;
        ColFlg = false;
        EffFlg = false;
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
        EXPZ.SetActive(false);
    }
    private void OnDisable()
    {
        //InitBom();
    }
    void Start()
    {
        InitBom();
    }
    void Update()
    {
        CountExp();
    }
    /// <summary>
    /// 爆発までの時間を計る関数
    /// </summary>
    void CountExp()
    {
        ExpTime -= Time.deltaTime;

        if (ExpTime < 0.0f || ColFlg)
        {
            Explosion();
        }
    }
    /// <summary>
    /// 爆発させる関数
    /// </summary>
    void Explosion()
    {
        if (!EffFlg)
        {
            EffFlg = true;
            Instantiate(ExpObj, this.transform.position, Quaternion.identity);
        }
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        EXPZ.SetActive(true);
        Invoke("BomDes", 2);
    }
    /// <summary>
    /// 爆弾を非表示に
    /// </summary>
    void BomDes()
    {
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// タンクとの当たり判定
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Tank")
        {
            ColFlg = true;
        }
    }
}
