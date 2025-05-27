using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDamage : EnemyAttack
{
  public float damage = 10f;
  public float duration = 5f;
  private float timer = 0f;

  private new void Awake()
  {
    speed = 5f;
    lifeTime = 3f;
    attDamage = 5f;       // 데미지
    DefIgnore = 0f;       // 방어력무시
    destroyOnHit = false;     // 피격 시 오브젝트를 없앨지 여부
  }

  private new void Update()
  {
    //이펙트이기 때문에base.Update는 넣지않음
    timer += Time.deltaTime;

    if (timer >= duration)
    {
      Destroy(gameObject);
    }

  }
}
