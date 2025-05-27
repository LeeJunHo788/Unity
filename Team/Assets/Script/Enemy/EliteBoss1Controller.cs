using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EliteBoss1State
{
  Idle,
  Walk,
  Attack,
  Monster, //쫄몹 소환
  Dead
}

public class EliteBoss1Controller : BossController
{
  //FSM형식구현
  public EliteBoss1State currentState = EliteBoss1State.Idle;
  private EliteBoss1State lastState;

  public float visionRange = 50f; //시야(무조건 쫓아오도록 넓게설정)

  //공격 관련 변수
  public GameObject attackPrefabAnim; //공격 이펙트 프리팹 애니메이션
  public GameObject attackPrefabSprite; //실제 나갈 이펙트 스프라이트
  public Transform attackPoint;   //이펙트 나가는 위치
  public float attackDuration = 5f; //공격 텀 5초
  private float attackTimer;        //공격 타이머

  //쫄몹 소환 관련
  public GameObject monsterPrefab; //소환할 쫄몹 프리팹
  public Transform center;         //소환 중심 포인트
  public int summonCount = 5;     //소환할 쫄몹 수
  public float summonRadius = 3f; //소환 반지름
  bool isSummoning = false; //소환중인지?
  bool hasSummoned = false; //소환했는가?

  protected override void Start()
  {
    base.Start();

    currentState = EliteBoss1State.Idle;
    attackTimer = attackDuration; //타이머 초기화
  }
  private new void Update()
  {
    if (player == null || isDead || currentState == EliteBoss1State.Dead) return;
    float dis = Vector2.Distance(transform.position, player.transform.position);

    //초기화
    attackTimer -= Time.deltaTime;

    switch(currentState)
    {
      case EliteBoss1State.Idle:
        if (dis <= visionRange)
          ChangeState(EliteBoss1State.Walk);
        break;
      case EliteBoss1State.Walk:
        Walk(); //실제로 걷는 함수
        if(currentHp <= maxHp * 0.3f && !hasSummoned) //체력30이하+소환한번도안했으면
        {
          ChangeState(EliteBoss1State.Monster);
        }
        //공격 쿨 다 돌때 공격
        else if (attackTimer <= 0f)
        {
          ChangeState(EliteBoss1State.Attack);
        }
        break;
      case EliteBoss1State.Attack:
        if (!isAttacking)//공격 중이 아닐 때만
        {
          //공격 이펙트 발사
          Attack();
        }
        break;
      case EliteBoss1State.Monster:
        if (!isSummoning) //소환중이 아닐때만
        {
          //쫄몹 소환
          SummonMonsters();
        }
        break;
      case EliteBoss1State.Dead:
        break;
    }
  }

  void ChangeState(EliteBoss1State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch (newState)
    {
      case EliteBoss1State.Idle:
        animator.SetBool("isWalking", false);
        break;
      case EliteBoss1State.Walk:
        animator.SetBool("isWalking", true);
        break;
      case EliteBoss1State.Attack:
        animator.SetTrigger("Attack");
        animator.SetBool("isWalking", false); //run은 걷는 상태 아니란걸 명시
                                              //함수호출
        break;
      case EliteBoss1State.Monster:
        animator.SetTrigger("Monster");
        animator.SetBool("isWalking", false);
        //함수호출
        break;
      case EliteBoss1State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //상속받는 BossController의 Dead함수 호출
        break;
    }
  }
  //Walk함수는 스테이지 보스들과 동일
  public void Walk()
  {
    dir = (player.transform.position - transform.position).normalized;    // 플레이어를 향하는 방향
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // 이동

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

  //공격 함수
  void Attack()
  {
    isAttacking = true; //공격시작

    if(attackPrefabAnim != null && attackPoint != null)
    {
      GameObject animObj = Instantiate(attackPrefabAnim, attackPoint.position, Quaternion.identity);
      // 플레이어 중심 방향 계산
      Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
      Vector2 firePos = attackPoint.position;
      Vector2 dir = (targetPos - firePos).normalized;

      // 회전 설정
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      animObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

      Animator anim = animObj.GetComponent<Animator>();
      if(anim != null)
      {
        anim.SetTrigger("Slash");
      }

      Destroy(animObj, 0.5f); //애니메이션 0.5초후 자동 삭제

      Invoke(nameof(SpawnSpriteAttack), 0.5f); //삭제 됨과 동시에 스프라이트 발사
    }
  }

  //실제 공격 이펙트 발사
  void SpawnSpriteAttack()
  {
    if(attackPrefabSprite != null && attackPoint != null)
    {
      GameObject spriteObj = Instantiate(attackPrefabSprite, attackPoint.position, Quaternion.identity);

      //플레이어 중심 위치로 정확히 방향 계산
      Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
      Vector2 firePos = attackPoint.position;
      Vector2 dir = (targetPos - firePos).normalized;

      // 스프라이트 방향 맞추기 (회전)
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      spriteObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

      //이동
      spriteObj.GetComponent<Rigidbody2D>().velocity = dir * 4f; //속도 조정

      attackTimer = attackDuration; //공격 쿨타임 리셋
      isAttacking = false; //공격이 끝난다
      ChangeState(EliteBoss1State.Walk); //다시 걷기
    }
  }
  //쫄몹 소환 함수
  void SummonMonsters()
  {
    isSummoning = true; //소환중
    hasSummoned = true; //소환됐다고 표시

    if(monsterPrefab != null && center != null)
    {
      //반복문으로 소환
      for(int i = 0; i < summonCount; i++)
      {
        Vector2 randomPos = Random.insideUnitCircle * summonRadius;
        Instantiate(monsterPrefab, center.position + (Vector3)randomPos, Quaternion.identity);
      }
    }
    attackTimer = attackDuration; //몇초동안 걷게 하기 위해서 attack쿨타임 리셋
    ChangeState(EliteBoss1State.Walk); //소환 끝나면 다시 걷기
  }
  public void ApplyFSMOnHit()
  {
    //데미지 처리는 상속받아 했으므로 여기는 상태 변환만
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(EliteBoss1State.Dead);
    }
    else if (currentState != EliteBoss1State.Attack && currentState != EliteBoss1State.Monster && currentState != EliteBoss1State.Dead && isAttacking)
    {
      ChangeState(EliteBoss1State.Walk);
    }
  }
}
