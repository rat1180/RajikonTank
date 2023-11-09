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
    public Vector3[] position = new Vector3[2];

    private void Start()
    {
        //if (enemyName == EnemyName.MOVEMENT)//往復する敵の場合は2点間を参照する
        //{
        //    position[0] = transform.GetChild(0).gameObject.transform.position;
        //    position[1] = transform.GetChild(1).gameObject.transform.position;
        //}
    }
}
