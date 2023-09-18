using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class PlayerInput : MonoBehaviour
{
    KeyList sendkey;  // âüÇ≥ÇÍÇΩÉLÅ[ÇÃèÓïÒÇëóÇÈïœêî.

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public KeyList KeyInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            sendkey = KeyList.A;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            sendkey = KeyList.D;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            sendkey = KeyList.S;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            sendkey = KeyList.W;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            sendkey = KeyList.UPARROW;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            sendkey = KeyList.RIGHTARROW;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            sendkey = KeyList.LEFTARROW;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            sendkey = KeyList.DOWNARROW;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            sendkey = KeyList.SPACE;
        }
        else
        {
            sendkey = KeyList.NONE;
        }

        return sendkey;

    }
}
