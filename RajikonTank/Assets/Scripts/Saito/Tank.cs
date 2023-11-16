using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] GameObject Rajikon;
    private string HitObjTag;
    protected TrailRenderer Trail;

    private void OnDisable()
    {
        
    }

    void Start()
    {
        Trail = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(HitObjTag);
    }

    //add.h
    private void OnCollisionEnter(Collision other)
    {
        HitObj(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitObj(other);
    }

    //add.h
    private void HitObj(Collision other)
    {
        HitObjTag = other.gameObject.tag;

        switch (HitObjTag)
        {
            case "Bullet":

                //add.h
                Rajikon.GetComponent<Rajikon>().TankHit();

                //ƒeƒXƒg
                EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_Tank_Deth,transform.position);

                //Rajikon.gameObject.SetActive(false);
                break;
        }
    }

    private void HitObj(Collider other)
    {
        HitObjTag = other.gameObject.tag;

        switch (HitObjTag)
        {
            case "ExpZone":
                Rajikon.GetComponent<Rajikon>().TankHit();
                break;
        }
    }

    public void SetPlayTrail(bool isPlay)
    {
        Trail.emitting = isPlay;
        Trail.Clear();
    }
}
