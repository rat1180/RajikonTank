using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConstList;

public class SceneLoadClass : MonoBehaviour
{

    public bool isMove;
    // Start is called before the first frame update
    void Start()
    {
        isMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(SceneNames name)
    {
        if (isMove) return;
        SceneManager.LoadScene(name.ToString());
        isMove = true;
    }

    public void GoGameScene()
    {
        ChangeScene(SceneNames.GameScene);
    }
    public void GoTitleScene()
    {
        ChangeScene(SceneNames.Title);
    }
}
