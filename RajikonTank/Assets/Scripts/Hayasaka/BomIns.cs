using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomIns : MonoBehaviour
{
    GameObject Boms;
    public bool InsFlg = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // スペースキーで生成
        if (Input.GetKey(KeyCode.Space))
        {
            if (!InsFlg)
            {
                InsFlg = true;
                Debug.Log("au");
                Boms = (GameObject)Resources.Load("Prefabs/Bom");
                Instantiate(Boms);
            }
        }
    }
}
