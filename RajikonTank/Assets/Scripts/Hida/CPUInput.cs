using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class CPUInput : PlayerInput
{
    //CPU���ϊ������ړ������̃L�[
    public KeyList moveveckey;


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        
    }

    public override KeyList KeyInput()
    {
        return moveveckey;
    }

    /// <summary>
    /// �^���N���_�����W
    /// </summary>
    /// <returns></returns>
    public override Vector3 TargetPosition()
    {
        return sendtarget;
    }
}
