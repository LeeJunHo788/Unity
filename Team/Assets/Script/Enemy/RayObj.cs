using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayObj : EnemyAttack
{

  public Vector2 boxSize = new Vector2(5f, 3f); // �ڽ� ũ��
  public Vector2 boxOffset = new Vector2(-1f, -1f); // ���� ��ġ ������


  private void Start()
  {
    attDamage = 15f;
    DefIgnore = 50f;


    StartCoroutine(Attack());

    Destroy(gameObject, 0.7f);
  }

  IEnumerator Attack()
  {
    yield return new WaitForSeconds(0.25f); // 0.25�� ��ٸ�

    // �ڽ��� �߽� ��ġ ��� (�� �������� ������ ����)
    Vector2 origin = (Vector2)transform.position + boxOffset;

    // �ڽ� ������ �浹�� ��� �ݶ��̴��� �˻�
    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Player"))
      {
        hit.GetComponent<PlayerController>().TakeDamage(attDamage, DefIgnore); // �Լ� �̸��� ���ÿ���!
      }
    }
  }
    private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Vector2 origin = (Vector2)transform.position + boxOffset;
    Gizmos.DrawWireCube(origin, boxSize);
  }

}
