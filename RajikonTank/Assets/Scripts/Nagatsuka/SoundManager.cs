using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class SoundManager : MonoBehaviour
{
    [Header("SE���X�g")]
    [SerializeField] List<AudioClip> SEList = new List<AudioClip>();

    [SerializeField] List<AudioClip> BGMList = new List<AudioClip>();

    AudioSource audioSource;

    #region Unity�C�x���g(Start)
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #region SE��炷

    /// <summary>
    /// ID���Q�Ƃ���SE��炷�֐�
    /// ��OverLoad(AudioClip)�L.
    /// </summary>
    /// <param name="id"></param>
    public void PlaySE(SE_ID id)
    {
        audioSource.PlayOneShot(SEList[(int)id]);
    }
    /// <summary>
    /// AudioClip�����������ɂ���SE��炷�֐�
    /// ��OverLoad(SE_ID)�L.
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySE(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
    #endregion

    /// <summary>
    /// ID�������ɊO����SE�𑗂�֐�
    /// �^���N��Move���Ɏg�p.
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
