using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeObj : EnemyAttack
{
  private void Start()
  {
    attDamage = 10f;
    DefIgnore = 20f;

    Destroy(gameObject, 0.6f);
  }

}
