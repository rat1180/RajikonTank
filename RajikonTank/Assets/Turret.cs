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
        // �J�����ƃ}�E�X�̈ʒu������Ray������.
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // �v���C���[�̍�����Plane���X�V���āA�J�����̏������ɒn�ʔ��肵�ċ������擾.
        Plane plane = new Plane(Vector3.up, transform.position);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            // ���������Ɍ�_���Z�o���āA��_�̕�������.
            var LookPoint = ray.GetPoint(distance);

            // �I�u�W�F�N�g�̏������Y���ɐݒ肵�ĉ�]���v�Z.
            Vector3 upDirection = Vector3.up;
            Quaternion targetRotation = Quaternion.LookRotation(LookPoint - transform.position, upDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }
    }
}
