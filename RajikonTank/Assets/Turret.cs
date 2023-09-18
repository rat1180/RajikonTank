using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] float RotateSpeed;

    void FixedUpdate()
    {
        LookMouse();
    }

    void LookMouse()
    {
        // �J�����̎��_����}�E�X�̌��݂̈ʒu�Ɍ�����Ray���쐬.
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // �v���C���[�̍�����PlaneObj���쐬���āA�J�����̏������ɒn�ʔ��肵�ċ������擾.
        Plane plane = new Plane(Vector3.up, transform.position);
        
        // Ray�̌�_�������i�[���邽�߂̕ϐ�.
        float distance;

        // plane��Ray�ƌ�������ꍇ.
        if (plane.Raycast(ray, out distance))
        {
            // ���������Ɍ�_���Z�o���āA��_�̕�������.
            var LookPoint = ray.GetPoint(distance);

            // �I�u�W�F�N�g�̏������Y���ɐݒ肵�ĉ�]���鏈��.
            Vector3 UpDirection = Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(LookPoint - transform.position, UpDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }
    }
}
