using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorConversion : MonoBehaviour // ベクトル変換
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
        // オブジェクトAとBの位置を取得
        Vector3 PosA = ObjectA.position;
        Vector3 PosB = ObjectB.position;

        // 内積を求め、角度に変換
        float VectorX = PosB.x - PosA.x;
        float VectorZ = PosB.z - PosA.z;
        float Radian = Mathf.Atan2(VectorZ, VectorX) * Mathf.Rad2Deg;

        //角度表示変更
        if (Radian < 0)
        {
            // -180～180で返るため、0～360に変換
            Radian += 360;
        }

        // 360度を8分割し、四捨五入する(0～8)
        int  Division = Mathf.RoundToInt(Radian / 45.0f);

        // 8の場合0に変換(9等分防止)
        if (Division == 8) Division = 0;

        // Vector2に変換して取得
        Vector2 direction = Conversion(Division);

        // Vector2変換後の結果表示
        Debug.Log("角度： " + direction);
    }

    // 8方向に分割した値をVector2(-1～1, -1～1)に変換
    Vector2 Conversion(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(0, 1);   // 0度
            case 1: return new Vector2(1, 1);   // 45度
            case 2: return new Vector2(1, 0);   // 90度
            case 3: return new Vector2(1, -1);  // 135度
            case 4: return new Vector2(0, -1);  // 180度
            case 5: return new Vector2(-1, -1); // 225度
            case 6: return new Vector2(-1, 0);  // 270度
            case 7: return new Vector2(-1, 1);  // 315度
            default: return Vector2.zero;
        }
    }
}