using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EnemyAttackController : MonoBehaviour
{
  public float att;    // ���ݷ�
  public float defIgn;  // ���¹���

  protected GameObject player;    // �÷��̾� ��ü
  protected PlayerController pc;  // �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
  protected float playerAtt;    // �÷��̾� ��ü�� ���ݷ�
  protected float playerDefIgn; // �÷��̾� ��ü�� ���� ����


  protected virtual void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player");    // �÷��̾� ��ü ��������
    pc = player.GetComponent<PlayerController>();
  }

  protected virtual void Update()
  {

  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    // �Ѿ��� �÷��̾ ���п� �ε��� ��� ����
    if(gameObject.CompareTag("EnemyBullet"))
    {
      if(collision.gameObject.CompareTag("Shield") || collision.gameObject.CompareTag("Player"))
      {
        Destroy(gameObject);
      }
    }
  }

}
