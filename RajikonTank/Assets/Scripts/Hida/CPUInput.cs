using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class CPUInput : PlayerInput
{
    //CPUが変換した移動方向のキー
    public KeyList moveveckey;

    public override KeyList KeyInput()
    {
        return moveveckey;
    }
}
