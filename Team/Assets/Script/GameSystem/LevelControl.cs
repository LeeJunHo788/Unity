using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
  public bool isWeather = false;
  public GameObject WeatherSystem;
  public LevelSystem Ls;
  public Text text;

  public GameObject WeatherOnObj;
  public GameObject WeatherOffObj;
  public GameObject LevelCanvas;
  public GameObject Weather;

  //효과음
  public AudioSource audioSource;


  // Start is called before the first frame update
  void Start()
  {
    Ls = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
    audioSource = GetComponent<AudioSource>(); //오디오 컴포넌트 가져오기
    Time.timeScale = 0;
  }

  // Update is called once per frame
  void Update()
  {
    if (isWeather)
    {
      WeatherSystem.SetActive(true);
    }
    else
    {
      WeatherSystem.SetActive(false);
    }
  }

  public void chooseLevel01()
  {
    Ls.L_State = LevelSystem.LevelState.Normal;
    text.text = "기본 난이도 선택됨";
    PlayClickSound();
  }

  public void chooseLevel02()
  {
    Ls.L_State = LevelSystem.LevelState.Hard;
    text.text = "어려움 난이도 선택됨";
    PlayClickSound();
  }

  public void chooseLevel03()
  {
    Ls.L_State = LevelSystem.LevelState.VeryHard;
    text.text = "매우 어려움 난이도 선택됨";
    PlayClickSound();
  }

  public void weatherOn()
  {
    isWeather = true;
    text.text = "추가강화요소가 켜졌습니다.";
    PlayClickSound();

    WeatherOffObj.SetActive(true);
    WeatherOnObj.SetActive(false);
    Weather.SetActive(true);
  }

  public void weatherOff()
  {
    isWeather = false;
    text.text = "추가강화요소가 꺼졌습니다.";
    PlayClickSound();

    WeatherOffObj.SetActive(false);
    WeatherOnObj.SetActive(true);
    Weather.SetActive(false);
  }

  public void Exit()
  {
    PlayClickSound();
    Time.timeScale = 1.0f;
    LevelCanvas.SetActive(false);
  }

  void PlayClickSound()
  {
    if (audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.buttonClicked);
    }
    else
    {
      Debug.LogWarning("효과음 재생 실패");
    }
  }
}
