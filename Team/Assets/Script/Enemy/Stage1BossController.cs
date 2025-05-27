using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public enum Boss1State
{
  Idle,
  Walk,
  Attack,
  Dead
}
public class Stage1BossController : BossController
{
  //FSM 형식 구현
  public Boss1State currentState = Boss1State.Idle;
  private Boss1State lastState;

  public float visionRange = 50f; //시야
  public float attackRange = 5f; //공격 시작 범위

  public float attackDuartion = 1f; //쿨타임

  bool alternatePattern = true; //true = 폭발, false=쫄몹

  //폭발 이펙트 프리팹
  public GameObject explosionEffect;
  public Transform[] explosionPoints; //슬라임 주변에 배치된 SpawnPoint
  public Transform[] outerExplosionPoints; //2단(바깥쪽)
  public float explosionDelay = 0.75f; //1,2단 사이 시간차

  //쫄몹 소환 프리팹
  public GameObject minionPrefab;     //소환할 쫄몹 프리팹
  public Transform[] minionSpawnPoints; //스폰위치


  protected override void Start()
  {
    base.Start();
    currentState = Boss1State.Idle;
  }

  private new void Update()
  {
    if (player == null || isDead) return;

    float dis = Vector2.Distance(transform.position, player.transform.position);

    switch (currentState)
    {
      case Boss1State.Idle:
        if (dis <= visionRange) ChangeState(Boss1State.Walk);
        break;
      case Boss1State.Walk:
        if (dis <= attackRange && canAttack) ChangeState(Boss1State.Attack);
        else Chase();
        break;
      case Boss1State.Attack:
        Attack();
        break;
    }
  }

  void ChangeState(Boss1State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch (newState)
    {
      case Boss1State.Idle:
        animator.SetInteger("State", 0);
        break;
      case Boss1State.Walk:
        animator.SetInteger("State", 1);
        break;
      case Boss1State.Attack:
        animator.SetInteger("State", 2);
        break;
      case Boss1State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //상속받는 BossController의 Dead함수 호출
        break;
    }
  }

  void Chase()
  {
    //EnemyController의 이동 함수와 동일

    dir = (player.transform.position - transform.position).normalized;    // 플레이어를 향하는 방향
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // 이동

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;              // 각도 구하기

    animator.SetInteger("State", 1); //Walk애니메이션으로 변경

    //방향에 따라 flip처리
    if (dir.x >= 0)
    {
      transform.localScale = new Vector3(1, 1, 1);//오른쪽
      slider.transform.localScale = new Vector3(Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
    else
    {
      transform.localScale = new Vector3(-1, 1, 1); //좌우반전
      slider.transform.localScale = new Vector3(-Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
  }
  void Attack()
  {
    animator.SetInteger("State", 2); //애니메이션 변경
    if (!isAttacking)
    {
      isAttacking = true;
      StartCoroutine(AttackCoroutine());
    }
  }

  IEnumerator AttackCoroutine()
  {
    canAttack = false;
    yield return new WaitForSeconds(0.6f);//공격애니메이션 끝날 때 까지 대기
    currentState = Boss1State.Walk; //Walk로전환

    yield return new WaitForSeconds(attackDuartion); //쿨타임

    isAttacking = false;
    canAttack = true;
  }
  //애니메이션 이벤트로 호출될 함수
  public void AttackTrigger()
  {
    if (currentHp <= 200)
    {
      ExplosionAttack(); //이펙트 공격 실행
    }

    if(currentHp <= 100) //체력 50%이하로 떨어지면 이펙트 두줄
    {
      DoubleExplosion(); //더블 이펙트 공격 실행
    }

    else if (currentHp <= 50f) //체력 30% 이하면 쫄몹 소환도 추가
    {
      if (alternatePattern)
      {
        SpawnMinion();
      }
      alternatePattern = !alternatePattern; //true/false전환
    }
  }

  void ExplosionAttack()
  {
    foreach (Transform point in explosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }
  }

  void DoubleExplosion()
  {
    StartCoroutine(DoubleExplosionRoutine());
  }

  IEnumerator DoubleExplosionRoutine()
  {
    //1단 먼저 실행
    foreach (Transform point in explosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }

    yield return new WaitForSeconds(explosionDelay); //시간차

    //2단 실행
    foreach(Transform point in outerExplosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }
  }

  void SpawnMinion()
  {
    if (minionSpawnPoints.Length == 0) return; //스폰 포인트 없으면 실행X
    
    foreach(Transform point in minionSpawnPoints)
    {
      Instantiate(minionPrefab, point.position, Quaternion.identity);
    }
   
  }

  public void ApplyFSMOnHit()
  {
    //데미지 처리는 상속받아 했으므로 여기는 상태 변환만
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(Boss1State.Dead);
    }
    else if (currentState != Boss1State.Attack && currentState != Boss1State.Dead)
    {
      currentState = Boss1State.Walk;
    }
  }
}
