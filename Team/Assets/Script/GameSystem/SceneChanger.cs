using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName01;      // 씬01
    public GameObject UI;
    
    // 각 씬을 로드하는 함수
    public void NextScene01()
    {
        SceneManager.LoadScene(SceneName01);
    }

    public void Exit()
    {
        UI.SetActive(false);
    }

}
