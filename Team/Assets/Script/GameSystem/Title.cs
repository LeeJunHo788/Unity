using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    public string SceneName;
    public TextMeshProUGUI text;
    // Update is called once per frame
    private void Start()
    {
        text = GameObject.Find("Press").GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeTextToFull());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
           PlayerPrefs.SetString("NextScene", SceneName);  // StageSelect ����
           SceneManager.LoadScene("LoadingScene");          // �ε������� ���� �̵�
        }

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
