using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamageStage3 : EnemyAttack
{
  public float duration = 1f;
  private float timer = 0f;

  protected override void Awake()
  {
    base.Awake();
    speed = 0f;
    lifeTime = 3f;
    attDamage = 10f;
    DefIgnore = 0f;
    destroyOnHit = false; // 관통형
  }

  private void Start()
  {
    // 사각형 콜라이더가 부착되어 있어야 함
    BoxCollider2D box = GetComponent<BoxCollider2D>();
    if (box != null) box.enabled = true;
  }

  protected override void Update()
  {
    timer += Time.deltaTime;
    if (timer >= duration)
    {
      Destroy(gameObject);
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      PlayerController player = collision.GetComponent<PlayerController>();
      if (player == null) return;

      float damage = attDamage;
      player.TakeDamage(Mathf.RoundToInt(damage), DefIgnore);

      if (destroyOnHit) Destroy(gameObject);
    }
  }
}
