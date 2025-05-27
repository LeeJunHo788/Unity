using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageEnter : MonoBehaviour
{
    SceneChanger scene;
    public GameObject EnterUI;
    public GameObject EnterStage;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("�浹");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("�÷��̾���");
            EnterUI.SetActive(true);
            EnterStage.SetActive(true);
        }
    }

  public void StartStage()
  {
    if(EnterStage.name == "Stage02" && !GameManager.Instance.clearedStage1)
    {
      Debug.Log("���� ���Ѹ�尡 �رݵ��� �ʾҽ��ϴ�.");
      return;
    }

    if(EnterStage == null || string.IsNullOrEmpty(EnterStage.name))
    {
      Debug.LogError("EnterSTage�� null�Դϴ�");
      return;
    }

    //���� ������ �������� �̸��� ����
    PlayerPrefs.SetString("NextStage", EnterStage.name);
    SceneManager.LoadScene("LoadingScene");
  }
}
