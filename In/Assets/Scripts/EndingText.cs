using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndingText : MonoBehaviour
{
  public TMP_Text endingText; 
  public float typingSpeed = 0.2f; // 글자 하나당 출력되는 속도

  private string fullText = "플레이 해주셔서 감사합니다.";

  void Start()
  {
    StartCoroutine(Ending());
  }

  IEnumerator Ending()
  {
    endingText.text = "";

    // 한글자씩 출력
    for (int i = 0; i < fullText.Length; i++)
    {
      endingText.text += fullText[i];
      yield return new WaitForSeconds(typingSpeed);
    }

    // 3초 대기 후 페이드 아웃 후 메인 씬으로 전환
    yield return new WaitForSeconds(3f);

    ScreenFader fader = FindObjectOfType<ScreenFader>();
    fader.FadeAndLoadScene("MainScene");


  }
}
