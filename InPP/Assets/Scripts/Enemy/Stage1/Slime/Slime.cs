using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BossController
{

  // ���� ������
  public GameObject Attack2;



  protected override void Awake()
  {
    range = 70f; // Ž�� ����
    attackRange = 3.5f; // ���ݹ���
    speed = 2.0f; // �̵��ӵ�
    att = 10;     // ���ݷ�
    def = 10;     // ����
    hp = 200;      // ü��
    exp = 20;     // ����ġ
    gold = 15;     // ��
    defIgn = 5;   // ���� ����

    returnDistance = 10f;
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

    int randomAttack = Random.Range(0, 3); // 0~2 ����

    switch (randomAttack)
    {
      case 0:
        {
          anim.SetTrigger("Attack1");   // 1�� ���� ����

          StartCoroutine(Attack_1());
        }
        break;

      case 1:
        {
          anim.SetTrigger("Attack2");   // 2�� ���� ����

          Vector2 pos = transform.position + new Vector3(0, -1);
          StartCoroutine(Attack_2(pos));
          
        }
        break;

      case 2:
        {
          anim.SetTrigger("Attack2");   // 2�� ���� ����

          Vector2 pos = transform.position + new Vector3(0, -1);
          StartCoroutine(Attack_2(pos));

        }
        break;
    }



    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��

    isAttack = false;
    state = EnemyState.Idle;
  }


  // 1�� ����(���� �� ����)
  IEnumerator Attack_1()
  {
    yield return new WaitForSeconds(0.8f); // ����
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);

    // ������ �ݶ��̴� �˻� ����
    foreach (Collider2D col in colliders)
    {
      // �ݶ��̴��� �±װ� Player���
      if (col != null && col.CompareTag("Player"))
      {
        Vector2 hitDir = (col.transform.position - transform.position);

        // �μ� ���� : ������, �湫, ����
        pc.TakeDem(5, 15, hitDir);
      }
    }
  }


  // 2�� ����(���� ����)
  IEnumerator Attack_2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.8f); // 0.8�� ��ٸ���

    GameObject clone = Instantiate(Attack2, pos, Quaternion.identity);


    Destroy(clone, 0.3f);
  }



 

}
