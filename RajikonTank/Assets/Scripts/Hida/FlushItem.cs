using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlushItem : MonoBehaviour
{
    public bool IsImage;  //点滅させるこのコンポーネントタイプ true:Image,false:text
    int DirectionFlush;  //点滅方向
    float ClearLance;  //透明度
    Coroutine FlushCoroutine;
    [SerializeField] float FlushInterval;

    // Start is called before the first frame update
    void Start()
    {
        DirectionFlush = -1;
        ClearLance = 1.0f;
        FlushInterval = FlushInterval == 0 ? 0.1f : FlushInterval;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        if (FlushCoroutine == null)
        {
            FlushCoroutine = StartCoroutine(Flush());
        }
    }

    void OnDisable()
    {
        if (FlushCoroutine != null)
        {
            StopCoroutine(Flush());
            FlushCoroutine = null;
        }
    }

    IEnumerator Flush()
    {
        Image ImageScript;
        Text TextScript;
        Color DefaltColor;
        while (true)
        {
            if (IsImage)
            {
                ImageScript = GetComponent<Image>();
                DefaltColor = ImageScript.color;
                if (ClearLance > 1 || ClearLance < 0)
                {
                    DirectionFlush *= -1;
                }
                ClearLance += 0.1f * DirectionFlush;

                ImageScript.color = new Color(DefaltColor.r, DefaltColor.g, DefaltColor.b, ClearLance);
                yield return new WaitForSeconds(FlushInterval);
            }
            else
            {
                TextScript = GetComponent<Text>();
                DefaltColor = TextScript.color;
                if (ClearLance > 1 || ClearLance < 0)
                {
                    DirectionFlush *= -1;
                }
                ClearLance += 0.1f * DirectionFlush;

                TextScript.color = new Color(DefaltColor.r, DefaltColor.g, DefaltColor.b, ClearLance);
                yield return new WaitForSeconds(FlushInterval);

            }
        }
        yield break;
    }
}
