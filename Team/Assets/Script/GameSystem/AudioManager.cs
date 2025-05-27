using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager instance; //�� �Ѿ�� �����ǵ���

  public AudioSource bgmSource;

  //������ ����� Ŭ��
  public AudioClip stageSelectBGM;
  public AudioClip stage1BGM;
  public AudioClip stage1BossBGM;
  public AudioClip infiniteModeBGM;

  //�����Ŵ����� AudioManager.instance.UpdateBGMByWave(currentWave); ���̺� �ѹ� �Ѱܹޱ�


  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
      Debug.Log("����� �Ŵ��� ������");
    }
    else
    {
      Debug.Log("����� �Ŵ��� �ߺ����� > ����");
      Destroy(gameObject);
      return;
    }
  }
  private void Start()
  {
    PlayBGM(SceneManager.GetActiveScene().name);
  }

  private void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded; //����ȯ �� �ڵ����� BGM�ٲ��ֱ� 
  }

  private void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    PlayBGM(scene.name);
  }

  private void PlayBGM(string sceneName)
  {
    AudioClip nextClip = null;

    switch(sceneName)
    {
      case "Title"://Ÿ��Ʋ�ϰ� StageSelect�������
      case "StageSelect":
        nextClip = stageSelectBGM;
        break;
      case "Stage1":
        nextClip = stage1BGM;
        break;
      case "Stage2":
        nextClip = infiniteModeBGM;
        break;
      default:
        nextClip = null;
        break;
    }

    if(nextClip != null && bgmSource.clip != nextClip)
    {
      bgmSource.clip = nextClip;
      bgmSource.Play();
    }
  }

  //Wave�� ���� BGM�ٲٴ� �Լ�
  public void BossBGM(int currentWave)
  {
    if (SceneManager.GetActiveScene().name != "Stage1")
      return; //Stage1 �϶��� �۵��ϵ���

    if(currentWave > 0 && currentWave % 5 == 0) //5�ǹ�� ���̺��϶�
    {
      if(bgmSource.clip != stage1BossBGM)
      {
        bgmSource.clip = stage1BossBGM;
        bgmSource.Play(); //�������� ��� Ʋ��
      }
    }
    else
    {
      if(bgmSource.clip != stage1BGM)
      {
        bgmSource.clip = stage1BGM;
        bgmSource.Play(); //�ƴϸ� �Ϲ� �������� ���
      }
    }
  }
}
