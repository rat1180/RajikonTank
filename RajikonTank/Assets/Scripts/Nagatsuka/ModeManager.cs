using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ConstList;
using Photon.Pun;
using Photon.Realtime;

public class ModeManager : MonoBehaviour
{
    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    /// <summary>
    /// 開始関数：このモード内のゲームを開始する。
    /// ゲームマネージャーはこの関数を呼んだ時に「開始した」と判断するので、
    /// この関数を基準にスタートする
    /// </summary>
    public void StartGame()
    {
        //Playerを操作可能にする.

    }

    /// <summary>
    /// 終了関数：ゲームが終了したときにゲームマネージャーにその旨を送る。
    /// この関数によって呼ばれたときに「終了した」と判断する
    /// </summary>
    public void FinishGame()
    {

    }

    /// <summary>
    /// セットアップ関数：ゲーム開始前に、ステージやキャラ等を生成する。
    /// ゲームマネジャーによって開始前に呼ばれ、この関数の結果によって開始関数を呼ぶ直前まで進む
    /// </summary>
    public void Setup()
    {
        //Playerの生成.

        //CPUの生成.
    }

    /// <summary>
    /// セーブ関数：もろもろのデータをゲームマネジャーに保存する。終了関数後に呼ばれる
    /// </summary>
    public void Save()
    {

    }
}
