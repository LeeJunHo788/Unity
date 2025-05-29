using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndingText : MonoBehaviour
{
  public TMP_Text endingText;
  public float fadeSpeed = 1f; // 8: ���̵� ��/�ƿ� �ӵ�

  void Start()
  {
    StartCoroutine(Ending());
  }

  IEnumerator Ending()
  {
    endingText.text = "The End"; // ù �ؽ�Ʈ ����
    yield return StartCoroutine(FadeText(true)); 
    yield return new WaitForSeconds(3f); 
    yield return StartCoroutine(FadeText(false)); 

    yield return new WaitForSeconds(0.5f); // 21: ª�� ��

    endingText.text = "�÷��� ���ּż� �����մϴ�."; 
    yield return StartCoroutine(FadeText(true));
    yield return new WaitForSeconds(2f); 
    yield return StartCoroutine(FadeText(false)); 

    yield return new WaitForSeconds(2f); 

    // 28: ���� ������ ��ȯ
    ScreenFader fader = FindObjectOfType<ScreenFader>();
    if (fader != null)
    {
      DestroyAllPersistentObjects(); // ���� �����ϰ�
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
    // PersistentObject ��ũ��Ʈ�� ���� ��� ������Ʈ�� ã�Ƽ�
    PersistentObject[] persistentObjects = FindObjectsOfType<PersistentObject>();

    foreach (PersistentObject obj in persistentObjects)
    {
      Destroy(obj.gameObject); // �ش� ������Ʈ ��ü�� ����
    }
  }
}
