using UnityEngine;

public class MovingTarget : EnemyController
{
  private ChaseEvent chaseEvent;

  [Header("도망 방향 조정")]
  public float randomAngleRange = 30f; // 도망 방향에 줄 랜덤 각도 범위 (±도 단위)

  protected override void Start()
  {
    base.Start();
    chaseEvent = FindObjectOfType<ChaseEvent>();
  }

  protected override void Update()
  {
    if (player == null || isDead) return;

    Vector3 dir = (transform.position - player.transform.position).normalized;

    // 랜덤 방향 살짝 섞기
    dir = ApplyRandomOffset(dir);

    transform.position += dir * moveSpeed * Time.deltaTime;

    if (dir.x >= 0)
      transform.localScale = new Vector3(1, 1, 1);
    else
      transform.localScale = new Vector3(-1, 1, 1);
  }

  // 랜덤 오프셋을 적용하는 함수
  private Vector3 ApplyRandomOffset(Vector3 originalDir)
  {
    float randomAngle = Random.Range(-randomAngleRange, randomAngleRange); // -30도 ~ +30도 사이 랜덤
    float rad = randomAngle * Mathf.Deg2Rad; // 라디안 변환

    float cos = Mathf.Cos(rad);
    float sin = Mathf.Sin(rad);

    Vector3 rotatedDir = new Vector3(
      originalDir.x * cos - originalDir.y * sin,
      originalDir.x * sin + originalDir.y * cos,
      0f
    );

    return rotatedDir.normalized; // 방향은 다시 정규화
  }

  protected override void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))
    {
      BulletController bc = collision.GetComponent<BulletController>();
      if (bc == null || bc.hasHit) return;

      bc.hasHit = true;
      Destroy(collision.gameObject);

      float damage = pc.attackDem * bc.damageCoefficient;
      HitEnemy(damage);
    }
  }

  public override void Dead()
  {
    base.Dead();

    if (chaseEvent != null)
    {
      chaseEvent.OnCatch();
    }
  }
}