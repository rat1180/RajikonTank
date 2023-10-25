using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class DOUGAYOU : MonoBehaviour
{
    public GameObject Tank;
    public GameObject Bullet;

    public GameObject Target;
    public bool isStart;
    public bool isStop;
    Keyboard keyboard = Keyboard.current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Target = Tank;
            }
        }
        else
        {
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                Target = Bullet;
            }
        }

        if (keyboard.enterKey.wasPressedThisFrame)
        {
            isStop = true;
        }

        if(Target != null && !isStop)
        {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 5, Target.transform.position.z);
        }
    }
}
