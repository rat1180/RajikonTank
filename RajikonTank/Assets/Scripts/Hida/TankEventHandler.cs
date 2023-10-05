using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

namespace TankClassInfomations
{
    /// <summary>
    /// �^���N�ŋN�������C�x���g���󂯎��N���X
    /// CPU��v���C���[�Ōp�����Ďg�p����B
    /// �^���N�����牽�炩�̃C�x���g�i�q�b�g�Ȃǁj��ʒm�������ꍇ��
    /// ���̃N���X���o�R����
    /// </summary>
    public class TankEventHandler : MonoBehaviour
    {
        /// <summary>
        /// �^���N�̏������������s��ꂽ���ɌĂ΂��
        /// �s�������C�x���g�ɂ���āA���e��override����
        /// </summary>
        public virtual void TankInitStarted()
        {
            Debug.Log("�^���N�������J�n");
        }

        /// <summary>
        /// �^���N�Ɋ֌W���鏉�������������������Ƃ��ɌĂ΂��
        /// �s�������C�x���g�ɂ���āA���e��override����
        /// </summary>
        public virtual void TankInitCompleated()
        {
            Debug.Log("�^���N����������");
        }

        /// <summary>
        /// �^���N�ɉ��炩�̃G���[���N�������ꍇ�ɌĂ΂��
        /// �s�������C�x���g�ɂ���āA���e��override����
        /// </summary>
        public virtual void TankError()
        {
            Debug.LogWarning("�^���N�G���[");
        }

        /// <summary>
        /// �^���N�ɒe���q�b�g�����ꍇ�A�^���N���Ă�
        /// �s�������C�x���g�ɂ���āA���e��override����
        /// </summary>
        public virtual void TankHit()
        {
            Debug.Log("�^���N�q�b�g");
        }
    }

}