using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Attack : EnemyAttackController
{
  protected override void Start()
  {
    att = 7;     // 공격력
    defIgn = 5;   // 방어력 무시
  }
}
