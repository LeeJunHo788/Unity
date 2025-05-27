using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
  public string SceneName;
  public TextMeshProUGUI text;

  private AudioSource audioSource; //ȿ������ ���� 
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
        audioSource.PlayOneShot(SFXManager.instance.uiAppear); //ȿ�������
      }
      else
      {
        Debug.Log("ȿ���� �������! �����ϳ��� null�Դϴ�.");
      }
      PlayerPrefs.SetString("NextScene", SceneName);  // StageSelect ����
      StartCoroutine(LoadSceneWithDelay(1f)); //����ȯ ������
    }

  }

  IEnumerator LoadSceneWithDelay(float delay)
  {
    yield return new WaitForSeconds(delay); // ��� ��ٸ�
    SceneManager.LoadScene("LoadingScene");
  }

  public IEnumerator FadeTextToFull() // ���İ� 0���� 1�� ��ȯ
  {
    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    while (text.color.a < 1.0f)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.02f);
      yield return new WaitForSeconds(0.01f);
    }
    StartCoroutine(FadeTextToZero());
  }

  public IEnumerator FadeTextToZero()  // ���İ� 1���� 0���� ��ȯ
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
