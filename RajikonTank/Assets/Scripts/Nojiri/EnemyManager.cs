using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class EnemyManager : MonoBehaviour
{
    public Vector3[] MovePointsArray { get; set; }   // ����p�̈ʒu���z��

    #region GameManager�p���\�b�h
    /// <summary>
    /// �G����(����Ȃ�ver)
    /// 
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
    /// �G����(���񂠂�ver)
    /// </summary>
    /// <param name="getSpawnPos">�����ʒu</param>
    /// <param name="spawnName">��������G�̖��O</param>
    /// <param name="points">���񂷂�ʒu��񃊃X�g</param>
    public void SpawnEnemy(Vector3 getSpawnPos, TankPrefabNames spawnName,List<Vector3> points)
    {
        GameObject enemyChildObj;
        enemyChildObj = TankGenerateClass.TankInstantiate(spawnName); // �^���N����
        enemyChildObj.transform.parent = this.transform;  // ���������G���q�I�u�W�F�N�g�ɒǉ�
        enemyChildObj.transform.position = getSpawnPos;   // �󂯎���������ʒu�ɐݒ�
        enemyChildObj.GetComponent<StateBaseAI>().SetPatrolPoint(points);
        PatrolPositionSet(points);  // ���X�g�̏����R�s�[
    }

    /// <summary>
    /// ����ʒu�̃��X�g���擾
    /// ����ꂽ�ʒu����ۑ�
    /// </summary>
    /// <param name="points">���񂷂�ʒu��񃊃X�g</param>
    public void PatrolPositionSet(List<Vector3> points)
    {
        // �v�f���ƈʒu�̃R�s�[
        MovePointsArray = new Vector3[points.Count];

        for(int i = 0; i < points.Count; i++)
        {
            MovePointsArray[i] = points[i];
        }
    }
    #endregion
}
