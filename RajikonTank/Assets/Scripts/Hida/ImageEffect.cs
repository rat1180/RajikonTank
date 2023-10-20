using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageEffect : MonoBehaviour
{
    [SerializeField, Header("�����Ŏg�p����ϐ�"), Tooltip("�����l")] private float ClearlanceNm;
    [SerializeField, Tooltip("�A�j���[�V����������C���[�W�R���|�[�l���g")] private Material Image;
    [SerializeField, Tooltip("�t�F�[�h�̃X�s�[�h"), Range(0.001f, 0.01f)] public float FadeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Image = GetComponent<Image>().material;
        Image.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �ݒ肵���C���[�W�Ƀt�F�[�h�C�����A�E�g���s��
    /// fadetime�ɐݒ肵�����Ԃɍ����悤�ɃA�E�g�C�����s��
    /// isfadeout��true�ɂ���΃t�F�[�h�A�E�g�ɂȂ�
    /// </summary>
    /// <param name="fadetime"></param>
    /// <param name="isfadeout"></param>
    /// <returns></returns>
    public IEnumerator ImageFadeInAndOut(float fadetime,bool isfadeout)
    {
        float time = 0;
        int fadedirection = isfadeout ? -1 : 1;
        float end = isfadeout ? 0 : 1;
        ClearlanceNm = Image.color.a;

        while (true)
        {
            time += Time.deltaTime;

            ClearlanceNm += fadedirection * FadeSpeed;

            Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, ClearlanceNm);

            if (isfadeout)
            {
                if (ClearlanceNm <= end) yield break;
            }
            else
            {
                if (ClearlanceNm >= end) yield break;
            }

            yield return null;
        }
    }

    public void DefaultFadeInAndOut(bool isfadeout)
    {
        StartCoroutine(ImageFadeInAndOut(0,isfadeout));
    }
}
