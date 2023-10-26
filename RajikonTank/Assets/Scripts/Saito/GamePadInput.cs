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

        // マルボタンが押されたとき.
        if (gamepad.buttonEast.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame)
        {
            Debug.Log("丸ボタン");
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
