using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFemale_Attack : EnemyAttack
{
  protected override void Awake()
  {
    base.Awake();

    speed = 7f;
    lifeTime = 3f;
    attDamage = 5f + sm.currentWave;       // 데미지
    DefIgnore = Mathf.Min(100f, 0f + sm.currentWave); // 최대 100으로 제한
    destroyOnHit = true;     // 피격 시 오브젝트를 없앨지 여부
  }
  protected override void Update()
  {
    base.Update();
  }
 
}
