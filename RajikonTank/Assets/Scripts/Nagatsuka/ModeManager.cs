using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;

public class ModeManager : MonoBehaviour
{
    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    /// <summary>
    /// �J�n�֐��F���̃��[�h���̃Q�[�����J�n����B
    /// �Q�[���}�l�[�W���[�͂��̊֐����Ă񂾎��Ɂu�J�n�����v�Ɣ��f����̂ŁA
    /// ���̊֐�����ɃX�^�[�g����
    /// </summary>
    public void StartGame()
    {
        //Player�𑀍�\�ɂ���.

    }

    /// <summary>
    /// �I���֐��F�Q�[�����I�������Ƃ��ɃQ�[���}�l�[�W���[�ɂ��̎|�𑗂�B
    /// ���̊֐��ɂ���ČĂ΂ꂽ�Ƃ��Ɂu�I�������v�Ɣ��f����
    /// </summary>
    public void FinishGame()
    {

    }

    /// <summary>
    /// �Z�b�g�A�b�v�֐��F�Q�[���J�n�O�ɁA�X�e�[�W��L�������𐶐�����B
    /// �Q�[���}�l�W���[�ɂ���ĊJ�n�O�ɌĂ΂�A���̊֐��̌��ʂɂ���ĊJ�n�֐����ĂԒ��O�܂Ői��
    /// </summary>
    public void Setup()
    {
        //Player�̐���.

        //CPU�̐���.
    }

    /// <summary>
    /// �Z�[�u�֐��F�������̃f�[�^���Q�[���}�l�W���[�ɕۑ�����B�I���֐���ɌĂ΂��
    /// </summary>
    public void Save()
    {

    }
}
