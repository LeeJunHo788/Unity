using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
  //씬 넘어가도 유지는 하되 얘는 오디오 클립 모아놓는 스크립트임
  //호출은 각자 스크립트에서 해야함
  public static SFXManager instance;

  public AudioClip axeThrew;
  public AudioClip bombThrew;
  public AudioClip bone;
  public AudioClip gun;
  public AudioClip knifeThrew;
  public AudioClip flame;
  public AudioClip enemyKill;
  public AudioClip boss3Kill; //엘리트보스3, 최종보스 전용
  public AudioClip catKill; //엘리트보스1 전용
  public AudioClip buttonClicked;
  public AudioClip item; //아이템 휙득시 나는 소리
  public AudioClip uiApear; //해금창 나올때 

  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); //씬 넘어가도 유지 
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
