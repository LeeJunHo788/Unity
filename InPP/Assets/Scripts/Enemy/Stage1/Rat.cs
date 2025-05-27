using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Rat : EnemyController
{
  protected override void Awake()
  {
    range = 70f; // 탐색 범위
    speed = 2.5f; // 이동속도
    att = 3;     // 공격력
    def = 0;     // 방어력
    hp = 10;      // 체력
    exp = 10;     // 경험치
    gold = 3;     // 돈
    defIgn = 5;   // 방어력 무시
  }

  protected override void Start()
  {
    currentHp = hp;
    base.Start();


  }

  protected override void Update()
  {
    base.Update();
  }
}
