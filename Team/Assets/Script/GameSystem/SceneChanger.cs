using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
  public string SceneName01 = "LoadingScene"; //����� �⺻ �ε���
  public GameObject UI;
  public string NextStageKey = "Stage01"; //�б� ���� ���ڿ� �Է�

  //ȿ����
  private AudioSource audioSource;

  private void Start()
  {
    audioSource = GetComponent<AudioSource>(); //������ҽ� ��������
  }

  // �� ���� �ε��ϴ� �Լ�
  public void NextScene01()
  {
    StartCoroutine(NextSceneCoroutine());
  }

  public void Exit()
  {
    UI.SetActive(false);
  }

  private IEnumerator NextSceneCoroutine()
  {
    PlayClickSound();

    //1�� ���� ��ٸ���
    yield return new WaitForSeconds(1f);

    //���⵵ ������
    PlayerPrefs.SetString("NextStage", NextStageKey);
    PlayerPrefs.Save();

    //�ε������� �̵�
    SceneManager.LoadScene(SceneName01);
  }
  private void PlayClickSound()
  {
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.buttonClicked);//ȿ�������
    }
  }
}
