using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
  GameObject player;
  PlayerController pc;

  public float speed = 15f;
  public float lifeTime = 3f; // 검기 수명

  private Vector2 direction = Vector2.right; // 기본값 오른쪽으로 설정

  void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    if (player.transform.rotation.y == 0)
    {
      direction = Vector2.right;
    }

    else if (player.transform.rotation.y == 180)
    {
      direction = Vector2.left;
    }

    Destroy(gameObject, lifeTime);


  }

  void Update()
  {

    // 지정된 방향으로 이동 (검기 방향 설정)
    transform.Translate(direction * (speed * pc.attackSpeed) * Time.deltaTime);
  }

  

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
    {

      EnemyController ec = other.GetComponent<EnemyController>();
      BossController bc = other.GetComponent<BossController>();

      if (ec != null) 
      ec.StartCoroutine(ec.KnockBack(pc.att * 2.3f, other.transform.position - transform.position));

      else if (bc != null)
        bc.StartCoroutine(bc.KnockBack(pc.att * 2.3f, other.transform.position - transform.position));


    }

    
  }
}