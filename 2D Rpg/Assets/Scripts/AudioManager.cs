using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance; // �̱��� �ν��Ͻ�

  public AudioSource audioSource;    // BGM�� ����� AudioSource
  public AudioSource sfxSource; // ȿ������ ����� AudioSource



  // ���� �ش��ϴ� BGM ����� Ŭ��
  public AudioClip mainSceneBGM;
  public AudioClip stage1BGM;
  public AudioClip stage2BGM;
  public AudioClip stage3BGM;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
    }
    else
    {
      Destroy(gameObject);
      return;
    }

    // ���� �ٲ� ������ BGM �ٲٱ�
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // �� �̸��� ���� BGM �����ؼ� ����ϱ�
    switch (scene.name)
    {
      case "MainScene":
        PlayBGM(mainSceneBGM);
        break;
      case "Stage1":
        PlayBGM(stage1BGM); 
        break;
      case "Stage2":
        PlayBGM(stage2BGM); 
        break;
      case "Stage3":
        PlayBGM(stage3BGM); 
        break;
      default:
        audioSource.Stop(); 
        break;
    }
  }

  void PlayBGM(AudioClip clip)
  {
    if (audioSource.clip == clip) return; // ���� ��� ���̸� �н�

    // BGM���� ���� �ٸ��� ����
    switch (clip.name)
    {
      case "MainScene_Bgm":
        audioSource.volume = 1.5f; 
        break;
      case "Stage1_Bgm":
        audioSource.volume = 0.3f;
        break;
      case "Stage2_Bgm":
        audioSource.volume = 0.3f;
        break;
      case "Stage3_Bgm":
        audioSource.volume = 0.3f; 
        break;
      default:
        audioSource.volume = 0.5f; // �⺻��
        break;
    }

    audioSource.clip = clip;
    audioSource.Play();
  }

  public void PlaySFX(AudioClip clip)
  {
    if (clip != null && sfxSource != null && sfxSource.enabled && sfxSource.gameObject.activeInHierarchy)
    {
      sfxSource.PlayOneShot(clip);
    }
  }
}