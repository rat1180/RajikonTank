using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstList;

namespace TankClassInfomations
{
    /// <summary>
    /// タンクで起こったイベントを受け取るクラス
    /// CPUやプレイヤーで継承して使用する。
    /// タンク側から何らかのイベント（ヒットなど）を通知したい場合は
    /// このクラスを経由する
    /// </summary>
    public class TankEventHandler : MonoBehaviour
    {
        /// <summary>
        /// タンクの初期化処理が行われた時に呼ばれる
        /// 行いたいイベントによって、内容をoverrideする
        /// </summary>
        public virtual void TankInitStarted()
        {
            Debug.Log("タンク初期化開始");
        }

        /// <summary>
        /// タンクに関係する初期化処理が完了したときに呼ばれる
        /// 行いたいイベントによって、内容をoverrideする
        /// </summary>
        public virtual void TankInitCompleated()
        {
            Debug.Log("タンク初期化完了");
        }

        /// <summary>
        /// タンクに何らかのエラーが起こった場合に呼ばれる
        /// 行いたいイベントによって、内容をoverrideする
        /// </summary>
        public virtual void TankError()
        {
            Debug.LogWarning("タンクエラー");
        }

        /// <summary>
        /// タンクに弾がヒットした場合、タンクが呼ぶ
        /// 行いたいイベントによって、内容をoverrideする
        /// </summary>
        public virtual void TankHit()
        {
            Debug.Log("タンクヒット");
        }
    }

}