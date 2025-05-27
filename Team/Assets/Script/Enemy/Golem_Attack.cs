using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem_Attack : EnemyAttack
{
  

  private void Start()
  {
    attDamage = 5f;
    DefIgnore = 10f;
    lifeTime = 5f;
    Destroy(gameObject, lifeTime);

  }

  public override void SetDirection(Vector2 dir)
  {
    SetRotation(dir);
    direction = dir.normalized;
    Destroy(gameObject, lifeTime);
  }

  protected override void Update()
  {
    transform.position += (Vector3)(direction * speed * Time.deltaTime);
    // transform.Translate(direction * speed * Time.deltaTime);
  }

}
