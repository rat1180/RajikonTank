using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BW : MonoBehaviour
{
    // �����Ŕj��
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ExpZone")
        {
            Destroy(this.gameObject);
        }
    }
}
