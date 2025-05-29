using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
  public Image fadeImage; // 검은 이미지 (전체 화면)
  public float fadeDuration = 1.75f;

  void Awake()
  {
    // 이미 존재하는 ScreenFader가 있다면 자기 자신 제거
    if (FindObjectsOfType<ScreenFader>().Length > 1)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환되어도 유지!
    SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 후 호출될 메서드 등록
  }


  public void FadeAndLoadScene(string sceneName)
  {
    StartCoroutine(FadeOutAndLoad(sceneName));
  }

  IEnumerator FadeOutAndLoad(string sceneName)
  {
    float timer = 0f;

    // 점점 어둡게 (alpha 증가)
    while (timer < fadeDuration)
    {
      timer += Time.deltaTime;
      float alpha = Mathf.Clamp01(timer / fadeDuration);
      fadeImage.color = new Color(0, 0, 0, alpha);
      yield return null;
    }

    // 씬 전환
    SceneManager.LoadScene(sceneName);
  }

  // 씬 로드 후 호출되는 콜백
  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // 페이드 이미지 다시 투명하게 초기화
    fadeImage.color = new Color(0, 0, 0, 0);
  }

  void OnDestroy()
  {
    // 이벤트 중복 등록 방지
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }
}
