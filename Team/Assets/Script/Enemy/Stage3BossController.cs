using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Boss3State
{
  Idle,
  Walk,
  Explosion,
  Heal,
  Dead
}
public class Stage3BossController : BossController
{
  //FSM 형식 구현
  public Boss3State currentState = Boss3State.Idle;
  private Boss3State lastState;

  public float visionRange = 10f; //시야

  //Explosion관련 변수
  public GameObject warningEffectPrefab;
  public GameObject explosionEffectPrefab;
  public float explosionCooldown = 10f; //10초에 한 번씩 공격
  private float explosionTimer;         //공격 타이머
  float explosionDuration = 2f;         //공격중에 상태유지
  float explosionStateTimer = 0f;
  
  public Transform[] spawnPoints; //Explosion 랜덤 위치 후보들
  

  //자힐 관련 변수
  public float healCooldown = 15f; //15초에 한 번씩 자힐
  private float healTimer;         //힐타이머
  public Transform healPoint;     //힐 이펙트 위치 
  public GameObject healEffectPrefab; //힐 이펙트 프리팹(연출용)

  protected override void Start()
  {
    base.Start();
    currentState = Boss3State.Idle;

    //타이머 초기화
    healTimer = healCooldown; 
    explosionTimer = explosionCooldown; 
  }
  private new void Update()
  {
    if (player == null || isDead || currentState == Boss3State.Dead) return;

    healTimer -= Time.deltaTime;
    explosionTimer -= Time.deltaTime;

    float dis = Vector2.Distance(transform.position, player.transform.position);

    switch (currentState)
    {
      case Boss3State.Idle:
      case Boss3State.Walk:
        Walk();
        //자힐조건
        if (healTimer <= 0f)
        {
          ChangeState(Boss3State.Heal); // 15초 경과하면 자힐 시작
          healTimer = healCooldown;     // 자힐 후 쿨타임 초기화
        }
        //폭발조건
        if(explosionTimer <= 0f)
        {
          ChangeState(Boss3State.Explosion);
          //체력 300 이하로 떨어지면 쿨타임 6초로 설정, 더 자주 공격
          explosionTimer = (currentHp <= 300f) ? 6f : explosionCooldown;
          explosionStateTimer = explosionDuration; //공격중 상태유지
        }

        //플레이어와 가까우면 Walk상태
        if(dis <= visionRange)
        {
          ChangeState(Boss3State.Walk);
        }
        else
        {
          ChangeState(Boss3State.Idle);
        }
          break;
      case Boss3State.Heal:
        //완료는 이벤트함수로 호출할거기때문에 생략
        break;
      case Boss3State.Explosion:
        explosionStateTimer -= Time.deltaTime;
        if(explosionStateTimer <= 0f)
        {
          ChangeState(Boss3State.Idle);
        }
        break;
      case Boss3State.Dead:
          break;
    }
  }

  void ChangeState(Boss3State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch(newState)
    {
      case Boss3State.Idle:
        animator.SetBool("isWalking", false);
        break;
      case Boss3State.Walk:
        animator.SetBool("isWalking", true);
        break;
      case Boss3State.Explosion:
        animator.SetTrigger("Explosion");
        TriggerExplosion();
        break;
      case Boss3State.Heal:
        animator.SetTrigger("Heal");
        break;
      case Boss3State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //상속받는 BossController의 Dead함수 호출
        break;
    }
  }

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
  //Heal애니메이션에서 이벤트함수 호출용
  public void HealComplete()
  {
    float healAmount = 300f;
    currentHp = Mathf.Min(currentHp + healAmount, maxHp);

    if(healPoint != null && healEffectPrefab != null)
    {
      GameObject healEffect = Instantiate(healEffectPrefab, healPoint.position, Quaternion.identity, healPoint);
      Destroy(healEffect, 1.5f); //제거
    }

    //힐 하는 동안 보라색으로 바뀜
    StartCoroutine(TurnPurple());

    ChangeState(Boss3State.Idle);
  }

  private IEnumerator TurnPurple()
  {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    Color originalColor = sr.color;

    //반투명
    sr.color  = new Color(0.6f, 0.3f, 0.9f, 0.9f);

    yield return new WaitForSeconds(1.5f); //이펙트 지속시간과 맞춤

    sr.color = originalColor;
  }

  //Explosion 실행 (경고 + 폭발)
  public void TriggerExplosion()
  {
    Debug.Log("폭발 시작");
    // SpawnPoint 중복 없이 3개 랜덤 선택
    List<int> indexPool = new List<int>();
    for (int i = 0; i < spawnPoints.Length; i++) indexPool.Add(i);

    int spawnCount = Mathf.Min(3, spawnPoints.Length); // 혹시 3개 미만일 경우 대비
    List<Transform> selectedPoints = new List<Transform>();

    for (int i = 0; i < spawnCount; i++)
    {
      int rndIdx = Random.Range(0, indexPool.Count);
      int selected = indexPool[rndIdx];
      selectedPoints.Add(spawnPoints[selected]);
      indexPool.RemoveAt(rndIdx);
    }

    // 각 위치에 경고, 1초 후 폭발
    foreach (Transform point in selectedPoints)
    {
      Vector3 spawnPos = point.position;
      GameObject warning = Instantiate(warningEffectPrefab, spawnPos, Quaternion.identity); // 경고
      Destroy(warning, 1.5f); //준비 프리팹은 1.5초뒤 자동제거

      StartCoroutine(DelayedExplosion(spawnPos));
    }
  }

  IEnumerator DelayedExplosion(Vector3 pos)
  {
    yield return new WaitForSeconds(1f);
    Instantiate(explosionEffectPrefab, pos, Quaternion.identity);
    ChangeState(Boss3State.Idle);
  }

  public void ApplyFSMOnHit()
  {
    //데미지 처리는 상속받아 했으므로 여기는 상태 변환만
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(Boss3State.Dead);
    }
    else if (currentState != Boss3State.Explosion && currentState != Boss3State.Heal && currentState != Boss3State.Dead)
    {
      currentState = Boss3State.Walk;
    }
  }
}
