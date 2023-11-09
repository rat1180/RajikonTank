using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const int MAXBULLET = 5;
    bool CreateFlg;
    [SerializeField]GameObject[] Bullets;

    // Start is called before the first frame update
    void Start()
    {
        CreateBullet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    bool CreateBullet()
    {
        for (int i = 0; i < MAXBULLET; i++)
        {
            Bullets[i] = (GameObject)Resources.Load("Prefabs/Bullets/Real3ReflectBullet");
            //BulletRb[patrolPoint] = Bullets[patrolPoint].transform.GetComponent<Rigidbody>();

            Instantiate(Bullets[i], new Vector3(0.0f, 0.0f, 1.0f + i), Quaternion.identity, this.gameObject.transform);

            if (Bullets.Length == MAXBULLET)
            {
                CreateFlg = true;              
            }
            else
            {
                CreateFlg = false;
            }
        }
        return CreateFlg;
    }
    
}
