using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Ghost : EnemyController
{
  protected override void Awake()
  {
    range = 80.0f; // 탐색 범위
    speed = 2.3f; // 이동속도
    att = 8;     // 공격력
    def = 0;     // 방어력
    hp = 25;      // 체력
    exp = 20;     // 경험치
    gold = 10;     // 돈
    defIgn = 20;   // 방어력 무시

    spawnWeight = 13f;

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
