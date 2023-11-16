using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StageChangeCommand();
    }

    /// <summary>
    /// Enter����������X�e�[�W�ύX���R�}���h�����s����
    /// </summary>
    void StageChangeCommand()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            
            //Debug.LogWarning("Enter����������");
        }
    }

    public void PushStageChangeCommand()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        InputField inputField;
        inputField = this.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<InputField>();
        GameManager.instance.NowStage = int.Parse(inputField.text) - 2;//ChangeReadyMode�Ŏ��ɐi�ނ���-1,�z���0�Ԗڂ�����̂�-1(�v-2)
        //Debug.LogWarning(inputField.text);
        GameManager.instance.AllEnemyDestroy();
        
    }
}
