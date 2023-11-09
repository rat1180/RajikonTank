using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime; // �����܂ł̑ҋ@����
    bool ColFlg;   // ���e�ƃ^���N�̐ڐG����
    bool EffFlg;   // �����G�t�F�N�g���Ŏ~�߂�t���O

    public GameObject EXPZ; �@// �����̓����蔻��I�u�W�F�N�g
    public GameObject ExpObj; // �G�t�F�N�g�̃I�u�W�F�N�g
  
    /// <summary>
    /// ������
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
    /// �����܂ł̎��Ԃ��v��֐�
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
    /// ����������֐�
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
        if(other.gameObject.tag == "Tank")
        {
            ColFlg = true;
        }
    }
}
