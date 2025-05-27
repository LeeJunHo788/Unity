using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
  public string SceneName;
  public TextMeshProUGUI text;

  private AudioSource audioSource; //효과음용 변수 
  private void Start()
  {
    audioSource = GetComponent<AudioSource>();
    text = GameObject.Find("Press").GetComponent<TextMeshProUGUI>();
    StartCoroutine(FadeTextToFull());
  }

  void Update()
  {
    if (Input.anyKeyDown)
    {
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.uiAppear != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.uiAppear); //효과음재생
      }
      else
      {
        Debug.Log("효과음 재생실패! 셋중하나가 null입니다.");
      }
      PlayerPrefs.SetString("NextScene", SceneName);  // StageSelect 저장
      StartCoroutine(LoadSceneWithDelay(1f)); //씬전환 딜레이
    }

  }

  IEnumerator LoadSceneWithDelay(float delay)
  {
    yield return new WaitForSeconds(delay); // 잠깐 기다림
    SceneManager.LoadScene("LoadingScene");
  }

  public IEnumerator FadeTextToFull() // 알파값 0에서 1로 전환
  {
    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    while (text.color.a < 1.0f)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.02f);
      yield return new WaitForSeconds(0.01f);
    }
    StartCoroutine(FadeTextToZero());
  }

  public IEnumerator FadeTextToZero()  // 알파값 1에서 0으로 전환
  {
    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    while (text.color.a > 0.0f)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.02f);
      yield return new WaitForSeconds(0.01f);
    }
    StartCoroutine(FadeTextToFull());
  }
}
