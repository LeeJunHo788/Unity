using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : EnemyController
{
  protected override void Awake()
  {
    range = 35.0f; // 탐색 범위
    speed = 2.2f; // 이동속도
    att = 8;     // 공격력
    def = 30;     // 방어력
    hp = 20;      // 체력
    exp = 15;     // 경험치
    gold = 5;     // 돈
    defIgn = 5;   // 방어력 무시

    isFlyingEnemy = true;
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
