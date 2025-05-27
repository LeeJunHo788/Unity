using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack_C2 : EnemyAttackController
{
  protected override void Start()
  {
    att = 10;     // 공격력
    defIgn = 20;   // 방어력 무시
  }

}

