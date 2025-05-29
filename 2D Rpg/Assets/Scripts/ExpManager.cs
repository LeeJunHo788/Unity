using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
  public static ExpManager instance;   // 싱글톤 패턴

  public float maxExp;        // 최대 경험치
  public float currentExp;    // 현재 경험치

  GameObject player;    // 플레이어 객체
  PlayerController pc;  // 플레이어 컨트롤러

  public static event Action<float, float, int> ExpChange; // 경험치 변경 이벤트
  public static event Action<int> LevelUp; // 레벨업 이벤트

  void Awake()
  {
    if(instance == null)
    {
      instance = this;
    }
  }

  void Start()
  {
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

    // 초기 경험치 설정
    maxExp = Mathf.RoundToInt(40 * Mathf.Pow(1.1f, pc.level - 1));     // 1렙 maxExp = 100;
    currentExp = 0;

    ExpChange?.Invoke(maxExp, currentExp, pc.level);  // 경험치 바 업데이트
  }

  public void AddExp(int exp)
  {
    currentExp += exp + (exp * pc.expAcq);

    if(currentExp >= maxExp)
    {
      // 레벨업 이펙트 발생
      // 내용추가

      pc.level++;   // 플레이어 레벨 증가
      currentExp -= maxExp; // 초과 경험치 적용
      maxExp = Mathf.RoundToInt(40 * Mathf.Pow(1.1f, pc.level - 1));   // 최대 경험치 증가

      LevelUp?.Invoke(pc.level);

    }

    ExpChange?.Invoke(maxExp, currentExp, pc.level);

  }


}
