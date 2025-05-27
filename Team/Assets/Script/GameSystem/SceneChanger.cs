using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
  public string SceneName01 = "LoadingScene"; //여기는 기본 로딩씬
  public GameObject UI;
  public string NextStageKey = "Stage01"; //분기 기준 문자열 입력

  //효과음
  private AudioSource audioSource;

  private void Start()
  {
    audioSource = GetComponent<AudioSource>(); //오디오소스 가져오기
  }

  // 각 씬을 로드하는 함수
  public void NextScene01()
  {
    StartCoroutine(NextSceneCoroutine());
  }

  public void Exit()
  {
    UI.SetActive(false);
  }

  private IEnumerator NextSceneCoroutine()
  {
    PlayClickSound();

    //1초 정도 기다리기
    yield return new WaitForSeconds(1f);

    //여기도 씬저장
    PlayerPrefs.SetString("NextStage", NextStageKey);
    PlayerPrefs.Save();

    //로딩씬으로 이동
    SceneManager.LoadScene(SceneName01);
  }
  private void PlayClickSound()
  {
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.buttonClicked);//효과음재생
    }
  }
}
