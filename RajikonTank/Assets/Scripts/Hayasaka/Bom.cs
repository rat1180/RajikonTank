using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime; // �����܂ł̑ҋ@����
    float MatTime;

    int MatCnt;
    bool ColFlg;   // ���e�ƃ^���N�̐ڐG����
    bool EffFlg;   // �����G�t�F�N�g���Ŏ~�߂�t���O
    bool MatFlg;

    public GameObject EXPZ; �@// �����̓����蔻��I�u�W�F�N�g
    public GameObject ExpObj; // �G�t�F�N�g�̃I�u�W�F�N�g

    public Material DefMat;
    public Material RedMat;
    /// <summary>
    /// ������
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
    /// �����܂ł̎��Ԃ��v��֐�
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
    /// ����������֐�
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
    /// ���e���\����
    /// </summary>
    void BomDes()
    {
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// �^���N�Ƃ̓����蔻��
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            ColFlg = true;
        }
    }
}
