using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime;
    bool ColFlg;
    bool EffFlg;
    public GameObject EXPZ;
    public GameObject ExpObj;
    // Start is called before the first frame update
    void InitBom()
    {
        ExpTime = 5.0f;
        ColFlg = false;
        EffFlg = false;
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
    // Update is called once per frame
    void Update()
    {
        ExpTime -= Time.deltaTime;

        if(ExpTime < 0.0f || ColFlg)
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
    }
    void BomDes()
    {
        this.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Tank")
        {
            ColFlg = true;
        }
    }
}
