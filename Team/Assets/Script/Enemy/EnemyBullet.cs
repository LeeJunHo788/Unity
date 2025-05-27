using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
  Vector2 direction;
  public float speed = 5f;
  public float lifeTime = 3f;

  public void SetDirection(Vector2 dir)
  {
    direction = dir.normalized;
    Destroy(gameObject,lifeTime);
  }

  void Update()
  {
    transform.Translate(Vector3.right * speed * Time.deltaTime);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.CompareTag("Player"))
    {
      //피해처리 향후 수정
      Destroy(gameObject);
    }
  }
}
