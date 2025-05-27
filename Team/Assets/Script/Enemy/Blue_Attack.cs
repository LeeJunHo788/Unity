using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue_Attack : EnemyAttack
{
  private Vector2 moveDir;

  protected override void Awake()
  {
    base.Awake();

    speed = 5f;
    lifeTime = 3f;
    attDamage = 5f;       // 데미지
    DefIgnore = 0f;       // 방어력무시
    destroyOnHit = true;     // 피격 시 오브젝트를 없앨지 여부
  }

  public new void SetDirection(Vector2 dir)
  {
    moveDir = dir.normalized;
  }

  protected override void Update()
  {
    base.Update();

    // 히트박스 이동
    transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
  }

  /*
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      PlayerController player = collision.GetComponent<PlayerController>();
      if (player == null) return;

      float damage = attDamage - (attDamage * (player.defence - (player.defence * (DefIgnore * 0.01f))) * 0.01f);
      player.TakeDamage(Mathf.RoundToInt(damage));

      if (destroyOnHit) Destroy(gameObject);
    }
  }*/
}
