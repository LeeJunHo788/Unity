using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack_B1 : EnemyAttackController
{
  protected override void Start()
  {
    att = 8;     // 공격력
    defIgn = 12;   // 방어력 무시
  }

}
