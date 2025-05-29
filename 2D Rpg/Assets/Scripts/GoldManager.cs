using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
  AudioSource audioSource;
  public AudioClip getGoldClip;

  public static GoldManager instance;   // �̱��� ����

  public float currentGold;

  GameObject player;    // �÷��̾� ��ü
  PlayerController pc;  // �÷��̾� ��Ʈ�ѷ�

  public static event Action<float> GoldChange; // �� ���� �̺�Ʈ

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
      yield return null; // ���� �����ӱ��� ���
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
    currentGold = 0f;
    GoldChange?.Invoke(currentGold);
  }

  // �� ��� �Լ�
  public void UseGold(int cost)
  {
    audioSource.PlayOneShot(getGoldClip, 0.5f);
    currentGold -= cost;
    GoldChange?.Invoke(currentGold);
  }

  // �� ȹ�� �Լ�
  public void GetGold(int addGold)
  {
    audioSource.PlayOneShot(getGoldClip, 0.5f);
    currentGold += addGold + (addGold * (pc.goldAcq * 0.01f));
    GoldChange?.Invoke(currentGold);
  }
}
