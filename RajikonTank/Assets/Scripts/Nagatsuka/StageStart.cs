using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConstList;

public class StageStart : MonoBehaviour
{
	Animation anim;
	public float alfa;
	private bool alfaFlg;

	private const float k_maxLength = 1f;
	private const string k_propName = "_MainTex";

	[SerializeField]
	private Vector2 m_offsetSpeed;

	private Material m_material;
	Image image;

	private void Start()
	{
		anim = GetComponent<Animation>();
		//image = GetComponent<Image>();
		if (GetComponent<Image>() is Image i)
		{
			m_material = i.material;
		}
		alfa = 255;
	}

	private void Update()
	{
		if (m_material)
		{
			// xとyの値が0 ～ 1でリピートするようにする
			var x = Mathf.Repeat(Time.time * m_offsetSpeed.x, k_maxLength);
			var y = Mathf.Repeat(Time.time * m_offsetSpeed.y, k_maxLength);
			var offset = new Vector2(x, y);
			m_material.SetTextureOffset(k_propName, offset);
		}
        if (alfaFlg)
        {
			if(alfa>0f)	alfa -= 1;
			//image.color = new Color(image.color.r, image.color.g, image.color.b, alfa);
		}
	}


		private void OnDestroy()
	{
		// ゲームをやめた後にマテリアルのOffsetを戻しておく
		if (m_material)
		{
			m_material.SetTextureOffset(k_propName, Vector2.zero);
		}
		
	}

	public void StartAnimation()
    {
		GameManager.instance.PlaySE(SE_ID.Start);
	}


	public void FinishAnimation()
    {
		GameManager.instance.NowGameState = GAMESTATUS.INGAME;
		//Debug.Log("非表示になたよ");
		//anim.Stop();
		//this.gameObject.transform.parent.gameObject.SetActive(false);
	}

	public void ChangeAlfa()
    {
		alfaFlg = true;
	}
}
