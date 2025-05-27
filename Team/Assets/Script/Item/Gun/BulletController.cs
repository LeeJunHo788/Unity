using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  Vector2 bulletDir;              // �Ѿ� ����
  float bulletSpeed = 25f;         // �Ѿ� �ӵ�
  public float damageCoefficient; // ������ ���
  public bool hasHit = false;  // �̹� �����ߴ��� Ȯ���ϴ� ����

  float lifeTime = 1f;


  private void Start()
  {
    damageCoefficient = 0.2f;
    Destroy(gameObject, lifeTime);
  }

  void Update()
  {
    transform.position += (Vector3)(bulletDir * bulletSpeed * Time.deltaTime);                         // �Ѿ� �̵�
  }


  // �ܺο��� ������ �Ѱܹޱ� ���� �޼���
  public void SetDirection(Vector2 dir)
  {
    bulletDir = dir;
  }
}
