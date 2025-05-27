using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunman_Attack : EnemyAttackController
{
  protected override void Start()
  {
    att = 10f;
    defIgn = 15;
  }

}
