using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  private Vector2 direction;      // 방향
  public float speed;             // 속도

  GameObject player;

  /*
  void Start()
  {
    player = GameObject.FindWithTag("Player");
    direction = player.transform.position - transform.position;
    direction += new Vector2(0, 0.5f);
    direction = direction.normalized; // 정규화

    // 총알이 날아가는 방향을 바라보도록 회전 설정
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle); // Z축 회전

  }*/



  void Update()
  {
    if(direction == Vector2.zero)
    {
      player = GameObject.FindWithTag("Player");
      direction = player.transform.position - transform.position;
      direction += new Vector2(0, 0.5f);
      direction = direction.normalized; // 정규화
                                        
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;    // 회전 설정
      transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    transform.position += (Vector3)(direction * speed * Time.deltaTime);
  }

  public void SetDirection(Vector2 dir)
  {
    direction = dir.normalized;

    // 회전 설정
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);
  }
}
