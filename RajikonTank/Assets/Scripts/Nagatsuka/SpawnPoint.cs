using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SpawnPoint : MonoBehaviour
{
    [Header("生成するタンクのID")]
    public TeamID teamID;
    public EnemyName enemyName;//CPUの場合名前も指定.

    [Header("移動する敵の2点")]
    public List<Vector3> position = new List<Vector3>();

    private void Awake()
    {
        if (this.transform.childCount == 0)
        {
            //Debug.Log("子要素ないよ");
        }
        else if (enemyName == EnemyName.MOVEMENT || enemyName == EnemyName.FASTANDMOVE)//往復する敵の場合は2点間を参照する
        {
            for(int i=0;i< this.transform.childCount; i++)
            {
                position.Add(transform.GetChild(i).gameObject.transform.position);
            }
        }
    }
}
