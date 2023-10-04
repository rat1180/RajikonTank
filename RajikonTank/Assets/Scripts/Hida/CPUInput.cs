using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class CPUInput : PlayerInput
{
    //CPU‚ª•ÏŠ·‚µ‚½ˆÚ“®•ûŒü‚ÌƒL[
    public KeyList moveveckey;

    public override KeyList KeyInput()
    {
        return moveveckey;
    }
}
