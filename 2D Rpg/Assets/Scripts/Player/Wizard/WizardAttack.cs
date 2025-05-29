using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAttack : MonoBehaviour
{
  GameObject player;
  PlayerController pc;
  Animator anim;

  public float speed = 12f;   // 속도
  public float lifeTime = 0.2f; // 수명
  float targetRange = 7f; // 탐색 범위

  float explodeRadius = 0.4f;        // 폭발 범위
  bool exploding = false;    

  private Vector2 direction; // 기본값 오른쪽으로 설정

  void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
    anim = GetComponent<Animator>();

    // 레벨에 따라 기본 공격 강화(5레벨, 10레벨)
    if(pc.level > 9)
    {
      explodeRadius *= 2f;
      speed *= 1.4f;
      gameObject.transform.localScale = new Vector3(2f, 2f);
    }

    else if(pc.level > 4)
    {
      explodeRadius *= 1.2f;
      speed *= 1.2f;
      gameObject.transform.localScale = new Vector3(1.5f, 1.5f);
    }

    // 적 감지 후 리스트 변환
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    List<GameObject> allTargets = new List<GameObject>(enemies);

    GameObject nearestEnemy = null;
    float shortestDistance = Mathf.Infinity;

    foreach (GameObject enemy in allTargets)
    {
      EnemyController ec = enemy.GetComponent<EnemyController>();
      if (ec == null || ec.currentHp <= 0) continue; // 체력이 0 이하인 적은 무시

      float distance = Vector2.Distance(transform.position, enemy.transform.position);
      Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;

      // 플레이어가 보는 방향과 일치하는지 확인
      if ((player.transform.rotation.y == 0 && dirToEnemy.x > 0) || (player.transform.rotation.y != 0 && dirToEnemy.x < 0))
      {
        if (distance < shortestDistance && distance <= targetRange)
        {
          shortestDistance = distance;
          nearestEnemy = enemy;
        }
      }
    }

    if (nearestEnemy != null)
    {
      // 적 방향으로 벡터 계산
      direction = (nearestEnemy.transform.position - transform.position).normalized;
    }
    else
    {
      // 타겟이 없으면 일직선 발사 
      if (player.transform.rotation.y == 0)
      {
        direction = Vector2.right;
      }

      else
      {
        direction = Vector2.left;
      }
    }

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // Z축 기준으로 회전

    Invoke("Explode", lifeTime);


  }

  void Update()
  {
    if (exploding)
      return;

    // 지정된 방향으로 이동
    transform.Translate(direction * (speed) * pc.attackSpeed * Time.deltaTime, Space.World);
  }



  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
    {
      bool isDead = false;

      EnemyController ec = other.GetComponent<EnemyController>();
      BossController bc = other.GetComponent<BossController>();

      if(ec != null)
      {
        if(ec.currentHp <= 0)
        {
          isDead = true;
        }
      }

      if(!isDead)
        Explode();
    }
  }

  void Explode()
  {
    if (exploding) return; //  중복 실행 방지
    exploding = true;

    anim.SetTrigger("Explode");
    Vector2 center = transform.position;

    HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    // 범위 탐색
    Collider2D[] hits = Physics2D.OverlapCircleAll(center, explodeRadius);

    foreach (Collider2D hit in hits)
    {

      GameObject target = hit.gameObject;

      // 이미 처리한 적이면 건너뜀
      if (damagedTargets.Contains(target))
        continue;


      float dem;
      if (pc.level > 9)
        dem = pc.att * 1.4f;
      else if (pc.level > 4)
        dem = pc.att * 1.2f;
      else
        dem = pc.att;

      if (hit.CompareTag("Enemy")) // 적 탐지
      {
        EnemyController ec = hit.GetComponent<EnemyController>();
        if (ec != null)
        {
          // 넉백 방향은 hit 위치 기준으로 계산
          Vector2 knockDir = hit.transform.position - transform.position;
          ec.StartCoroutine(ec.KnockBack(dem, knockDir));
          damagedTargets.Add(target); // 데미지 준 적 등록
        }
      }

      else if (hit.CompareTag("Boss")) // 적 탐지
      {
        BossController bc = hit.GetComponent<BossController>();
        if (bc != null)
        {
          // 넉백 방향은 hit 위치 기준으로 계산
          Vector2 knockDir = hit.transform.position - transform.position;
          bc.StartCoroutine(bc.KnockBack(dem, knockDir));
          damagedTargets.Add(target); // 데미지 준 보스 등록
        }
      }
    }

    Destroy(gameObject, 0.7f);
  }

  void OnDrawGizmos()
  {
    // (2) ▶ 기즈모 색상 설정 (빨간색 원)
    Gizmos.color = new Color(1f, 0f, 0f, 1f); // 반투명 빨간색

    // (3) ▶ 현재 오브젝트 위치를 중심으로 원 그리기
    Gizmos.DrawWireSphere(transform.position, explodeRadius); // 폭발 범위 원 그리기

  }
}
