using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Golem : BossController
{
  public GameObject golem_Bullet;
  public GameObject defUpEffect;
  float attackRange = 20f;


  float buffCooldown = 10f;         // 디펜스 버프 쿨타임 추가
  float buffCooldownTimer = 0f;     // 버프 타이머


  protected override void Start()
  {
    base.Start();
    isAttacking = false;
  }

  protected override void Update()
  {
    if (currentHp <= 0) return;

    base.Update();    // 이동 관련 처리

    float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);

    // 버프 스킬 쿨타임 체크
    if (buffCooldownTimer <= 0f)
    {
      StartCoroutine(DefenseBuff());
      buffCooldownTimer = buffCooldown; // 쿨타임 초기화
    }
    else
    {
      buffCooldownTimer -= Time.deltaTime; // 쿨타임 감소
    }

    if (distanceToPlayer < attackRange)
    {
      if (!isAttacking)
      {
        StartCoroutine(Attack());
      }
    }
  }

  IEnumerator Attack()
  {
    if (isAttacking) yield break;
    isAttacking = true;

    animator.SetTrigger("Attack");

    yield return new WaitForSeconds(0.5f);

    Vector3 playerPos = player.transform.position;
    Vector3 enemyPos = transform.position;
    Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

    float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

    // 방향 반전 처리
    float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;

    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * -30f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * -10f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 0f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 10f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 30f));

    StartCoroutine(AttackCooldown());
  }

  // 총알 생성 메서드 
  void CreateBulletAtAngle(Vector3 spawnPos, float angle)
  {
    float rad = angle * Mathf.Deg2Rad;
    Vector3 shootDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

    GameObject bullet = Instantiate(golem_Bullet, spawnPos, Quaternion.identity);
    bullet.GetComponent<Golem_Attack>().SetDirection(shootDir);

  }

  IEnumerator AttackCooldown()
  {
    isAttacking = true;
    yield return new WaitForSeconds(2f);
    isAttacking = false;
  }

  IEnumerator DefenseBuff()
  {
    if (isAttacking) yield break;
    isAttacking = true;

    animator.SetTrigger("Ability");

    // 씬에 존재하는 모든 EnemyController 찾기
    EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();

    foreach (EnemyController enemy in allEnemies)
    {
      if (enemy == this) continue; // 자기 자신이면 넘어가기

      enemy.defence += 10f; // 방어력 10만큼 증가

      // 콜라이더를 이용해서 이펙트 위치 계산
      Collider2D collider = enemy.GetComponent<Collider2D>();

      Vector3 spawnPos = enemy.transform.position;
      if (collider != null)
      {
        // 콜라이더의 bounds를 이용해 가장 위쪽 지점 계산
        spawnPos = collider.bounds.center + new Vector3(0f, collider.bounds.extents.y, 0f);

        // 그리고 약간 오른쪽으로도 이동 (원하는 만큼 조정 가능)
        spawnPos += new Vector3(0.3f, 0.3f, 0f);
      }
      else
      {
        // 콜라이더가 없다면 기본 위치 (살짝 위)
        spawnPos += new Vector3(0.5f, 1.2f, 0f);
      }

      GameObject arrowObj = Instantiate(defUpEffect, spawnPos, Quaternion.identity, enemy.transform);

      Vector3 arrowScale = arrowObj.transform.localScale;
      arrowScale.x = enemy.transform.localScale.x > 0 ? Mathf.Abs(arrowScale.x) : -Mathf.Abs(arrowScale.x);
      arrowObj.transform.localScale = arrowScale;

      Destroy(arrowObj, 1.0f); // 1초 후 화살표 삭제
    }

    Debug.Log("모든 적 방어력 증가 + 화살표 표시!");

    yield return null;

    StartCoroutine(AttackCooldown());
  }
}

