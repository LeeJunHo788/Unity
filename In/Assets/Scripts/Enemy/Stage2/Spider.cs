using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : EnemyController
{
  protected override void Awake()
  {
    range = 35.0f; // 탐색 범위
    speed = 3.3f; // 이동속도
    att = 7;     // 공격력
    def = 10;     // 방어력
    hp = 10;      // 체력
    exp = 15;     // 경험치
    gold = 5;     // 돈
    defIgn = 15;   // 방어력 무시
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
