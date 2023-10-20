using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resultTanks : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void StopAnim()
    {
        anim.enabled = false; ;
    }
}
