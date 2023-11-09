using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomIns : MonoBehaviour
{
    GameObject Boms;
    // Start is called before the first frame update
    void Start()
    {
        Boms = (GameObject)Resources.Load("Prefabs/Bom");
        Instantiate(Boms, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, this.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("au");
            
        }
    }
}
