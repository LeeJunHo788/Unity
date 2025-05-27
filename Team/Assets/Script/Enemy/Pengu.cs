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

    base.Update();    // �̵� ���ø� ó����

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
    int rand = Random.Range(1, 4); // 1���� 3���� ���� ���� �̱�

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

    yield return new WaitForSeconds(0.5f); // 0.5�� ����

    StartCoroutine(SpawnIceObj());

    yield return new WaitForSeconds(0.75f); // 0.5�� �ĵ�

    isAttacking = false;
  }

  IEnumerator SpawnIceObj()
  {
    // �ϳ��� ������� �����ϸ鼭 ������ �ֱ�
    for (int i = 0; i < 7; i++)
    {
      Instantiate(iceObj, player.transform.position + new Vector3(0, 2), Quaternion.identity);
      yield return new WaitForSeconds(0.25f); // 0.25�� �������� ����
    }
  }

  IEnumerator Attack2()
  {
    isAttacking = true;
    animator.SetTrigger("Attack2");

    int baseEffectCount = 10;    // �� ���� �� ���� ��������
    float effectInterval = 0.3f;   // �� ���� ���� ����
    float ringSpacing = 1.5f;      // �� ���� ������ ���� ����

    Vector3 center =  transform.localScale == new Vector3(1,1,1) ?  transform.position + new Vector3(2, 0) : transform.position + new Vector3(-2, 0); // ���� ��ġ

    yield return new WaitForSeconds(0.45f); // ��������

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

        float scaleMultiplier = 1f + 0.2f * ring; // ring�� Ŀ������ ũ�⵵ Ŀ��
        fx.transform.localScale *= scaleMultiplier;
      }

      yield return new WaitForSeconds(effectInterval); // ���� �� �������� ���
    }

    yield return new WaitForSeconds(0.5f); // �� ���� �ð� ����
    isAttacking = false;
  }

 

  IEnumerator Attack3()
  {
    isAttacking = true;
    animator.SetTrigger("Attack3");

    yield return new WaitForSeconds(1f);

    Vector2 boxSize = new Vector2(4f, 2f); // �ڽ� ũ��
    Vector2 boxOffset = new Vector2(3f, -1f); // ���� ��ġ ������

    // �ڽ��� �߽� ��ġ ��� (�� �������� ������ ����)
    Vector2 origin = (Vector2)transform.position + boxOffset;

    // �ڽ� ������ �浹�� ��� �ݶ��̴��� �˻�
    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Player"))
      {
        hit.GetComponent<PlayerController>().TakeDamage(15, 50); // �Լ� �̸��� ���ÿ���!
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
