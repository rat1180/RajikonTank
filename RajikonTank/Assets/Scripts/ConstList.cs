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

    public enum SE_ID
    {
        Start,
        Move,
        EnemyDeath,
        PlayerDeath,
        Shot
    }

    public enum TankPrefabNames
    {
        NONE,
        TANK_TEST,
        TANK_NORMAL_RED,
        TANK_NORMAL,
        TankBase,
        Enemy_Normal
    }

    /// <summary>
    /// �`�[��ID�ꗗ�񋓑�.
    /// </summary>
    public enum TeamID
    {
        player,
        player2,
        player3,
        player4,
        CPU
    }

    /// <summary>
    /// �Q�[���̌��݂̏�Ԃ�\���񋓑�
    /// </summary>
    public enum GAMESTATUS
    {
        NONE,         //�Q�[���V�[���O�A�������̓Z�b�g����Ă��Ȃ�
        READY,        //�Q�[���J�n�O
        INGAME,       //�Q�[����
        ENDGAME,
        ENDGAME_WIN,  //�Q�[������
        ENDGAME_LOSE, //�Q�[���s�k
        COUNT         //���̗񋓑̂̐�
    }

    //�I�u�W�F�N�g���Ɛ����p�t�H���_���w�肷�邱�Ƃ�
    //���̃t�H���_����v���t�@�u����������
    public static class FolderObjectFinder
    {
        //�����p�t�H���_�ւ̃p�X
        const string DefalutGenerateFolderName = "Prefabs/";

        /// <summary>
        /// �����p�t�H���_����I�u�W�F�N�g��T���A�Ԃ��B
        /// ����������Ȃ���΋�̃Q�[���I�u�W�F�N�g��Ԃ�
        /// �f�t�H���g�̐����t�H���_���w�肳��Ă���̂ŁA�f�t�H���g����̑��ΎQ�ƂŖ��O������
        /// </summary>
        /// <param name="objectname"></param>
        /// <returns></returns>
        public static GameObject GetResorceObject(string objectname)
        {
            var obj = (GameObject)Resources.Load(DefalutGenerateFolderName+objectname);

            Debug.Log(DefalutGenerateFolderName + objectname);

            if (obj != null) return obj;
            else
            {
                Debug.LogError("���\�[�X�t�@�C����\"" + objectname + "\"��������܂���ł���");
                //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
                return new GameObject();
            }
        }
    }

    //�e�𐶐�����֐�������static�N���X
    public static class BulletGenerateClass
    {
        //Bullet�p�����t�H���_�ւ̃p�X
        const string GenerateFolderName = "Bullets/";

        /// <summary>
        /// �e�𐶐�����
        /// �����Ftank          ���̒e�̎�����Ƃ��Đݒ肷��^���N
        ///       parentobject�@���������e�́A�e�ɂ������I�u�W�F�N�g
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
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + bulletname);

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

        /// <summary>
        /// �e���������������֐�
        /// ���������e�ɑ΂��ď������s�������ꍇ�͂��̊֐����g��
        /// </summary>
        /// <param name="parentobject"></param>
        /// <param name="buletname"></param>
        /// <returns></returns>
        public static GameObject BulletInstantiateOne(GameObject parentobject,string bulletname)
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + bulletname);

            //�e��e�I�u�W�F�N�g��Ő���
            var obj = Object.Instantiate(prefabobj, parentobject.transform.position, parentobject.transform.rotation, parentobject.transform);

            //�G���[�`�F�b�N
            if (obj == null)
            {
                Debug.LogError("�e�̐����Ɏ��s���܂���");
                return new GameObject();
            }
            return obj;
        }
    }

    public static class TankGenerateClass
    {
        //Tank�p�t�H���_�ւ̃p�X
        const string GenerateFolderName = "Tanks/";

        /// <summary>
        /// �^���N�𐶐�����
        /// �^���N�̎�ނ�TankPrefabNames�̒�����I��ň����ɓ���邱��
        /// </summary>
        /// <param name="tankname"></param>
        /// <returns></returns>
        public static GameObject TankInstantiate(TankPrefabNames tankname)
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.GetResorceObject(GenerateFolderName + tankname);

            //�^���N�𐶐�
            var obj = Object.Instantiate(prefabobj);

            //�^�[�Q�b�g�I�u�W�F�N�g�𐶐�
            //var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var target = new GameObject();
            target.name = "targetobject";
            obj.GetComponent<Rajikon>().SetTargetObject(target);

            if(obj == null)
            {
                Debug.LogError("�^���N�̐����Ɏ��s���܂���");
                return new GameObject();
            }

            return obj;
        }
    }
}