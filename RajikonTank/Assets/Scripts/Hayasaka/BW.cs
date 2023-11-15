using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BW : MonoBehaviour
{
    // ”š•—‚Å”j‰ó
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ExpZone")
        {
            EffectManager.instance.PlayEffect(ConstList.EffectNames.Effect_BW, transform.position);
            Destroy(this.gameObject);
        }
    }
}
