using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bom : MonoBehaviour
{
    float ExpTime = 5.0f;
    SphereCollider SC;
    // Start is called before the first frame update
    void Start()
    {
        SC = this.transform.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        ExpTime -= Time.deltaTime;

        if(ExpTime < 0.0f)
        {
            
            this.gameObject.SetActive(false);
        }
    }
}
