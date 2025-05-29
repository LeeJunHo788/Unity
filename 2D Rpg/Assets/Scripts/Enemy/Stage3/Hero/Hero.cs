using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Hero : EnemyController
{
  public GameObject attack;

  protected override void Awake()
  {
    range = 80.0f; // 탐색 범위
    speed = 3.5f; // 이동속도
    att = 10;     // 공격력
    def = 10;     // 방어력
    hp = 25;      // 체력
    exp = 20;     // 경험치
    gold = 8;     // 돈
    defIgn = 15;   // 방어력 무시

    canAttack = true;
    attackRange = 2.0f;
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
    state = EnemyState.Attack;

    anim.SetTrigger("Attack");
    Vector2 hitDir = (player.transform.position - transform.position);


    if (hitDir.x > 0)
    {
      Vector2 pos = transform.position + new Vector3(0.8f, -0.1f);

      StartCoroutine(Attack(pos));

    }

    else if (hitDir.x <= 0)
    {
      Vector2 pos = transform.position + new Vector3(-1.1f, -0.1f);
      StartCoroutine(Attack(pos));

    }

    state = EnemyState.Idle;

    yield return new WaitForSeconds(0.5f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(1.5f); // 공격 후 쿨타임
    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.3f); // 0.3초 기다리기

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(attack, pos, Quaternion.identity);

    Destroy(clone, 0.3f);
  }
}
