using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class PlayerInput : MonoBehaviour
{
    KeyList sendkey;  // ‰Ÿ‚³‚ê‚½ƒL[‚Ìî•ñ‚ğ‘—‚é•Ï”.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public KeyList KeyInput()
    { 
        if (Input.GetKey(KeyCode.A))
        {
            sendkey = KeyList.A;
        }
        if (Input.GetKey(KeyCode.B))
        {
            sendkey = KeyList.B;
        }
        if (Input.GetKey(KeyCode.C))
        {
            sendkey = KeyList.C;
        }
        if (Input.anyKeyDown == false)
        {
            sendkey = KeyList.NONE;
        }

        return sendkey;
    }
}
