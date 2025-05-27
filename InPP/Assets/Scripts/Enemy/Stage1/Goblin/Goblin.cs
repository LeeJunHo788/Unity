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
    range = 70f; // Ž�� ����
    speed = 2.3f; // �̵��ӵ�
    att = 5;     // ���ݷ�
    def = 5;     // ����
    hp = 14;      // ü��
    exp = 12;     // ����ġ
    gold = 3;     // ��
    defIgn = 15;   // ���� ����

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
      Vector2 pos = transform.position + new Vector3(0.3f, -0.1f);
      StartCoroutine(Attack(pos));

    }
    
    state = EnemyState.Idle;

    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��
    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.6f); // 0.3�� ��ٸ���

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
