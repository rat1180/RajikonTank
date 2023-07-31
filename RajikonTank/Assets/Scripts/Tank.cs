using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class Tank : MonoBehaviour
{
    [SerializeField] PlayerInput PlayerInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput(PlayerInput.KeyInput());
    }

    public void MoveInput(KeyList inputkey)
    {
        Debug.Log(inputkey);
        Move(PlayerInput.KeyInput());
    }

    private void Move(KeyList keylist)
    {
        switch (keylist)
        {
            case KeyList.A:
                Debug.Log("A");
                break;
            case KeyList.B:
                Debug.Log("B");
                break;
            case KeyList.C:
                Debug.Log("C");
                break;
        }
    }
}
