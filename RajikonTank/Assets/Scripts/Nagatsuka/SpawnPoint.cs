using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SpawnPoint : MonoBehaviour
{
    [Header("��������^���N��ID")]
    public TeamID teamID;
    public EnemyName enemyName;//CPU�̏ꍇ���O���w��.

    [Header("�ړ�����G��2�_")]
    public Vector3[] position = new Vector3[2];

    private void Start()
    {
        //if (enemyName == EnemyName.MOVEMENT)//��������G�̏ꍇ��2�_�Ԃ��Q�Ƃ���
        //{
        //    position[0] = transform.GetChild(0).gameObject.transform.position;
        //    position[1] = transform.GetChild(1).gameObject.transform.position;
        //}
    }
}
