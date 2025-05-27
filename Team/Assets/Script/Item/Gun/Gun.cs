using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{
  public LayerMask enemyLayer;    // 적 레이어
  public GameObject bulletPrefb;  // 총알 프리팹

  float range = 30f;              // 적 탐지 범위
  float attackSpeed = 5f;              // 공격 속도
  float attackTime;         // 공격 딜레이 시간
  float attackRange = 8f;        // 공격 범위
  Vector2 dir;                    // 공격 방향
  float angle;                    // 방향에 따른 각도
  


  bool isAttacking = false;       // 공격 상태

  GameObject player;      // 플레이어 오브젝트
  PlayerController pc;    // 플레이어 컨트롤러
  GameObject nearEnemy;       // 가장 가까운 적

  void Start()
  {
    player = GameObject.FindWithTag("Player");                          // 플레이어 찾기
    pc = player.GetComponent<PlayerController>();                       // 플레이어 컨트롤러 가져오기
    attackTime = 1 / (pc.attackSpeed * attackSpeed);   // 공격 속도 가져오기
    StartCoroutine(FindEnemyCoroutine());    // 일정 시간 마다 적 감지 실행

  }

  void Update()
  {
    transform.position = player.transform.position + (Vector3.right * 1.5f);    // 플레이어 우측으로 위치 고정

    // 현재 공격중이 아니라면 공격 실행
    if(!isAttacking)
    {
      StartCoroutine(Attack());
    }
  }


  // 적 탐지 메서드
  void FindEnemy()
  {
    
    float minDistance = Mathf.Infinity;   // 최소 거리
    
    nearEnemy = null;

    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);   // 플레이어 주변 탐지범위 내의 적 탐지

    foreach (var collider in hitColliders)
    {
      float dist = Vector2.Distance(transform.position, collider.transform.position);   // 거리 확인

      if (dist < minDistance)
      {
        minDistance = dist;       // 최소 거리 갱신
        nearEnemy = collider.gameObject;
      }
    }

    if (nearEnemy != null)
    {
      dir = (nearEnemy.transform.position - transform.position).normalized;       // 방향 설정
      angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;  
      transform.rotation = Quaternion.Euler(0, 0, angle);                         // 회전 설정

      if (dir.x < 0)    // 적이 왼쪽에 있는 경우
      {
        transform.localScale = new Vector3(1, -1, 1);     // 스프라이트 반전
      }

      else
      {
        transform.localScale = new Vector3(1, 1, 1);      // 스프라이트 정상화
      }
    }
  }

  // 일정시간 동안 탐지 메서드 반복
  IEnumerator FindEnemyCoroutine()
  {
    while(true)
    {
      FindEnemy();
      yield return new WaitForSeconds(0.05f);
    }
  }
  
  // 공격 실행
  IEnumerator Attack()
  {
    isAttacking = true;

    // 적이 없으면 공격 중지
    if (nearEnemy == null)
    {
      isAttacking = false;
      yield break; 
    }

    float dist = Vector2.Distance(transform.position, nearEnemy.transform.position);      // 가장 가까운 적과의 거리

    // 공격 범위 안에 있는 동안 공격 반복
    while (dist <= attackRange)
    {
      if (nearEnemy == null) break; //타겟이 삭제되었거나 사라졌을 경우 루프 탈출

      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

      GameObject bullet = Instantiate(bulletPrefb, transform.position, Quaternion.Euler(0, 0, angle));    // 총알 생성
      bullet.GetComponent<BulletController>().SetDirection(dir);    // 방향 넘겨주기

      yield return new WaitForSeconds(attackTime);       // 공격 시간만큼 대기

      if (nearEnemy == null) break; //타겟이 삭제되었거나 사라졌을 경우 루프 탈출
      dist = Vector2.Distance(transform.position, nearEnemy.transform.position);    // 거리 다시 재기

      Destroy(bullet, 10f);     // 10초 후 제거

    }

    isAttacking = false; // 공격 끝나면 상태 해제
  }

}
