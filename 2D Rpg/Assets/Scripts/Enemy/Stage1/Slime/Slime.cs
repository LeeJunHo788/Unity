using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BossController
{

  // 공격 프리팹
  public GameObject Attack2;



  protected override void Awake()
  {
    range = 70f; // 탐색 범위
    attackRange = 3.5f; // 공격범위
    speed = 2.0f; // 이동속도
    att = 10;     // 공격력
    def = 10;     // 방어력
    hp = 200;      // 체력
    exp = 20;     // 경험치
    gold = 15;     // 돈
    defIgn = 5;   // 방어력 무시

    returnDistance = 100f;
  }

  protected override void Start()
  {
    currentHp = hp;


    base.Start();

  }

  protected override void Update()
  {

    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {
    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;

    int randomAttack = Random.Range(0, 3); // 0~2 랜덤

    switch (randomAttack)
    {
      case 0:
        {
          anim.SetTrigger("Attack1");   // 1번 공격 실행

          StartCoroutine(Attack_1());
        }
        break;

      case 1:
        {
          anim.SetTrigger("Attack2");   // 2번 공격 실행

          Vector2 pos = transform.position + new Vector3(0, -1);
          StartCoroutine(Attack_2(pos));
          
        }
        break;

      case 2:
        {
          anim.SetTrigger("Attack2");   // 2번 공격 실행

          Vector2 pos = transform.position + new Vector3(0, -1);
          StartCoroutine(Attack_2(pos));

        }
        break;
    }



    yield return new WaitForSeconds(1f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(1f); // 공격 후 쿨타임

    isAttack = false;
    state = EnemyState.Idle;
  }


  // 1번 공격(범위 내 공격)
  IEnumerator Attack_1()
  {
    yield return new WaitForSeconds(0.8f); // 선딜
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);

    // 감지된 콜라이더 검사 실행
    foreach (Collider2D col in colliders)
    {
      // 콜라이더의 태그가 Player라면
      if (col != null && col.CompareTag("Player"))
      {
        Vector2 hitDir = (col.transform.position - transform.position);

        // 인수 순서 : 데미지, 방무, 방향
        pc.TakeDem(5, 15, hitDir);
      }
    }
  }


  // 2번 공격(점프 공격)
  IEnumerator Attack_2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.8f); // 0.8초 기다리기

    GameObject clone = Instantiate(Attack2, pos, Quaternion.identity);


    Destroy(clone, 0.3f);
  }



 

}
