using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Bat : EnemyController
{
  protected override void Awake()
  {
    range = 70f; // 탐색 범위
    speed = 2.0f; // 이동속도
    att = 4;     // 공격력
    def = 5;     // 방어력
    hp = 5;      // 체력
    exp = 10;     // 경험치
    gold = 3;     // 돈
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
