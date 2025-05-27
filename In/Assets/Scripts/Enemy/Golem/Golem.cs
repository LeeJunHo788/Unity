using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : BossController
{
  public int phase = 1;
  protected bool isUpgrade = false;

  public AnimatorOverrideController phase1Override;
  public AnimatorOverrideController phase2Override;
  public AnimatorOverrideController phase3Override;

  public AnimationClip upgrade1;
  public AnimationClip upgrade2;
  float upgradTime1;
  float upgradTime2;


  // ���� ������
  public GameObject A_Attack2;
  public GameObject B_Attack1;
  public GameObject B_Attack2;
  public GameObject C_Attack1;
  public GameObject C_Attack2;
  public GameObject C_Attack3;

  protected override void Awake()
  {
    // 1������ ����
    range = 10.0f; // Ž�� ����
    attackRange = 3.0f; // ���ݹ���
    speed = 2.0f; // �̵��ӵ�
    att = 10;     // ���ݷ�
    def = 10;     // ����
    hp = 400;      // ü��
    exp = 20;     // ����ġ
    gold = 3;     // ��
    defIgn = 5;   // ���� ����

    returnDistance = 8f;
  }

  protected override void Start()
  {
    currentHp = hp;
    base.Start();

    // 1������ X�� ��ġ�� ����
    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;

    upgradTime1 = upgrade1.length;
    upgradTime2 = upgrade2.length;

  }

  protected override void Update()
  {

    // ü�¿� ���� ������ ����
    if (currentHp <= hp * 0.6f && phase == 1 && !isUpgrade)   // 2������
    {
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange2());
      
    }

    if (currentHp <= hp * 0.3f && phase == 2 && !isUpgrade)   // 3 ������ 
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
    rb.linearVelocity = Vector2.zero;
    state = EnemyState.Attack;

    if (phase == 1)   // 1�������� ���
    {
      int randomAttack = Random.Range(0, 2); // 0~1 ����
      Debug.Log(randomAttack);

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1�� ���� ����

            StartCoroutine(Attack_A1());
          }
          break;

        case 1:
          {
            anim.SetTrigger("Attack2");   // 2�� ���� ����
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

    else if(phase == 2) // 2�������� ���
    {
      int randomAttack = Random.Range(0, 3); // 0~2 ����

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1�� ���� ����

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
            anim.SetTrigger("Attack2");   // 2�� ���� ����
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
            anim.SetTrigger("Attack1");   // 1�� ���� ����

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

    else if (phase == 3) // 3�������� ���
    {
      int randomAttack = Random.Range(0, 3); // 0~2 ����

      switch (randomAttack)
      {
        case 0:
          {
            anim.SetTrigger("Attack1");   // 1�� ���� ����
            StartCoroutine(Attack_C1(transform.position));
          }

          break;
        case 1:
          {
            anim.SetTrigger("Attack2");   // 2�� ���� ����
            StartCoroutine(Attack_C2(transform.position));

          }
          
          break;
        case 2:
          {
            anim.SetTrigger("Attack3");   // 3�� ���� ����
            StartCoroutine(Attack_C3(transform.position));
            StartCoroutine(Attack_C3(transform.position + new Vector3(0,2)));
            StartCoroutine(Attack_C3(transform.position + new Vector3(0,-2)));

          }
          break;

      }
    }

    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��

    isAttack = false;
    state = EnemyState.Idle;
  }

  protected override bool CanMove()
  {
    // 1��������� �̵� ����
    return phase != 1;
  }

  IEnumerator Attack_A1()
  {
    yield return new WaitForSeconds(0.7f); // 0.7�� ��ٸ���
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.5f);

    // ������ �ݶ��̴� �˻� ����
    foreach (Collider2D col in colliders)
    {
      // �ݶ��̴��� �±װ� Player���
      if (col != null && col.CompareTag("Player"))
      {
        Vector2 hitDir = (col.transform.position - transform.position);

        pc.TakeDem(5, 15, hitDir);
      }
    }
  }

  IEnumerator Attack_A2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.6f); // ����

    GameObject clone = Instantiate(A_Attack2, pos, Quaternion.identity);


    Destroy(clone, 0.5f);
  }

  IEnumerator Attack_B1(Vector2 pos)
  {

    GameObject clone = Instantiate(B_Attack1, pos, Quaternion.identity);

    yield return new WaitForSeconds(1f); // 1�� ��ٸ���

    // FallDown�� ���� �����ϰ� ��ٷ��ֱ�
    yield return StartCoroutine(FallDown(clone));

    Destroy(clone, 0.1f); // �������� ���� ��� �ִٰ� ����

  }


  IEnumerator FallDown(GameObject obj)
  {
    float duration = 0.5f;
    float elapsed = 0f;
    Vector2 startPos = obj.transform.position;
    Vector2 endPos = startPos + Vector2.down * 3.0f; // 2.5��ŭ �Ʒ��� �̵�

    while (elapsed < duration)
    {
      float t = elapsed / duration;
      t = Mathf.Pow(t, 3f); // �������� ���ӵ� ȿ�� �ֱ� (�� ���ϰ� ������)

      obj.transform.position = Vector2.Lerp(startPos, endPos, t);
      elapsed += Time.deltaTime;
      yield return null;
    }

      obj.transform.position = endPos; // ��Ȯ�� ��ġ ����
  }

  IEnumerator Attack_B2(Vector2 pos)
  {
    yield return new WaitForSeconds(0.4f); // 0.4�� ��ٸ���

    GameObject clone = Instantiate(B_Attack2, pos, Quaternion.identity);

    Destroy(clone, 0.5f);
  }

  IEnumerator Attack_C1(Vector2 pos)
  {
    // 0.4�� �� ���� ����Ʈ ����
    yield return new WaitForSeconds(0.4f);
    GameObject clone1 = Instantiate(C_Attack1, pos + new Vector2(2.5f,0), Quaternion.identity);
    clone1.GetComponent<BoxCollider2D>().enabled = false;

    GameObject clone2 = Instantiate(C_Attack1, pos + new Vector2(-2.5f, 0), Quaternion.identity);
    clone2.GetComponent<BoxCollider2D>().enabled = false;

    // 0.4�� �� �ٱ� ����Ʈ ����
    yield return new WaitForSeconds(0.4f);
    GameObject clone3 = Instantiate(C_Attack1, pos + new Vector2(5f, 0), Quaternion.identity);
    clone3.GetComponent<BoxCollider2D>().enabled = false;

    GameObject clone4 = Instantiate(C_Attack1, pos + new Vector2(-5, 0), Quaternion.identity);
    clone4.GetComponent<BoxCollider2D>().enabled = false;



    // �����ð� �� �ǰ����� Ȱ��ȭ
    yield return new WaitForSeconds(1.4f);
    clone1.GetComponent<BoxCollider2D>().enabled = true;
    clone2.GetComponent<BoxCollider2D>().enabled = true;

    Destroy(clone1, 0.7f);
    Destroy(clone2, 0.7f);

    // �����ð� �� �ǰ����� Ȱ��ȭ
    yield return new WaitForSeconds(0.4f);
    clone3.GetComponent<BoxCollider2D>().enabled = true;
    clone4.GetComponent<BoxCollider2D>().enabled = true;

    Destroy(clone3, 0.7f);
    Destroy(clone4, 0.7f);

  }

  IEnumerator Attack_C2(Vector2 pos)
  {
    yield return new WaitForSeconds(1.0f); // 1�� ��ٸ���

    GameObject clone = Instantiate(C_Attack2, pos + new Vector2(0, -0.7f), Quaternion.identity);


    Destroy(clone, 0.3f);
  }

  IEnumerator Attack_C3(Vector2 pos)
  {
    yield return new WaitForSeconds(0.4f); // 0.4�� ��ٸ���

    GameObject clone = Instantiate(C_Attack3, pos, Quaternion.identity);

    if(clone != null)
    {
      Destroy(clone, 10f);
    }
  }


  IEnumerator PhaseChange2()
  {
    isUpgrade = true;
    

    // X�� ������ ����
    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;

    attackRange = 3.0f; // ���ݹ���
    speed = 2.0f; // �̵��ӵ�
    att = 15;     // ���ݷ�
    def = 20;     // ����

    yield return new WaitForSeconds(upgradTime1);

    isUpgrade = false;
    phase = 2;
    anim.runtimeAnimatorController = phase2Override; // 2������ �ִϸ����ͷ� ��ü
  }

  IEnumerator PhaseChange3()
  {
    isUpgrade = true;


    attackRange = 4.0f; // ���ݹ���
    speed = 2.75f; // �̵��ӵ�
    att = 20;     // ���ݷ�
    def = 30;     // ����

    yield return new WaitForSeconds(upgradTime2);

    isUpgrade = false;
    phase = 3;
    anim.runtimeAnimatorController = phase3Override; // 2������ �ִϸ����ͷ� ��ü
  }
}
