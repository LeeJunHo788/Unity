using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Haloghost : EnemyController
{
  protected override void Awake()
  {
    range = 80.0f; // 탐색 범위
    speed = 2.8f; // 이동속도
    att = 13;     // 공격력
    def = 0;     // 방어력
    hp = 30;      // 체력
    exp = 20;     // 경험치
    gold = 10;     // 돈
    defIgn = 20;   // 방어력 무시

    spawnWeight = 7f;
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
