using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class PlayerInput : MonoBehaviour
{
    KeyCode KeyInfo;  // 押されたキーの情報.
    KeyList sendkey;  // 押されたキーの情報を送る変数.

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public KeyList KeyInput()
    {
        KeyInfo = KeyCode.None;

        if (Input.GetKey(KeyCode.A))
        {
            KeyInfo = KeyCode.A;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            KeyInfo = KeyCode.D;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            KeyInfo = KeyCode.S;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            KeyInfo = KeyCode.W;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            KeyInfo = KeyCode.UpArrow;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            KeyInfo = KeyCode.RightArrow;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            KeyInfo = KeyCode.LeftArrow;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            KeyInfo = KeyCode.DownArrow;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            KeyInfo = KeyCode.Space;
        }
        else
        {
            sendkey = KeyList.NONE;
        }

        switch (KeyInfo)
        {
            case KeyCode.A:          sendkey = KeyList.A;          break;
            case KeyCode.D:          sendkey = KeyList.D;          break;
            case KeyCode.S:          sendkey = KeyList.S;          break;
            case KeyCode.W:          sendkey = KeyList.W;          break;
            case KeyCode.UpArrow:    sendkey = KeyList.UPARROW;    break;
            case KeyCode.RightArrow: sendkey = KeyList.RIGHTARROW; break;
            case KeyCode.LeftArrow:  sendkey = KeyList.LEFTARROW;  break;
            case KeyCode.DownArrow:  sendkey = KeyList.DOWNARROW;  break;
            case KeyCode.Space:      sendkey = KeyList.SPACE;      break;
        }

        return sendkey;

    }
}
