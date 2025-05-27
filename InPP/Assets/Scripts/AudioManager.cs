using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance; // 싱글톤 인스턴스

  public AudioSource audioSource;    // BGM을 재생할 AudioSource
  public AudioSource sfxSource; // 효과음을 재생할 AudioSource



  // 씬에 해당하는 BGM 오디오 클립
  public AudioClip mainSceneBGM;
  public AudioClip stage1BGM;
  public AudioClip stage2BGM;
  public AudioClip stage3BGM;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    }
    else
    {
      Destroy(gameObject);
      return;
    }

    // 씬이 바뀔 때마다 BGM 바꾸기
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // 씬 이름에 따라 BGM 선택해서 재생하기
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
    if (audioSource.clip == clip) return; // 현재 재생 중이면 패스

    // BGM마다 볼륨 다르게 설정
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
        audioSource.volume = 0.5f; // 기본값
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