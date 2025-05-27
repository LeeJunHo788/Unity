using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  Vector2 bulletDir;              // 총알 방향
  float bulletSpeed = 25f;         // 총알 속도
  public float damageCoefficient; // 데미지 계수
  public bool hasHit = false;  // 이미 적중했는지 확인하는 변수

  float lifeTime = 1f;


  private void Start()
  {
    damageCoefficient = 0.2f;
    Destroy(gameObject, lifeTime);
  }

  void Update()
  {
    transform.position += (Vector3)(bulletDir * bulletSpeed * Time.deltaTime);                         // 총알 이동
  }


  // 외부에서 방향을 넘겨받기 위한 메서드
  public void SetDirection(Vector2 dir)
  {
    bulletDir = dir;
  }
}
