using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{

    [Header("�Q�[�����")]
    public GAMESTATUS NowGameState;//���݂̃Q�[�����.

    #region Unity�C�x���g(Awake�EStart�EUpdate)

    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        //NowGameState = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        switch (NowGameState)//�Q�[�����[�h�ɂ���ď����𕪊򂷂�.
        {
            case GAMESTATUS.READY:
                ReadyRoop();
                break;
            case GAMESTATUS.INGAME:
                InGameRoop();
                break;
            default:
                Debug.Log("�G���[:�\�����ʃQ�[�����[�h");
                break;
        }
    }
    #endregion

    /// <summary>
    /// Ready�̎��ɓ������֐�.
    /// </summary>
    private void ReadyRoop()
    {

    }

    /// <summary>
    /// InGame�̎��ɓ������֐�.
    /// </summary>
    private void InGameRoop()
    {

    }
}
