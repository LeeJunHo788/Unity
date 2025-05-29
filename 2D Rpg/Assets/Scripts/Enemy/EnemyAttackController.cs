using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EnemyAttackController : MonoBehaviour
{
  public float att;    // 공격력
  public float defIgn;  // 방어력무시

  protected GameObject player;    // 플레이어 객체
  protected PlayerController pc;  // 플레이어 컨트롤러 스크립트
  protected float playerAtt;    // 플레이어 객체의 공격력
  protected float playerDefIgn; // 플레이어 객체의 방어력 무시


  protected virtual void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player");    // 플레이어 객체 가져오기
    pc = player.GetComponent<PlayerController>();
  }

  protected virtual void Update()
  {

  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    // 총알이 플레이어나 방패에 부딪힌 경우 삭제
    if(gameObject.CompareTag("EnemyBullet"))
    {
      if(collision.gameObject.CompareTag("Shield") || collision.gameObject.CompareTag("Player"))
      {
        Destroy(gameObject);
      }
    }
  }

}
