using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    string HitObjTag;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(HitObjTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        HitObj(other);
    }

    void HitObj(Collider other)
    {
        HitObjTag = other.gameObject.tag;

        switch (HitObjTag)
        {
            case "Bullet":

                gameObject.SetActive(false);
                break;
            case "Tank":

                break;
        }
    }
}
