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
        Debug.Log("충돌");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어임");
            EnterUI.SetActive(true);
            EnterStage.SetActive(true);
        }
    }

  public void StartStage()
  {
    if(EnterStage.name == "Stage02" && !GameManager.Instance.clearedStage1)
    {
      Debug.Log("아직 무한모드가 해금되지 않았습니다.");
      return;
    }

    if(EnterStage == null || string.IsNullOrEmpty(EnterStage.name))
    {
      Debug.LogError("EnterSTage가 null입니다");
      return;
    }

    //현재 설정된 스테이지 이름을 저장
    PlayerPrefs.SetString("NextStage", EnterStage.name);
    SceneManager.LoadScene("LoadingScene");
  }
}
