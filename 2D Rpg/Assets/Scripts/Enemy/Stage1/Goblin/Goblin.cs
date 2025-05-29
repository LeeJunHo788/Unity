using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Goblin : EnemyController
{
  public GameObject attack;

  protected override void Awake()
  {
    range = 70f; // 탐색 범위
    speed = 2.3f; // 이동속도
    att = 5;     // 공격력
    def = 5;     // 방어력
    hp = 14;      // 체력
    exp = 12;     // 경험치
    gold = 3;     // 돈
    defIgn = 15;   // 방어력 무시

    canAttack = true;
    attackRange = 1.5f;
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
      Vector2 pos = transform.position + new Vector3(0.3f, -0.1f);
      StartCoroutine(Attack(pos));

    }
    
    state = EnemyState.Idle;

    yield return new WaitForSeconds(1f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(1f); // 공격 후 쿨타임
    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.6f); // 0.3초 기다리기

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
