using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSwitching : MonoBehaviour
{
    Text tutorialText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tutorialText = transform.GetChild(0).GetComponent<Text>();

        Swiching();
    }

    private void Swiching()
    {
        tutorialText.text = "���X�e�B�b�N�ňړ�\n" +
            "ZR�Ŏˌ�";
    }
}
