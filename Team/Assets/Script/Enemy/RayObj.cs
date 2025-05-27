using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayObj : EnemyAttack
{

  public Vector2 boxSize = new Vector2(5f, 3f); // 박스 크기
  public Vector2 boxOffset = new Vector2(-1f, -1f); // 감지 위치 오프셋


  private void Start()
  {
    attDamage = 15f;
    DefIgnore = 50f;


    StartCoroutine(Attack());

    Destroy(gameObject, 0.7f);
  }

  IEnumerator Attack()
  {
    yield return new WaitForSeconds(0.25f); // 0.25초 기다림

    // 박스의 중심 위치 계산 (적 기준으로 오프셋 적용)
    Vector2 origin = (Vector2)transform.position + boxOffset;

    // 박스 영역에 충돌한 모든 콜라이더를 검사
    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Player"))
      {
        hit.GetComponent<PlayerController>().TakeDamage(attDamage, DefIgnore); // 함수 이름은 예시예요!
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
