using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class NagatsukaPlayer : MonoBehaviour
{
    [Header("�������郁���o�[�̃`�[��ID")]
    public TeamID teamID;
    
    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.PushTeamList(teamID);
        GameManager.instance.PushTank(teamID,this.gameObject.GetComponent<Rajikon>());
    }
}
