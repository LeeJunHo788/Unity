using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringer_Attack1 : EnemyAttackController
{
  protected override void Start()
  {
    att = 15;     // 공격력
    defIgn = 30;   // 방어력 무시
  }

}
