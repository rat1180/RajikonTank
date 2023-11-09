using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ConstList;

public class ResorceManager : MonoBehaviour
{
    //���̃}�l�[�W���[�̃C���X�^���X
    public static ResorceManager Instance;

    //���[�h�����I�u�W�F�N�g�̃��X�g(���̂������Ȃ�)
    Dictionary<BulletPrefabNames, GameObject> Bullets;
    Dictionary<TankPrefabNames, GameObject> Tanks;
    Dictionary<SE_ID, AudioClip> SEs;
    Dictionary<EffectNames, GameObject> Effects;
    Dictionary<OtherPrefabNames, UnityEngine.Object> Others;

    //Bullet�p�����t�H���_�ւ̃p�X
    const string BulletGenerateFolderName = "Bullets/";
    //Tank�p�t�H���_�ւ̃p�X
    const string TankGenerateFolderName = "Tanks/";
    //SE�p�t�H���_�ւ̃p�X
    const string SEGenerateFolderName = "Sounds/AlengeSEandBGM/";
    //�G�t�F�N�g�p�t�H���_�ւ̃p�X
    const string EffectObjectFolderName = "Effects/";
    //���̑��p�t�H���_�ւ̃p�X
    const string OtherGenerateFolderName = "Prefabs/Others/";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

            LoadResorceObjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �K�v�ȃ��\�[�X�����[�h����
    /// ���[�h���郊�\�[�X��ConstList�ɓo�^����Ă��郊�X�g����Q��
    /// ���[�h�����ނ��ꂼ�ꃊ�X�g��p�ӂ���
    /// </summary>
    public void LoadResorceObjects()
    {
        LoadBullets();
        LoadTanks();
        LoadSEs();
        LoadEffects();
        LoadOthers();
    }

    #region ���[�h�֐�

    void LoadBullets()
    {
        //������
        Bullets = new Dictionary<BulletPrefabNames, GameObject>();

        //�e�̖��O���J��Ԃ�
        foreach(BulletPrefabNames name in Enum.GetValues(typeof(BulletPrefabNames)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceGameObject(BulletGenerateFolderName + name.ToString());

            Bullets.Add(name,prefabobj);
        }
    }

    void LoadTanks()
    {
        //������
        Tanks = new Dictionary<TankPrefabNames, GameObject>();

        //�e�̖��O���J��Ԃ�
        foreach (TankPrefabNames name in Enum.GetValues(typeof(TankPrefabNames)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceGameObject(TankGenerateFolderName + name.ToString());

            Tanks.Add(name, prefabobj);
        }
    }

    void LoadSEs()
    {
        //������
        SEs = new Dictionary<SE_ID, AudioClip>();

        //�e�̖��O���J��Ԃ�
        foreach (SE_ID name in Enum.GetValues(typeof(SE_ID)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceObject(SEGenerateFolderName + name.ToString());

            SEs.Add(name, (AudioClip)prefabobj);
        }
    }

    void LoadEffects()
    {
        //������
        Effects = new Dictionary<EffectNames, GameObject>();

        //�e�̖��O���J��Ԃ�
        foreach (EffectNames name in Enum.GetValues(typeof(EffectNames)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceGameObject(EffectObjectFolderName + name.ToString());

            Effects.Add(name, prefabobj);
        }
    }

    void LoadOthers()
    {
        //������
        Others = new Dictionary<OtherPrefabNames, UnityEngine.Object>();

        //�e�̖��O���J��Ԃ�
        foreach (OtherPrefabNames name in Enum.GetValues(typeof(OtherPrefabNames)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceObject(OtherGenerateFolderName + name.ToString());

            Others.Add(name, prefabobj);
        }
    }

    #endregion

    #region �Q�b�g�֐�

    public GameObject GetBulletResorce(BulletPrefabNames name)
    {
        if (Bullets.ContainsKey(name))
        {
            return Bullets[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽBulletPrefabName�����X�g�ɂ���܂���");
            return new GameObject();
        }
    }

    public GameObject GetTankResorce(TankPrefabNames name)
    {
        if (Tanks.ContainsKey(name))
        {
            return Tanks[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽTankPrefabName�����X�g�ɂ���܂���");
            return new GameObject();
        }
    }

    public AudioClip GetSEResorce(SE_ID id)
    {
        if (SEs.ContainsKey(id))
        {
            return SEs[id];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽSE_ID�����X�g�ɂ���܂���");
            return null;
        }
    }

    public GameObject GetEffectResorce(EffectNames name)
    {
        if (Effects.ContainsKey(name))
        {
            return Effects[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽEffectName�����X�g�ɂ���܂���");
            return new GameObject();
        }
    }

    public UnityEngine.Object GetOtherResorce(OtherPrefabNames name)
    {
        if (Others.ContainsKey(name))
        {
            return Others[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽOtherPrefabName�����X�g�ɂ���܂���");
            return new UnityEngine.Object();
        }
    }

    #endregion

}
