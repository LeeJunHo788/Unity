using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : EnemyController
{
  //돌진형 적 전용 변수
  Vector2 dashDir;          //고정 돌진 방향
  bool isDashing = false;   //현재 돌진 중인가?
  float dashSpeed = 10f;     //돌진 스피드
  float dashTime = 0.5f;    //돌진 지속 시간
  float dashTimer = 0f;     // 돌진 타이머(일정시간 이후 돌진가능)
  float dashCooldown = 1f;  // 돌진 쿨타임
  

  float prepareTime = 0.5f;   //돌진 전 준비시간
  float prepareTimer = 0f;    //멈춤 타이머
  bool isPreparing = false;   //돌진 전 준비상태
  bool canDash = true;        // 돌진 가능 상태

  public Animator dashAnimator;
  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

    maxHp = 10f + sm.currentWave;
    currentHp = maxHp;
    moveSpeed = 2f;
    attackDem = 5f + sm.currentWave;
    defence = 10f + sm.currentWave;
    defIgnore = Mathf.Min(100f, 10f + sm.currentWave); // 최대 100으로 제한
  }
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (isDead) return; //죽었으면 돌진도 X

    if(!isDashing && !isPreparing)
      base.Update();

    //대시 중이거나 준비중이 아니라면
    if (!isDashing && !isPreparing && canDash)
    {
      //플레이어 위치 찾기
      float distance = Vector2.Distance(transform.position, player.transform.position);

      //돌진조건
      if (distance < 4f)
      {

        //돌진 전 준비 시작
        dashDir = (player.transform.position - transform.position).normalized;
        isPreparing = true;
        prepareTimer = prepareTime;


        //잠깐 idle상태로 전환
        dashAnimator.SetBool("isWalking", false);
      }
    }
    //돌진 준비중
    else if (!isDashing && isPreparing)
    {
      prepareTimer -= Time.deltaTime;

      if (prepareTimer <= 0f)
      {
        //돌진 시작
        isPreparing = false;
        isDashing = true;
        dashTimer = dashTime;
        StartCoroutine(DashColldown());

        //다시 걷는 상태로 전환
        dashAnimator.SetBool("isWalking", true);

      }
    }
    //돌진 중
    else if (isDashing)
    {
      rb.velocity = Vector2.zero; //돌진 중 기본 이동 멈춤
      //돌진 중에는 방향고정 및 대시
      rb.velocity = dashDir * dashSpeed;
      dashTimer -= Time.deltaTime;


      if (dashTimer <= 0f)
      {
        rb.velocity = Vector2.zero;
        isDashing = false; //돌진 종료 (다시 Enemy Controller대로 움직임)
      }
    }
  }
  //사망시 돌진 중단
  public override void Dead()
  {
    isDashing = false;
    isPreparing = false;

    base.Dead(); //EnemyController의 사망처리
  }

  IEnumerator DashColldown()
  {
    canDash = false;
    yield return new WaitForSeconds(dashCooldown);
    canDash = true;
  }

  protected override void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // 공격과 부딪혔을 경우
    {
      if(!isPreparing)    // 돌진 준비중에는 넉백 비활성화
      {
        Vector2 dir = (collision.transform.position - player.transform.position).normalized;    // 밀어낼 방향

        rb.AddForce(dir * pc.knockBackForce, ForceMode2D.Impulse);             // 넉백
        StartCoroutine(StopEnemy(rb));                            // 0.2초후 정지

      }

      //여기서부터 슬라이더 체력깎는 관련코드
      BulletController bc = collision.GetComponent<BulletController>();   // 계수를 가져오기 위해 불릿 컨트롤러 가져오기

      if (bc == null || bc.hasHit) return;  // 이미 피격한 총알이라면 피격효과 무시

      bc.hasHit = true;  // 피격 처리 표시

      //발사체 삭제
      Destroy(collision.gameObject);

      float damage = pc.attackDem * bc.damageCoefficient;                 // 받는 피해량 = 플레이어 공격력 * 공격의 계수

      // 최종 데미지
      float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f));

      bool isCritical = Random.value < (pc.criticalChance / 100f);        // 치명타 확률 적용

      // 치명타라면
      if (isCritical)
      {
        finalDamage *= pc.criticalDem;     // 데미지 적용
      }


      currentHp -= Mathf.RoundToInt(finalDamage);          //피해 적용
      slider.value = currentHp; //슬라이더갱신
      StartCoroutine(MaterialShift());

      if (currentHp <= 0)
      {
        Dead();
      }

      // 데미지 텍스트 생성
      GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
      dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // 데미지, 치명타 여부 전달
    }
  }


}
   
