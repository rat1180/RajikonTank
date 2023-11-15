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
    public List<Vector3> position = new List<Vector3>();

    private void Awake()
    {
        if (this.transform.childCount == 0)
        {
            //Debug.Log("�q�v�f�Ȃ���");
        }
        else if (enemyName == EnemyName.MOVEMENT || enemyName == EnemyName.FASTANDMOVE)//��������G�̏ꍇ��2�_�Ԃ��Q�Ƃ���
        {
            for(int i=0;i< this.transform.childCount; i++)
            {
                position.Add(transform.GetChild(i).gameObject.transform.position);
            }
        }
    }
}
