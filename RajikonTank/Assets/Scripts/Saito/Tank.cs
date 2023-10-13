using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] GameObject Rajikon;
    string HitObjTag;

    private void OnDisable()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(HitObjTag);
    }

    //add.h
    private void OnCollisionEnter(Collision other)
    {
        HitObj(other);
    }

    //add.h
    void HitObj(Collision other)
    {
        HitObjTag = other.gameObject.tag;

        switch (HitObjTag)
        {
            case "Bullet":

                //add.h
                Rajikon.GetComponent<Rajikon>().TankHit();

                //�e�X�g
                EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Tank_Deth,transform.position);

                //Rajikon.gameObject.SetActive(false);
                break;
            case "Tank":

                break;
        }
    }
}
