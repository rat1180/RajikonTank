using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSwitching : MonoBehaviour
{
    Text tutorialText;

    /// <summary>
    /// チュートリアルテキストの切り替え
    /// </summary>
    IEnumerator Swiching()
    {
        yield return new WaitForSeconds(4);
        tutorialText.text = "<color=blue><b>左スティック</b></color>で移動しよう！\n" + "狙いを定めて<color=blue><b>R2ボタン</b></color>で弾を撃て！";
    }

    private void OnEnable()
    {
        // テキストの取得
        tutorialText = transform.GetChild(0).GetComponent<Text>();
        tutorialText.text = "「<b>ラジタンク！</b>」へようこそ！";
        StartCoroutine(Swiching());
    }
}
