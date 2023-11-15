using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime; // 爆発までの待機時間
    float MatTime;

    int MatCnt;
    bool ColFlg;   // 爆弾とタンクの接触判定
    bool EffFlg;   // 爆発エフェクト一回で止めるフラグ
    bool MatFlg;

    public GameObject EXPZ; 　// 爆風の当たり判定オブジェクト
    public GameObject ExpObj; // エフェクトのオブジェクト

    public Material DefMat;
    public Material RedMat;
    /// <summary>
    /// 初期化
    /// </summary>
    void InitBom()
    {
        ExpTime = 5.0f;
        MatTime = 0.0f;
        MatCnt = 0;
        ColFlg = false;
        EffFlg = false;
        MatFlg = false;

        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
        EXPZ.SetActive(false);
    }
    private void OnDisable()
    {
        InitBom();
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
        MatTime += Time.deltaTime;

        if (MatTime > 0.5f && MatCnt < 3)
        {
            this.GetComponent<MeshRenderer>().material = RedMat;
            if (MatTime > 0.55f)
            {
                MatTime = 0.0f;
                MatCnt++;
            }
        }
        if (MatTime > 0.3f)
        {
            if (MatCnt >= 3 && MatCnt < 9)
            {
                this.GetComponent<MeshRenderer>().material = RedMat;
                if (MatTime > 0.33f)
                {
                    MatTime = 0.0f;
                    MatCnt++;
                }
            }
        }
        if (MatTime > 0.1f)
        {
            if (MatCnt >= 9)
            {
                this.GetComponent<MeshRenderer>().material = RedMat;
                if (MatTime > 0.11f)
                {
                    MatTime = 0.0f;
                    MatCnt++;
                }
            }
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = DefMat;
        }
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
            EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Bom, transform.position);
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
        if(other.gameObject.tag == "Bullet")
        {
            ColFlg = true;
        }
    }
}
