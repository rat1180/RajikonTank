using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SEObject : MonoBehaviour
{
    [SerializeField, Tooltip("�炷SE�̖��O")] SE_ID SE_Name;
    [SerializeField, Tooltip("�������ɖ炷")] bool PlayOnAwake;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayOnAwake) PlaySE();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySE()
    {
        GameManager.instance.PlaySE(SE_Name);
    }
}
