using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveMouse();
    }

    /// <summary>
    /// �}�E�X�̈ʒu�ɃI�u�W�F�N�g���ړ�.
    /// </summary>
    void MoveMouse()
    {
        Mouse mouse = Mouse.current;

        // �J�[�\���̈ʒu���擾.
        Vector3 MousePos = mouse.position.ReadValue();

        // �J�[�\���ʒu��Z���W��ύX ��0���ƃJ�����̃����Y�ɒ���t���Ă��銴���ɂȂ��肭���[���h���W�ɕϊ��ł��Ȃ���.
        MousePos.z = 10;

        Vector3 Target = Camera.main.ScreenToWorldPoint(MousePos);

        transform.position = Target;
    }
}
