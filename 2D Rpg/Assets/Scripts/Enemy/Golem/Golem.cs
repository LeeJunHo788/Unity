using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : BossController
{
  public int phase = 1;

  public AnimatorOverrideController phase1Override;
  public AnimatorOverrideController phase2Override;
  public AnimatorOverrideController phase3Override;

  public AnimationClip upgrade1;
  public AnimationClip upgrade2;
  float upgradTime1;
  float upgradTime2;


  // 공격 프리팹
  public GameObject A_Attack2;
  public GameObject B_Attack1;
  public GameObject B_Attack2;
  public GameObject C_Attack1;
  public GameObject C_Attack2;
  public GameObject C_Attack3;

  protected override void Awake()
  {
    // 1페이즈 스탯
    range = 10.0f; // 탐색 범위
    attackRange = 5.0f; // 공격범위
    speed = 2.0f; // 이동속도
    att = 10;     // 공격력
    def = 10;     // 방어력
    hp = 650;      // 체력
    exp = 20;     // 경험치
    gold = 3;     // 돈
    defIgn = 5;   // 방어력 무시

    returnDistance = 5f;
  }

  protected override void Start()
  {
    currentHp = hp;
    base.Start();

    // 1페이즈 X축 위치를 고정
    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;

    upgradTime1 = upgrade1.length;
    upgradTime2 = upgrade2.length;

  }

  protected override void Update()
  {

    // 체력에 따라 페이즈 변경
    if (currentHp <= hp * 0.6f && phase == 1 && !isUpgrade)   // 2페이즈
    {
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange2());
      
    }

    if (currentHp <= hp * 0.3f && phase == 2 && !isUpgrade)   // 3 페이즈 
    {
      
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange3());
      
    }

    if(isUpgrade)
    {
      return;
    }

    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {
    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;
    state = EnemyState.Attack;

    if (phase == 1)   // 1페이즈의 경우
    {
      int randomAttack = Random.Range(0, 2); // 0~1 랜덤
      Debug.Log(randomAttack);

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1번 공격 실행

            StartCoroutine(Attack_A1());
          }
          break;

        case 1:
          {
            anim.SetTrigger("Attack2");   // 2번 공격 실행
            Vector2 hitDir = (player.transform.position - transform.position);

            if(hitDir.x > 0)
            {
              Vector2 pos = transform.position + new Vector3(1.5f, -0.5f);

              StartCoroutine(Attack_A2(pos));
              
            }

            else if (hitDir.x <= 0)
            {
              Vector2 pos = transform.position + new Vector3(-1.5f, -0.5f);
              StartCoroutine(Attack_A2(pos));
             
            }
          }

          break;
       
      }
    }

    else if(phase == 2) // 2페이즈의 경우
    {
      int randomAttack = Random.Range(0, 3); // 0~2 랜덤

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1번 공격 실행

            Vector2 pos = transform.position;

            StartCoroutine(Attack_B1(pos + new Vector2(1.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(3.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(5.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-1.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-3.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-5.5f, 1.5f)));

          }
          break;
        case 1:
          {
            anim.SetTrigger("Attack2");   // 2번 공격 실행
            Vector2 hitDir = (player.transform.position - transform.position);

            if (hitDir.x > 0)
            {
              Vector2 pos = transform.position + new Vector3(1.3f, -0.5f);

              StartCoroutine(Attack_B2(pos));

            }

            else if (hitDir.x <= 0)
            {
              Vector2 pos = transform.position + new Vector3(-1.3f, -0.5f);
              StartCoroutine(Attack_B2(pos));

            }

          }
          break;
        case 2:
          {
            anim.SetTrigger("Attack1");   // 1번 공격 실행

            Vector2 pos = transform.position;

            StartCoroutine(Attack_B1(pos + new Vector2(1.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(3.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(5.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-1.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-3.5f, 1.5f)));
            StartCoroutine(Attack_B1(pos + new Vector2(-5.5f, 1.5f)));

          }
          break;

      }
    }

    else if (phase == 3) // 3페이즈의 경우
    {
      int randomAttack = Random.Range(0, 3); // 0~2 랜덤

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1번 공격 실행
            StartCoroutine(Attack_C1(transform.position));
          }

          break;
        case 1:
          {
            anim.SetTrigger("Attack2");   // 2번 공격 실행
            StartCoroutine(Attack_C2(transform.position));

          }
          
          break;
        case 2:
          {
            anim.SetTrigger("Attack3");   // 3번 공격 실행
            StartCoroutine(Attack_C3(transform.position));
            StartCoroutine(Attack_C3(transform.position + new Vector3(0,2)));
            StartCoroutine(Attack_C3(transform.position + new Vector3(0,-2)));

          }
          break;

      }
    }

    yield return new WaitForSeconds(1f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(3f); // 공격 후 쿨타임

    isAttack = false;
    state = EnemyState.Idle;
  }

  protected override bool CanMove()
  {
    // 1페이즈에서는 이동 금지
    return phase != 1;
  }

  IEnumerator Attack_A1()
  {
    yield return new WaitForSeconds(0.7f); // 0.7초 기다리기
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.5f);

    // 감지된 콜라이더 검사 실행
    foreach (Collider2D col in colliders)
    {
      // 콜라이더의 태그가 Player라면
      if (col != null && col.CompareTag("Player"))
      {
        Vector2 hitDir = (col.transform.position - transform.position);

        pc.TakeDem(5, 15, hitDir);
      }
    }
  }

  IEnumerator Attack_A2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.6f); // 선딜

    GameObject clone = Instantiate(A_Attack2, pos, Quaternion.identity);


    Destroy(clone, 0.5f);
  }

  IEnumerator Attack_B1(Vector2 pos)
  {

    GameObject clone = Instantiate(B_Attack1, pos, Quaternion.identity);

    yield return new WaitForSeconds(1f); // 1초 기다리기

    // FallDown을 먼저 실행하고 기다려주기
    yield return StartCoroutine(FallDown(clone));

    Destroy(clone, 0.1f); // 떨어지고 나서 잠깐 있다가 삭제

  }


  IEnumerator FallDown(GameObject obj)
  {
    float duration = 0.5f;
    float elapsed = 0f;
    Vector2 startPos = obj.transform.position;
    Vector2 endPos = startPos + Vector2.down * 3.0f; // 2.5만큼 아래로 이동

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      t = Mathf.Pow(t, 3f); // 제곱으로 가속도 효과 주기 (더 강하게 떨어짐)

      obj.transform.position = Vector2.Lerp(startPos, endPos, t);
      elapsed += Time.deltaTime;
      yield return null;
    }

      obj.transform.position = endPos; // 정확한 위치 보정
  }

  IEnumerator Attack_B2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.4f); // 0.4초 기다리기

    GameObject clone = Instantiate(B_Attack2, pos, Quaternion.identity);

    Destroy(clone, 0.5f);
  }

  IEnumerator Attack_C1(Vector2 pos)
  {
    // 0.4초 후 안쪽 이펙트 생성
    yield return new WaitForSeconds(0.4f);
    GameObject clone1 = Instantiate(C_Attack1, pos + new Vector2(2.5f,0), Quaternion.identity);
    clone1.GetComponent<BoxCollider2D>().enabled = false;

    GameObject clone2 = Instantiate(C_Attack1, pos + new Vector2(-2.5f, 0), Quaternion.identity);
    clone2.GetComponent<BoxCollider2D>().enabled = false;

    // 0.4초 후 바깥 이펙트 생성
    yield return new WaitForSeconds(0.4f);
    GameObject clone3 = Instantiate(C_Attack1, pos + new Vector2(5f, 0), Quaternion.identity);
    clone3.GetComponent<BoxCollider2D>().enabled = false;

    GameObject clone4 = Instantiate(C_Attack1, pos + new Vector2(-5, 0), Quaternion.identity);
    clone4.GetComponent<BoxCollider2D>().enabled = false;



    // 일정시간 후 피격판정 활성화
    yield return new WaitForSeconds(1.4f);
    clone1.GetComponent<BoxCollider2D>().enabled = true;
    clone2.GetComponent<BoxCollider2D>().enabled = true;

    Destroy(clone1, 0.7f);
    Destroy(clone2, 0.7f);

    // 일정시간 후 피격판정 활성화
    yield return new WaitForSeconds(0.4f);
    clone3.GetComponent<BoxCollider2D>().enabled = true;
    clone4.GetComponent<BoxCollider2D>().enabled = true;

    Destroy(clone3, 0.7f);
    Destroy(clone4, 0.7f);

  }

  IEnumerator Attack_C2(Vector2 pos)
  {
    yield return new WaitForSeconds(1.0f); // 1초 기다리기

    GameObject clone = Instantiate(C_Attack2, pos + new Vector2(0, -0.7f), Quaternion.identity);


    Destroy(clone, 0.3f);
  }

  IEnumerator Attack_C3(Vector2 pos)
  {
    yield return new WaitForSeconds(0.4f); // 0.4초 기다리기

    GameObject clone = Instantiate(C_Attack3, pos, Quaternion.identity);

    if(clone != null)
    {
      Destroy(clone, 10f);
    }
  }


  IEnumerator PhaseChange2()
  {
    isUpgrade = true;
    

    // X축 고정을 해제
    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;

    attackRange = 3.0f; // 공격범위
    speed = 2.0f; // 이동속도
    att = 15;     // 공격력
    def = 20;     // 방어력

    yield return new WaitForSeconds(upgradTime1);

    isUpgrade = false;
    phase = 2;
    anim.runtimeAnimatorController = phase2Override; // 2페이즈 애니메이터로 교체
  }

  IEnumerator PhaseChange3()
  {
    isUpgrade = true;


    attackRange = 4.0f; // 공격범위
    speed = 2.75f; // 이동속도
    att = 20;     // 공격력
    def = 30;     // 방어력

    yield return new WaitForSeconds(upgradTime2);

    isUpgrade = false;
    phase = 3;
    anim.runtimeAnimatorController = phase3Override; // 2페이즈 애니메이터로 교체
  }
}
