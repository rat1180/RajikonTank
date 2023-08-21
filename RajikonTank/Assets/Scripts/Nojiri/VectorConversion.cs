using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorConversion : MonoBehaviour // �x�N�g���ϊ�
{
    [SerializeField] public Transform ObjectA;
    [SerializeField] public Transform ObjectB;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // �I�u�W�F�N�gA��B�̈ʒu���擾
        Vector3 PosA = ObjectA.position;
        Vector3 PosB = ObjectB.position;

        // ���ς����߁A�p�x�ɕϊ�
        float VectorX = PosB.x - PosA.x;
        float VectorZ = PosB.z - PosA.z;
        float Radian = Mathf.Atan2(VectorZ, VectorX) * Mathf.Rad2Deg;

        //�p�x�\���ύX
        if (Radian < 0)
        {
            // -180�`180�ŕԂ邽�߁A0�`360�ɕϊ�
            Radian += 360;
        }

        // 360�x��8�������A�l�̌ܓ�����(0�`8)
        int  Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8�̏ꍇ0�ɕϊ�(9�����h�~)
        if (Division == 8) Division = 0;

        // Vector2�ɕϊ����Ď擾
        Vector2 direction = Conversion(Division);

        // Vector2�ϊ���̌��ʕ\��
        Debug.Log("�p�x�F " + direction);
    }

    // 8�����ɕ��������l��Vector2(-1�`1, -1�`1)�ɕϊ�
    Vector2 Conversion(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(0, 1);   // 0�x
            case 1: return new Vector2(1, 1);   // 45�x
            case 2: return new Vector2(1, 0);   // 90�x
            case 3: return new Vector2(1, -1);  // 135�x
            case 4: return new Vector2(0, -1);  // 180�x
            case 5: return new Vector2(-1, -1); // 225�x
            case 6: return new Vector2(-1, 0);  // 270�x
            case 7: return new Vector2(-1, 1);  // 315�x
            default: return Vector2.zero;
        }
    }
}