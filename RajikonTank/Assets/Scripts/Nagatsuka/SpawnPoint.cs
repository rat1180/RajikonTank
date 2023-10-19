using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SpawnPoint : MonoBehaviour
{
    [Header("生成するタンクのID")]
    public TeamID teamID;
    public EnemyName enemyName;//CPUの場合名前も指定.
}
