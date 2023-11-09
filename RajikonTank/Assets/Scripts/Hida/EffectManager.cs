using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ConstList;

/// <summary>
/// �G�t�F�N�g�̊Ǘ����s���N���X
/// �G�t�F�N�g�͂��̃N���X�ňꊇ�Ǘ�����A
/// �G�t�F�N�g�𔭐�������ꍇ�͂��̃N���X�ɗv������B
/// 
/// ���̃I�u�W�F�N�g�̎q�Ɋe�G�t�F�N�g�����[�����
/// </summary>
public class EffectManager : MonoBehaviour
{

    #region �ϐ�

    [SerializeField, Header("�Q�[���}�l�[�W���[�ւ̃C���X�^���X")] GameManager GameManagerInstance;
    [SerializeField, Header("�g�p����G�t�F�N�g�̃��X�g")] List<GameObject> EffectList = new List<GameObject>();
    [SerializeField, Header("�G�t�F�N�g���܂Ƃ߂Ă���q�I�u�W�F�N�g")] GameObject EffectParent;

    #endregion

    //�e�X�g
    public static EffectManager instance;

    #region Unity�C�x���g

    // Start is called before the first frame update
    void Start()
    {
        Init();

        //�e�X�g
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region �֐�

    #region �������֐�

    /// <summary>
    /// ���̃}�l�[�W���[�̃C���X�^���X���Q�[���}�l�[�W���[�ɃZ�b�g���A
    /// �e�G�t�F�N�g�̌������s���A���X�g�ɃZ�b�g����
    /// ���炩�̃^�C�~���O�ōēx�ݒ���s�������ꍇ�A�O�����炱�̊֐����Ă�
    /// </summary>
    public void Init()
    {
        //�Q�[���}�l�[�W���[�̃C���X�^���X���擾
        SetGameManagerInstance();

        //���g�̃G�t�F�N�g���X�g���N���A���A�Z�b�g����
        InitEffectList();
    }

    private void SetGameManagerInstance()
    {
        GameManagerInstance = GameManager.instance;
        if(GameManagerInstance == null)
        {
            Debug.LogWarning("GameManager��������܂���");
        }
    }

    /// <summary>
    /// �G�t�F�N�g���X�g�̃N���A�ƃZ�b�g���s��
    /// </summary>
    private void InitEffectList()
    {
        //�G�t�F�N�g�܂Ƃ߃I�u�W�F�N�g�̃N���A
        ClearEffects();

        //�G�t�F�N�g���X�g���N���A����
        EffectList.Clear();

        //�񋓑̖̂��O��Ńt�H���_�[��T�����A�v���t�@�u��Ԃ�
        foreach(var effectname in Enum.GetValues(typeof(EffectNames))){
            //EffectList.Add(FolderObjectFinder.GetResorceGameObject(EffectObjectFolderName + effectname));
            EffectList.Add(ResorceManager.Instance.GetEffectResorce((EffectNames)effectname));
        }
    }

    #endregion

    /// <summary>
    /// �G�t�F�N�g�����ׂăN���A����
    /// </summary>
    public void ClearEffects()
    {
        if(EffectParent != null)
        {
            Destroy(EffectParent);
        }
        EffectParent = new GameObject();
        EffectParent.name = "EffectParent";
        EffectParent.transform.parent = gameObject.transform;
        EffectParent.transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// �G�t�F�N�g���Đ�����
    /// </summary>
    /// <param name="effect"></param>
    public void PlayEffect(EffectNames effect,Vector3 pos)
    {
        var obj = Instantiate(EffectList[(int)effect]);
        obj.transform.parent = EffectParent.transform;
        obj.transform.position = pos;
    }

    #endregion
}
