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
        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        GamePadPushButton();
    }

    public void GamePadPushButton()
    {
        if (gamepad == null)
        {
            Debug.Log("null");
            return;
        }

        // ƒ}ƒ‹ƒ{ƒ^ƒ“‚ª‰Ÿ‚³‚ê‚½‚Æ‚«.
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
