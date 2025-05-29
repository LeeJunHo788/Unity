using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
  private Vector2 direction;      // ����
  public float speed;             // �ӵ�

  GameObject player;

  /*
  void Start()
  {
    player = GameObject.FindWithTag("Player");
    direction = player.transform.position - transform.position;
    direction += new Vector2(0, 0.5f);
    direction = direction.normalized; // ����ȭ

    // �Ѿ��� ���ư��� ������ �ٶ󺸵��� ȸ�� ����
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle); // Z�� ȸ��

  }*/



  void Update()
  {
    if(direction == Vector2.zero)
    {
      player = GameObject.FindWithTag("Player");
      direction = player.transform.position - transform.position;
      direction += new Vector2(0, 0.5f);
      direction = direction.normalized; // ����ȭ
                                        
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;    // ȸ�� ����
      transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    transform.position += (Vector3)(direction * speed * Time.deltaTime);
  }

  public void SetDirection(Vector2 dir)
  {
    direction = dir.normalized;

    // ȸ�� ����
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);
  }
}
