using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
  public Image fadeImage; // ���� �̹��� (��ü ȭ��)
  public float fadeDuration = 1.75f;

  void Awake()
  {
    // �̹� �����ϴ� ScreenFader�� �ִٸ� �ڱ� �ڽ� ����
    if (FindObjectsOfType<ScreenFader>().Length > 1)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ�Ǿ ����!
    SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �� ȣ��� �޼��� ���
  }


  public void FadeAndLoadScene(string sceneName)
  {
    StartCoroutine(FadeOutAndLoad(sceneName));
  }

  IEnumerator FadeOutAndLoad(string sceneName)
  {
    float timer = 0f;

    // ���� ��Ӱ� (alpha ����)
    while (timer < fadeDuration)
    {
      timer += Time.deltaTime;
      float alpha = Mathf.Clamp01(timer / fadeDuration);
      fadeImage.color = new Color(0, 0, 0, alpha);
      yield return null;
    }

    // �� ��ȯ
    SceneManager.LoadScene(sceneName);
  }

  // �� �ε� �� ȣ��Ǵ� �ݹ�
  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // ���̵� �̹��� �ٽ� �����ϰ� �ʱ�ȭ
    fadeImage.color = new Color(0, 0, 0, 0);
  }

  void OnDestroy()
  {
    // �̺�Ʈ �ߺ� ��� ����
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }
}
