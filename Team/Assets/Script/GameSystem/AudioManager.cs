using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager instance; //씬 넘어가도 유지되도록

  public AudioSource bgmSource;

  //연결할 오디오 클립
  public AudioClip stageSelectBGM;
  public AudioClip stage1BGM;
  public AudioClip stage1BossBGM;
  public AudioClip infiniteModeBGM;

  //스폰매니저에 AudioManager.instance.UpdateBGMByWave(currentWave); 웨이브 넘버 넘겨받기


  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
      Debug.Log("오디오 매니저 생성됨");
    }
    else
    {
      Debug.Log("오디오 매니저 중복감지 > 삭제");
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
    SceneManager.sceneLoaded += OnSceneLoaded; //씬전환 시 자동으로 BGM바꿔주기 
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
      case "Title"://타이틀하고 StageSelect같은브금
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

  //Wave에 따라 BGM바꾸는 함수
  public void BossBGM(int currentWave)
  {
    if (SceneManager.GetActiveScene().name != "Stage1")
      return; //Stage1 일때만 작동하도록

    if(currentWave > 0 && currentWave % 5 == 0) //5의배수 웨이브일때
    {
      if(bgmSource.clip != stage1BossBGM)
      {
        bgmSource.clip = stage1BossBGM;
        bgmSource.Play(); //보스전용 브금 틀기
      }
    }
    else
    {
      if(bgmSource.clip != stage1BGM)
      {
        bgmSource.clip = stage1BGM;
        bgmSource.Play(); //아니면 일반 스테이지 브금
      }
    }
  }
}
