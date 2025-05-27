using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
  GameObject player;
  Rigidbody2D rb;

  public TextMeshProUGUI tipText;          // �� �ؽ�Ʈ
  public Slider loadingSlider;  // �ε���
  float fakeProgress = 0f;

  string[] tips = new string[]
  {
    "���͵� ���°� ���� ���ø� ������ �ֽ��ϴ�.",
    "�ִ� ��ġ�� �������ִ� ������ �ֽ��ϴ�.",
    "������ ������ ������ �����ϴ�.\n���������� = ���ݷ�-( ���ݷ� * ( ���� - ( ���� * ���� ����%))%",
    "�� ��򰡿� �ִ� �ڷ����͸� �������Ѿ� �մϴ�.",
    "�ڷ����Ͱ� �����Ǹ� �������� �ٽ� �� �� �����ϴ�.",
    "���� óġ�ϸ� �幮 Ȯ���� �������� �������ϴ�."

  };

  void Start()
  {
    ShowRandomTip(); // ������ �� �� ǥ��
    StartCoroutine(LoadNextScene());
  }

  void ShowRandomTip()
  {
    int index = Random.Range(0, tips.Length);
    tipText.text = tips[index];
  }

  IEnumerator LoadNextScene()
  {
    player = GameObject.FindWithTag("Player");
    if(player != null)
    {
      rb = player.GetComponent<Rigidbody2D>();
      rb.gravityScale = 0f;
    }

    // 1�� ��ٷȴٰ� �ε� ���� (�ִϸ��̼ǿ� ������)
    yield return new WaitForSeconds(0.5f);

    // �����տ��� ��������
    string nextSceneName = PlayerPrefs.GetString("NextScene");
    AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
    op.allowSceneActivation = false; // �� ��ȯ ��Ȱ��ȭ

    // ���� �ε��� 0 ~ 0.9������ ������
    while (fakeProgress < 0.9f)
    {
      fakeProgress += Time.deltaTime * 0.7f; // õõ�� ���� (�ӵ� ���� ����)
      loadingSlider.value = fakeProgress;
      yield return null;
    }

    // ���� �ε� �Ϸ�� ������ ���
    while (op.progress < 0.9f)
    {
      yield return null;
    }

    // 0.9 �Ѿ����� ������ 0.1 õõ�� �ø���
    while (fakeProgress < 1f)
    {
      fakeProgress += Time.deltaTime * 0.3f;
      loadingSlider.value = fakeProgress;
      yield return null;
    }

    // ��¦ ����ٰ� �� ��ȯ!
    yield return new WaitForSeconds(0.5f);
    op.allowSceneActivation = true;

    rb.gravityScale = 2f;


    /*
    // �ε� �Ϸ�� ������ ��ٸ���
    while (!op.isDone)
    {
      yield return null;
    }*/
  }
}
