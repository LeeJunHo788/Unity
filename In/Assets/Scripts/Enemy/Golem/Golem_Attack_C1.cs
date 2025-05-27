using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack_C1 : EnemyAttackController
{
  protected override void Start()
  {
    att = 15;     // 공격력
    defIgn = 20;   // 방어력 무시
  }

}

