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
        // �e�L�X�g�̎擾
        tutorialText = transform.GetChild(0).GetComponent<Text>();
        tutorialText.text = "�u<b>���W�^���N�I</b>�v�ւ悤�����I";
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Swiching());
    }

    /// <summary>
    /// �`���[�g���A���e�L�X�g�̐؂�ւ�
    /// </summary>
    IEnumerator Swiching()
    {
        yield return new WaitForSeconds(5);
        tutorialText.text = "<color=blue><b>���X�e�B�b�N</b></color>�ňړ����悤�I\n" + "�_�����߂�<color=blue><b>A�{�^��</b></color>�Œe�����āI";
    }
}
