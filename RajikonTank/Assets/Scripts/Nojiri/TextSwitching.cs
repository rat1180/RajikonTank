using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSwitching : MonoBehaviour
{
    Text tutorialText;

    /// <summary>
    /// �`���[�g���A���e�L�X�g�̐؂�ւ�
    /// </summary>
    IEnumerator Swiching()
    {
        yield return new WaitForSeconds(4);
        tutorialText.text = "<color=blue><b>���X�e�B�b�N</b></color>�ňړ����悤�I\n" + "�_�����߂�<color=blue><b>R2�{�^��</b></color>�Œe�����āI";
    }

    private void OnEnable()
    {
        // �e�L�X�g�̎擾
        tutorialText = transform.GetChild(0).GetComponent<Text>();
        tutorialText.text = "�u<b>���W�^���N�I</b>�v�ւ悤�����I";
        StartCoroutine(Swiching());
    }
}
