using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack_A1 : EnemyAttackController
{
  protected override void Start()
  {
    att = 6;     // 공격력
    defIgn = 10;   // 방어력 무시
  }

}
