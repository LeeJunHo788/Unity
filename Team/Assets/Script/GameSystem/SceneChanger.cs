using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName01;      // ��01
    public GameObject UI;
    
    // �� ���� �ε��ϴ� �Լ�
    public void NextScene01()
    {
        SceneManager.LoadScene(SceneName01);
    }

    public void Exit()
    {
        UI.SetActive(false);
    }

}
