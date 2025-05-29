using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndingText : MonoBehaviour
{
  public TMP_Text endingText;
  public float fadeSpeed = 1f; // 8: 페이드 인/아웃 속도

  void Start()
  {
    StartCoroutine(Ending());
  }

  IEnumerator Ending()
  {
    endingText.text = "The End"; // 첫 텍스트 설정
    yield return StartCoroutine(FadeText(true)); 
    yield return new WaitForSeconds(3f); 
    yield return StartCoroutine(FadeText(false)); 

    yield return new WaitForSeconds(0.5f); // 21: 짧은 텀

    endingText.text = "플레이 해주셔서 감사합니다."; 
    yield return StartCoroutine(FadeText(true));
    yield return new WaitForSeconds(2f); 
    yield return StartCoroutine(FadeText(false)); 

    yield return new WaitForSeconds(2f); 

    // 28: 메인 씬으로 전환
    ScreenFader fader = FindObjectOfType<ScreenFader>();
    if (fader != null)
    {
      DestroyAllPersistentObjects(); // 먼저 삭제하고
      fader.FadeAndLoadScene("MainScene");
    }
    else
    {
      SceneManager.LoadScene("MainScene");
    }
  }

  IEnumerator FadeText(bool fadeIn)
  {
    float alpha = fadeIn ? 0f : 1f; 
    float target = fadeIn ? 1f : 0f; 

    Color color = endingText.color;

    while (!Mathf.Approximately(alpha, target))
    {
      alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime * fadeSpeed); 
      endingText.color = new Color(color.r, color.g, color.b, alpha);
      yield return null;
    }
  }

  public void DestroyAllPersistentObjects()
  {
    // PersistentObject 스크립트를 가진 모든 오브젝트를 찾아서
    PersistentObject[] persistentObjects = FindObjectsOfType<PersistentObject>();

    foreach (PersistentObject obj in persistentObjects)
    {
      Destroy(obj.gameObject); // 해당 오브젝트 자체를 삭제
    }
  }
}
