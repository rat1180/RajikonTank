using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

public class CPUInput : PlayerInput
{
    //CPUが変換した移動方向のキー
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
    /// タンクが狙う座標
    /// </summary>
    /// <returns></returns>
    public override Vector3 TargetPosition()
    {
        return sendtarget;
    }
}
