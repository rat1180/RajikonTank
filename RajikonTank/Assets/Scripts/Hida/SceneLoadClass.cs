using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConstList;

public class SceneLoadClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(SceneNames name)
    {
        SceneManager.LoadScene(name.ToString());
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
