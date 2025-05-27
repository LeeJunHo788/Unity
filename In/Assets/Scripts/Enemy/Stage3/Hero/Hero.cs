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
    range = 35.0f; // Ž�� ����
    speed = 2.3f; // �̵��ӵ�
    att = 10;     // ���ݷ�
    def = 10;     // ����
    hp = 25;      // ü��
    exp = 20;     // ����ġ
    gold = 8;     // ��
    defIgn = 15;   // ���� ����

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
    rb.linearVelocity = Vector2.zero;
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

    yield return new WaitForSeconds(0.5f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(1.5f); // ���� �� ��Ÿ��
    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.3f); // 0.3�� ��ٸ���

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
