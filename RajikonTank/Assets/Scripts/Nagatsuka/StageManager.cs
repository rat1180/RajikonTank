using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class StageManager : MonoBehaviour
{
    #region 定数値
    const int PLAYER_NUM = 0;
    const int CPU_NUM = 1;
    #endregion
    [SerializeField] GameObject SpawnPoints;
    [SerializeField] GameObject[] tanks;

    [Header("確認用変数")]
    [SerializeField]int ChindCnt;
    // Start is called before the first frame update
    void Start()
    {
        ChindCnt = SpawnPoints.transform.childCount;
        TeamID teamID;
        for(int i = 0; i < ChindCnt; i++)
        {
            teamID = SpawnPoints.transform.GetChild(i).gameObject.GetComponent<SpawnPoint>().teamID;
            Debug.Log(SpawnPoints.transform.GetChild(i).gameObject.transform.position.x);
            CreateTank(teamID, SpawnPoints.transform.GetChild(i).gameObject.transform.position);
        }
    }
    void CreateTank(TeamID teamID,Vector3 position)
    {
        switch (teamID) {
            case TeamID.player:
                Instantiate(tanks[PLAYER_NUM], position, Quaternion.identity);
                //Instantiate(tanks[PLAYER_NUM], new Vector3(-1.76f,0f,0f), Quaternion.identity);
                break;
            case TeamID.CPU:
                Instantiate(tanks[CPU_NUM], position, Quaternion.identity);
                break;
        }
    }
}
