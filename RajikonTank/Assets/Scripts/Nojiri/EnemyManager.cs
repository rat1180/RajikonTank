using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;              // �Q�Ɨp
    public Vector3[] MovePointsArray { get; set; }   // ����p�̈ʒu���z��

    private void Awake()
    {
        instance = this;
    }

    #region GameManager�p���\�b�h
    /// <summary>
    /// �ʏ�G����
    /// </summary>
    /// <param name="getSpawnPos">�����ʒu</param>
    /// <param name="spawnName">��������G�̖��O</param>
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // �^���N����
        enemyChildObj.transform.parent = this.transform;  // ���������G���q�I�u�W�F�N�g�ɒǉ�
        enemyChildObj.transform.position = getSpawnPos;   // �󂯎���������ʒu�ɐݒ�
    }

    /// <summary>
    /// �O�����珄��ʒu�̔z����擾
    /// ����ꂽ�ʒu����ۑ�
    /// </summary>
    /// <param name="points">����ʒu�̔z��</param>
    public void PatrolPositionSet(Vector3[] points)
    {
        // �v�f���ƈʒu�̃R�s�[
        MovePointsArray = new Vector3[points.Length];

        for(int i = 0; i < points.Length; i++)
        {
            MovePointsArray[i] = points[i];
        }
    }
    #endregion

    #region StateBaseAI�p���\�b�h
    /// <summary>
    /// StateBaseAI�p
    /// ����ʒu���̎擾�p���\�b�h
    /// PatrolPositionSet���s�O�A�Ԃ�l�FNULL
    /// PatrolPositionSet���s��A�Ԃ�l�F�ʒu���z��
    /// </summary>
    /// <returns></returns>
    //public Vector3[] PatrolPositionGet()
    //{
    //    return MovePointsArray;
    //}
    #endregion
}
