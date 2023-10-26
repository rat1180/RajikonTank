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
        // テキストの取得
        tutorialText = transform.GetChild(0).GetComponent<Text>();
        tutorialText.text = "「<b>ラジタンク！</b>」へようこそ！";
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Swiching());
    }

    /// <summary>
    /// チュートリアルテキストの切り替え
    /// </summary>
    IEnumerator Swiching()
    {
        yield return new WaitForSeconds(5);
        tutorialText.text = "<color=blue><b>左スティック</b></color>で移動しよう！\n" + "狙いを定めて<color=blue><b>Aボタン</b></color>で弾を撃て！";
    }
}
