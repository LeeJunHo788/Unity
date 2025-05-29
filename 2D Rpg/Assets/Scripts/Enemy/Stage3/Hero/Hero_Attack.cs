using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Attack : EnemyAttackController
{
  protected override void Start()
  {
    att = 5;     // 공격력
    defIgn = 10;   // 방어력 무시
  }

}
