using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConstList
{
    public enum KeyList
    {
        NONE,
        A,
        D,
        S,
        W,
        UPARROW,
        RIGHTARROW,
        LEFTARROW,
        DOWNARROW,
        SPACE,
    }

    //�e�𐶐�����֐�������static�N���X
    public static class BulletGenerateClass
    {
        //�����p�t�H���_�ւ̃p�X
        const string GenerateFolderName = "";

        /// <summary>
        /// �����p�t�H���_����I�u�W�F�N�g��T���A�Ԃ��B
        /// ����������Ȃ���΋�̃Q�[���I�u�W�F�N�g��Ԃ�
        /// </summary>
        /// <param name="objectname"></param>
        /// <returns></returns>
        public static GameObject GetResorceObject(string objectname)
        {
            var obj = (GameObject)Resources.Load(objectname);

            if (obj != null) return obj;
            else
            {
                Debug.LogError("���\�[�X�t�@�C����\"" + objectname + "\"��������܂���ł���");
                //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
                return new GameObject();
            }
        }

        /// <summary>
        /// �e�𐶐�����
        /// �����Fparentobject�@���������e�́A�e�ɂ������I�u�W�F�N�g
        /// �@�@�@bulletname�@�@��������e�̖��O
        /// �@�@�@bulletnm�@�@�@�e�̐�
        /// �߂�l�F�e������ɐ���������true���Ԃ�
        /// </summary>
        /// <param name="parentobject"></param>
        /// <param name="bulletname"></param>
        /// <param name="bulletnm"></param>
        /// <returns></returns>
        public static bool BulletInstantiate(GameObject tank,GameObject parentobject,string bulletname,int bulletnm)
        {
            //�����Ώۂ�T��
            var prefabobj = GetResorceObject(GenerateFolderName + bulletname);

            //�������܂ŌJ��Ԃ�
            for (int i = 0; i < bulletnm; i++)
            {
                //�e��e�I�u�W�F�N�g��Ő���
                var obj = Object.Instantiate(prefabobj, parentobject.transform.position, parentobject.transform.rotation,parentobject.transform);

                //�G���[�`�F�b�N
                if (obj == null)
                {
                    Debug.LogError("�e�̐����Ɏ��s���܂���");
                    return false;
                }

                //�e�Ƀ^���N���Z�b�g
                //obj.GetComponent<Bullet>().SetTank(tank);
            }
            return true;
        }

    }
}