using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : EnemyAttack
{
  public float damage = 10f;
  public float duration = 0.5f;

  //터질때에 맞춰 콜라이더가 점점 커지게 하는 변수
  private CircleCollider2D circle;
  public float maxRadius = 1.5f;
  private float timer = 0f;

  //크기 조절여부(Stage1은 크기조절O Elite3은 크기조절X)
  public bool useExpansion = true;

  protected override void Awake()
  {
    base.Awake();

    speed = 5f;
    lifeTime = 3f;
    attDamage = 5f;       // 데미지
    DefIgnore = 0f;       // 방어력무시
    destroyOnHit = false;     // 피격 시 오브젝트를 없앨지 여부
  }

  private void Start()
  {
    circle = GetComponent<CircleCollider2D>();
    if (useExpansion)
    {
      circle.radius = 0f; //시작시 0
    }
  }
  private new void Update()
  {
    //이펙트이기 때문에base.Update는 넣지않음
    timer += Time.deltaTime;

    if (useExpansion)
    {
      //시간흐름에 따라 커지도록
      circle.radius = Mathf.Lerp(0, maxRadius, timer / duration);
    }

      if (timer >= duration)
      {
        Destroy(gameObject);
      }
    
  }
  /*
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.CompareTag("Player")) //충돌이 플레이어라면
    {
      PlayerController player = collision.GetComponent<PlayerController>();
      if (player == null) return;

      float damage = attDamage - (attDamage * (player.defence - (player.defence * (DefIgnore * 0.01f))) * 0.01f);
      player.TakeDamage(Mathf.RoundToInt(damage));

      if (destroyOnHit) Destroy(gameObject);
    }
  }*/
}
