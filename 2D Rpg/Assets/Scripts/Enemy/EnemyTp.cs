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

  public float maxDistance = 15f;        // 플레이어와의 최대 거리
  public float teleportRadius = 10f;     // 플레이어 주변 반경
  public float raycastHeight = 5f;       // 레이 시작 높이

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

    // 플레이어와의 거리 (2D라서 X, Y만 비교)
    float distance = Vector2.Distance(transform.position, playerTransform.position);

    if (distance > maxDistance)
    {
      Vector2 randomCircle;    // 무작위 방향
      Vector2 targetPos; // 목표 위치 계산
      float minDistanceFromPlayer = 2.0f; // 플레이어 위치로부터 최소 거리 제한

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
      
      // 지면을 찾았을 때
      if (hit.collider != null)
      {
        Vector2 groundPos = hit.point;

        // 콜라이더 기준으로 높이 자동 계산
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
          // 폴리곤 콜라이더 등 irregular shape에도 대응
          groundPos.y = hit.collider.bounds.max.y + myCol.bounds.extents.y;
        }
        else
        {
          groundPos.y += 0.5f; // 콜라이더 없으면 기본값
        }



        transform.position = groundPos;
        GameObject clone = Instantiate(tpEffect, groundPos, Quaternion.identity);
        Destroy(clone, 0.6f);
      }


      // 디버그 레이 보기 (Scene 뷰에서만 보임)
      Debug.DrawLine(rayStart, rayStart + Vector2.down * raycastHeight * 2, Color.red, 1f);
    }
  }
}
