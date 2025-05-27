using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndingText : MonoBehaviour
{
  public TMP_Text endingText; 
  public float typingSpeed = 0.2f; // ���� �ϳ��� ��µǴ� �ӵ�

  private string fullText = "�÷��� ���ּż� �����մϴ�.";

  void Start()
  {
    StartCoroutine(Ending());
  }

  IEnumerator Ending()
  {
    endingText.text = "";

    // �ѱ��ھ� ���
    for (int i = 0; i < fullText.Length; i++)
    {
      endingText.text += fullText[i];
      yield return new WaitForSeconds(typingSpeed);
    }

    // 3�� ��� �� ���̵� �ƿ� �� ���� ������ ��ȯ
    yield return new WaitForSeconds(3f);

    ScreenFader fader = FindObjectOfType<ScreenFader>();
    fader.FadeAndLoadScene("MainScene");


  }
}
