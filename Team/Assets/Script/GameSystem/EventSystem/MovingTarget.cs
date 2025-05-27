using UnityEngine;

public class MovingTarget : EnemyController
{
  private ChaseEvent chaseEvent;

  [Header("���� ���� ����")]
  public float randomAngleRange = 30f; // ���� ���⿡ �� ���� ���� ���� (���� ����)

  protected override void Start()
  {
    base.Start();
    chaseEvent = FindObjectOfType<ChaseEvent>();
  }

  protected override void Update()
  {
    if (player == null || isDead) return;

    Vector3 dir = (transform.position - player.transform.position).normalized;

    // ���� ���� ��¦ ����
    dir = ApplyRandomOffset(dir);

    transform.position += dir * moveSpeed * Time.deltaTime;

    if (dir.x >= 0)
      transform.localScale = new Vector3(1, 1, 1);
    else
      transform.localScale = new Vector3(-1, 1, 1);
  }

  // ���� �������� �����ϴ� �Լ�
  private Vector3 ApplyRandomOffset(Vector3 originalDir)
  {
    float randomAngle = Random.Range(-randomAngleRange, randomAngleRange); // -30�� ~ +30�� ���� ����
    float rad = randomAngle * Mathf.Deg2Rad; // ���� ��ȯ

    float cos = Mathf.Cos(rad);
    float sin = Mathf.Sin(rad);

    Vector3 rotatedDir = new Vector3(
      originalDir.x * cos - originalDir.y * sin,
      originalDir.x * sin + originalDir.y * cos,
      0f
    );

    return rotatedDir.normalized; // ������ �ٽ� ����ȭ
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