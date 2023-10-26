using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePadInput : MonoBehaviour
{
    Gamepad gamepad = Gamepad.current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GamePadPushButton()
    {
        // �}���{�^���������ꂽ�Ƃ�.
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
