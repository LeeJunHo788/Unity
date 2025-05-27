using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Pengu : BossController
{
  public GameObject iceObj;
  public GameObject freezeObj; 
  public GameObject rayObj; 

  float attackRange = 6f;
  
  
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (currentHp <= 0) return;

    base.Update();    // 이동 관련만 처리중

    float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);

    if (distanceToPlayer < attackRange && !isAttacking)
    {
      Attack();
    }

  }

  void Attack()
  {
    isWalking = false;
    animator.SetBool("isWalking", isWalking);
    int rand = Random.Range(1, 4); // 1부터 3까지 랜덤 숫자 뽑기

    switch (rand)
    {
        case 1: StartCoroutine(Attack1()); break;
        case 2: StartCoroutine(Attack2()); break;
        case 3: StartCoroutine(Attack3()); break;
    }
  }

  IEnumerator Attack1()
  {
    isAttacking = true;
    animator.SetTrigger("Attack1");

    yield return new WaitForSeconds(0.5f); // 0.5초 선딜

    StartCoroutine(SpawnIceObj());

    yield return new WaitForSeconds(0.75f); // 0.5초 후딜

    isAttacking = false;
  }

  IEnumerator SpawnIceObj()
  {
    // 하나씩 순서대로 생성하면서 딜레이 주기
    for (int i = 0; i < 7; i++)
    {
      Instantiate(iceObj, player.transform.position + new Vector3(0, 2), Quaternion.identity);
      yield return new WaitForSeconds(0.25f); // 0.25초 간격으로 생성
    }
  }

  IEnumerator Attack2()
  {
    isAttacking = true;
    animator.SetTrigger("Attack2");

    int baseEffectCount = 10;    // 각 원에 몇 개씩 생성할지
    float effectInterval = 0.3f;   // 원 사이 생성 간격
    float ringSpacing = 1.5f;      // 각 원의 반지름 증가 간격

    Vector3 center =  transform.localScale == new Vector3(1,1,1) ?  transform.position + new Vector3(2, 0) : transform.position + new Vector3(-2, 0); // 생성 위치

    yield return new WaitForSeconds(0.45f); // 선딜레이

    for (int ring = 1; ring <= 4; ring++)
    {
      float radius = ring * ringSpacing;

      int effectCount = baseEffectCount;

      if (ring >= 3)
        effectCount = ring == 3 ? 15 : 20; 

      for (int i = 0; i < effectCount; i++)
      {
        float angle = i * Mathf.PI * 2 / effectCount;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        Vector3 spawnPos = center + offset;

        GameObject fx = Instantiate(freezeObj, spawnPos, Quaternion.identity);

        float scaleMultiplier = 1f + 0.2f * ring; // ring이 커질수록 크기도 커짐
        fx.transform.localScale *= scaleMultiplier;
      }

      yield return new WaitForSeconds(effectInterval); // 다음 원 생성까지 대기
    }

    yield return new WaitForSeconds(0.5f); // 총 공격 시간 유지
    isAttacking = false;
  }

 

  IEnumerator Attack3()
  {
    isAttacking = true;
    animator.SetTrigger("Attack3");

    yield return new WaitForSeconds(1f);

    Vector2 boxSize = new Vector2(4f, 2f); // 박스 크기
    Vector2 boxOffset = new Vector2(3f, -1f); // 감지 위치 오프셋

    // 박스의 중심 위치 계산 (적 기준으로 오프셋 적용)
    Vector2 origin = (Vector2)transform.position + boxOffset;

    // 박스 영역에 충돌한 모든 콜라이더를 검사
    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Player"))
      {
        hit.GetComponent<PlayerController>().TakeDamage(15, 50); // 함수 이름은 예시예요!
      }
    }

    Vector3 pos1 = player.transform.position + new Vector3(2, 1);
    yield return new WaitForSeconds(0.25f);
    Instantiate(rayObj, pos1, Quaternion.Euler(0,180,0));

    yield return new WaitForSeconds(0.25f);

    Vector3 pos2 = player.transform.position + new Vector3(-2, 1);
    yield return new WaitForSeconds(0.25f);
    Instantiate(rayObj, pos2, Quaternion.identity);

    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

}
