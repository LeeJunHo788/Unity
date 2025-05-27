using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceObj : EnemyAttack
{
  public Vector3 offset;
  public float fallSpeed;
  public float delayBeforeFall; // 낙하 지연 시간
  public float fallDuration;    // 낙하 지속 시간

  private Vector3 startPos;
  private float timer = 0f;
  private bool isFalling = false;
  private float fallTimer = 0f;

  private void Start()
  {
    startPos = transform.position = transform.parent.position + offset;

    fallSpeed = 3.5f;
    delayBeforeFall = 0.45f; // 낙하 지연 시간
    fallDuration = 0.4f;    // 낙하 지속 시간

    lifeTime = 1.5f;        // 생존 시간
    attDamage = 10f;       // 데미지
    DefIgnore = 20f;       // 방어력무시
    destroyOnHit = false;     // 피격 시 오브젝트를 없앨지 여부

    Destroy(transform.parent.gameObject, 1.5f); // 부모까지 같이 파괴
  }


  protected override void Update()
  {
    if (!isFalling)
    {
      timer += Time.deltaTime;
      if (timer >= delayBeforeFall)
      {
        isFalling = true; // 낙하 시작
        fallTimer = 0f;   // 낙하 시간 카운트 시작
      }
      else return;
    }

    if (fallTimer < fallDuration)
    {
      fallTimer += Time.deltaTime;
      transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
  }




}
