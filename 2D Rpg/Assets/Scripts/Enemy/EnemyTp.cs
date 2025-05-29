using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTp : MonoBehaviour
{
  public GameObject tpEffect;
  GameObject player;
  Transform playerTransform;

  EnemyController ec;
  BossController bc;

  public float maxDistance = 15f;        // �÷��̾���� �ִ� �Ÿ�
  public float teleportRadius = 10f;     // �÷��̾� �ֺ� �ݰ�
  public float raycastHeight = 5f;       // ���� ���� ����

  private void Start()
  {
    player = GameObject.FindWithTag("Player");

    ec = GetComponent<EnemyController>();
    bc = GetComponent<BossController>();
  }

  private void Update()
  {
    if (player == null) return;
    if (ec != null && ec.isAttack == true)
    {
      return;
    }

    else if (bc != null && bc.isAttack == true)
    {
      return;
    }

    playerTransform = player.transform;

    // �÷��̾���� �Ÿ� (2D�� X, Y�� ��)
    float distance = Vector2.Distance(transform.position, playerTransform.position);

    if (distance > maxDistance)
    {
      Vector2 randomCircle;    // ������ ����
      Vector2 targetPos; // ��ǥ ��ġ ���
      float minDistanceFromPlayer = 2.0f; // �÷��̾� ��ġ�κ��� �ּ� �Ÿ� ����

      int maxAttempts = 10;
      int attempts = 0;

      do
      {
        randomCircle = Random.insideUnitCircle * teleportRadius;
        targetPos = (Vector2)playerTransform.position + randomCircle;
        attempts++;
      }

      while (Vector2.Distance(playerTransform.position, targetPos) < minDistanceFromPlayer && attempts < maxAttempts);

      Vector2 rayStart = targetPos + Vector2.up * raycastHeight;

      RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, raycastHeight * 2);
      
      // ������ ã���� ��
      if (hit.collider != null)
      {
        Vector2 groundPos = hit.point;

        // �ݶ��̴� �������� ���� �ڵ� ���
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
          // ������ �ݶ��̴� �� irregular shape���� ����
          groundPos.y = hit.collider.bounds.max.y + myCol.bounds.extents.y;
        }
        else
        {
          groundPos.y += 0.5f; // �ݶ��̴� ������ �⺻��
        }



        transform.position = groundPos;
        GameObject clone = Instantiate(tpEffect, groundPos, Quaternion.identity);
        Destroy(clone, 0.6f);
      }


      // ����� ���� ���� (Scene �信���� ����)
      Debug.DrawLine(rayStart, rayStart + Vector2.down * raycastHeight * 2, Color.red, 1f);
    }
  }
}
