using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class NagatsukaPlayer : MonoBehaviour
{
    [Header("ê∂ê¨Ç∑ÇÈÉ^ÉìÉNÇÃID")]
    public TeamID teamID;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.PushTank(teamID,this.gameObject.GetComponent<Rajikon>());
    }
}
