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
    /// Enterを押したらステージ変更をコマンドを実行する
    /// </summary>
    void StageChangeCommand()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            
            //Debug.LogWarning("Enterを押したよ");
        }
    }

    public void PushStageChangeCommand()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        InputField inputField;
        inputField = this.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<InputField>();
        GameManager.instance.NowStage = int.Parse(inputField.text) - 2;//ChangeReadyModeで次に進むから-1,配列で0番目があるので-1(計-2)
        //Debug.LogWarning(inputField.text);
        GameManager.instance.AllEnemyDestroy();
        
    }
}
