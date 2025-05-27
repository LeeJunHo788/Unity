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

  //ȿ����
  public AudioSource audioSource;


  // Start is called before the first frame update
  void Start()
  {
    Ls = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
    audioSource = GetComponent<AudioSource>(); //����� ������Ʈ ��������
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
    text.text = "�⺻ ���̵� ���õ�";
    PlayClickSound();
  }

  public void chooseLevel02()
  {
    Ls.L_State = LevelSystem.LevelState.Hard;
    text.text = "����� ���̵� ���õ�";
    PlayClickSound();
  }

  public void chooseLevel03()
  {
    Ls.L_State = LevelSystem.LevelState.VeryHard;
    text.text = "�ſ� ����� ���̵� ���õ�";
    PlayClickSound();
  }

  public void weatherOn()
  {
    isWeather = true;
    text.text = "�߰���ȭ��Ұ� �������ϴ�.";
    PlayClickSound();

    WeatherOffObj.SetActive(true);
    WeatherOnObj.SetActive(false);
    Weather.SetActive(true);
  }

  public void weatherOff()
  {
    isWeather = false;
    text.text = "�߰���ȭ��Ұ� �������ϴ�.";
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
      Debug.LogWarning("ȿ���� ��� ����");
    }
  }
}
