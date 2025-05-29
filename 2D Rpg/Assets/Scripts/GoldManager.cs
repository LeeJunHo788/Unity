using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
  AudioSource audioSource;
  public AudioClip getGoldClip;

  public static GoldManager instance;   // 싱글톤 패턴

  public float currentGold;

  GameObject player;    // 플레이어 객체
  PlayerController pc;  // 플레이어 컨트롤러

  public static event Action<float> GoldChange; // 돈 변경 이벤트

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }


  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
    currentGold = 0f;
    GoldChange?.Invoke(currentGold);
  }

  // 돈 사용 함수
  public void UseGold(int cost)
  {
    audioSource.PlayOneShot(getGoldClip, 0.5f);
    currentGold -= cost;
    GoldChange?.Invoke(currentGold);
  }

  // 돈 획득 함수
  public void GetGold(int addGold)
  {
    audioSource.PlayOneShot(getGoldClip, 0.5f);
    currentGold += addGold + (addGold * (pc.goldAcq * 0.01f));
    GoldChange?.Invoke(currentGold);
  }
}
