using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class CPUInput : PlayerInput
{
    //CPU���ϊ������ړ������̃L�[
    public KeyList moveveckey;

    public override KeyList KeyInput()
    {
        return moveveckey;
    }
}
