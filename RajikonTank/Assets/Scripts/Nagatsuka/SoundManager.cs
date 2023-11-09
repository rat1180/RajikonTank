using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SoundManager : MonoBehaviour
{
    [Header("SEリスト")]
    [SerializeField] List<AudioClip> SEList = new List<AudioClip>();

    [SerializeField] List<AudioClip> BGMList = new List<AudioClip>();

    AudioSource audioSource;

    #region Unityイベント(Start)
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //SE・BGMの読み込み.
        SEList[(int)SE_ID.Start] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.Move] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.EnemyDeath] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.PlayerDeath] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.Reflect] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.BulletDestroy] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.Ready] = (AudioClip)Resources.Load("Sounds/SE/Start");
        SEList[(int)SE_ID.Clear] = (AudioClip)Resources.Load("Sounds/SE/Start");

    }
    #endregion

    #region SEを鳴らす

    /// <summary>
    /// IDを参照してSEを鳴らす関数
    /// ※OverLoad(AudioClip)有.
    /// </summary>
    /// <param name="id"></param>
    public void PlaySE(SE_ID id)
    {
        audioSource.PlayOneShot(SEList[(int)id]);
    }
    /// <summary>
    /// AudioClip音源を引数にしてSEを鳴らす関数
    /// ※OverLoad(SE_ID)有.
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySE(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
    #endregion

    /// <summary>
    /// IDを引数に外部にSEを送る関数
    /// タンクのMove等に使用.
    public AudioClip ReturnSE(SE_ID id)
    {
        return SEList[(int)id];
    }


    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlayBGM(BGM_ID id)
    {
        audioSource.clip = BGMList[(int)id];
        audioSource.Play();
    }
}
