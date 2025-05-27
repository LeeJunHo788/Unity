using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EliteBoss3State
{
  Idle,
  Walk,
  Run,
  Attack,
  Dead
}
public class EliteBoss3Controller : BossController
{
  //FSM 형식 구현
  public EliteBoss3State currentState = EliteBoss3State.Idle;
  private EliteBoss3State lastState;

  public float visionRange = 50f; //시야 -> 걷기시작
  public float attackDuration = 18f; //20초 텀 중복공격 방지
  private float attackTimer;   //공격 타이머
  public GameObject attackEffectPrefab; //공격이펙트
  public Transform effectSpawnPoint;    //보스 중앙 위치

  //Run돌진형 공격 변수들
  public float runCooldown = 15f; //15초단위로 돌진
  private float runTimer; //타이머
  private bool isRunning = false;

  private bool isShield = false; //실드스킬 썼는가?

  public Transform shieldPoint;         //실드 이펙트 위치
  public GameObject shieldEffectPrefab; //실드 이펙트 프리팹(연출용)

  protected override void Start()
  {
    base.Start();
    currentState = EliteBoss3State.Idle;
    animator.ResetTrigger("Run");
    animator.ResetTrigger("Attack");
    animator.ResetTrigger("Shield");

    currentState = EliteBoss3State.Idle;
    runTimer = runCooldown;
    attackTimer = attackDuration;
  }

  private new void Update()
  {
    if (player == null || isDead || currentState == EliteBoss3State.Dead) return;
    float dis = Vector2.Distance(transform.position, player.transform.position);

    runTimer -= Time.deltaTime;
    attackTimer -= Time.deltaTime;

    if (!canAttack && attackTimer <= 0f)
    {
      canAttack = true;
    }

    switch (currentState)
    {
      case EliteBoss3State.Idle:
      case EliteBoss3State.Walk:
        Walk(); //실제로 걷는 함수

        //실드
        if (!isShield && currentHp <= maxHp * 0.7f) //체력이 70퍼이하
        {
          StartCoroutine(ShieldBuff());
          isShield = true;
          return;
        }
        //회오리 돌진 공격
        if (runTimer <= 0f)
        {
          runTimer = runCooldown;
          ChangeState(EliteBoss3State.Run);
          return;
        }
        //근접공격
        if (!isAttacking && canAttack && attackTimer <= 0f && currentState != EliteBoss3State.Attack)
        {
          isAttacking = true;
          canAttack = false;
          attackTimer = attackDuration; //여기서 한 번 초기화
          ChangeState(EliteBoss3State.Attack);
          return;
        }
        //상태 전이 판단
        if (dis <= visionRange)
          ChangeState(EliteBoss3State.Walk);
        else
          ChangeState(EliteBoss3State.Idle);
        break;
      case EliteBoss3State.Run:
        break;
      case EliteBoss3State.Attack:
        break;
      case EliteBoss3State.Dead:
        break;
    }
  }
  void ChangeState(EliteBoss3State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

      switch (newState)
      {
        case EliteBoss3State.Idle:
          animator.SetBool("isWalking", false);
          break;
        case EliteBoss3State.Walk:
          animator.SetBool("isWalking", true);
          break;
        case EliteBoss3State.Run:
          animator.SetTrigger("Run");
          animator.SetBool("isWalking", false); //run은 걷는 상태 아니란걸 명시
          //함수호출
          break;
        case EliteBoss3State.Attack:
          animator.SetTrigger("Attack");
          animator.SetBool("isWalking", false);
        //함수호출
        break;
        case EliteBoss3State.Dead:
          animator.SetTrigger("Dead");
         animator.SetBool("isWalking", false);
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
  
  //이벤트함수로 처리
  public void StartRun()
  {
    if (isRunning) return; //이미 실행중이면 중복실행X

    isRunning = true;
    StartCoroutine(RunRoutine());
  }

  IEnumerator RunRoutine()
  {
    moveSpeed *= 1.5f; //돌진 속도 증가

    float runTime = 8f;
    float timer = 0f;

    while(timer < runTime)
    {
      Walk(); //이동함수호출
      timer += Time.deltaTime;
      yield return null;
    }

    moveSpeed /= 1.5f;
    isRunning = false;
    runTimer = runCooldown; //초기화
    
    ChangeState(EliteBoss3State.Walk);
    
  }

  IEnumerator ShieldBuff()
  {
    animator.SetTrigger("Shield"); //애니메이션 재생
    //쉴드 이펙트 생성
    GameObject shield = Instantiate(shieldEffectPrefab, shieldPoint.position, Quaternion.identity, shieldPoint);
    Destroy(shield, 1.5f); //1.5초뒤 제거

    defence *= 5;         //방어력 증가
    yield return new WaitForSeconds(30f); //30초 동안 유지
    defence /= 5;         //방어력 원래대로 돌아옴

  }
  public void EndAttack()
  {
    Debug.Log("EndAttack실행");
    Debug.Log($"[쿨타임 초기화] 이전 attackTimer: {attackTimer}");

    isAttacking = false;
    attackTimer = attackDuration; //여기서 쿨타임 초기화
    Debug.Log($"[쿨타임 초기화 완료] 새로운 attackTimer: {attackTimer}");
    ChangeState(EliteBoss3State.Walk);

    Debug.Log("");
  }

  public void EnableAttackEffect()
  {
    GameObject effect = Instantiate(attackEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
    var explosion = effect.GetComponent<ExplosionDamage>();
    explosion.useExpansion = false; //혹시 모르니 코드상으로도 false설정
    explosion.duration = 8f; //8초간유지
    effect.transform.SetParent(transform); //보스 따라다니도록 
  }

  public void ApplyFSMOnHit()
  {
    //데미지 처리는 상속받아 했으므로 여기는 상태 변환만
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(EliteBoss3State.Dead);

      //스테이지 클리어 처리 확인 나중에 정리할때 추가예정 
      //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    else if (currentState != EliteBoss3State.Run && currentState != EliteBoss3State.Attack && currentState != EliteBoss3State.Dead && isAttacking)
    {
      ChangeState(EliteBoss3State.Walk);
    }
  }
}
