using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBlock : MonoBehaviour
{
  SwordMan swordman;
  GameObject player;


  void Start()
  {
    StartCoroutine(WaitForPlayerAndInit());
  }

  IEnumerator WaitForPlayerAndInit()
  {
    // 플레이어가 생성될 때까지 기다림
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }

    GameObject player = GameObject.FindWithTag("Player");
    swordman = player.GetComponent<SwordMan>();
  }

  void Update()
  {

    if (player == null)
      player = GameObject.FindWithTag("Player");

    // 플레이어의 방향에 따라 방패 위치 설정
    if (player.transform.rotation == Quaternion.Euler(0, 0, 0)
      )
    {
      transform.position = player.transform.position + new Vector3(0.1f, 0, 0);
    }

    else
    {
      transform.position = player.transform.position + new Vector3 (-0.15f, 0, 0);
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
    {
      swordman.BlockSucces();
    }
  }

}
