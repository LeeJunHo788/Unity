using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Skeleton : EnemyController
{
  protected override void Awake()
  {
    range = 50.0f; // 탐색 범위
    speed = 2.7f; // 이동속도
    att = 7;     // 공격력
    def = 20;     // 방어력
    hp = 30;      // 체력
    exp = 15;     // 경험치
    gold = 5;     // 돈
    defIgn = 15;   // 방어력 무시
    spawnWeight = 10;
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
