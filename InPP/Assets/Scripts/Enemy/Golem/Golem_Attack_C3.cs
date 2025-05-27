using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack_C3 : EnemyAttackController
{
  protected override void Start()
  {
    att = 12;     // 공격력
    defIgn = 10;   // 방어력 무시
  }

}

