using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : EnemyController
{
  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 2.2f; // �̵��ӵ�
    att = 8;     // ���ݷ�
    def = 30;     // ����
    hp = 20;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 5;   // ���� ����

    isFlyingEnemy = true;
  }

  protected override void Start()
  {
    currentHp = hp;


    base.Start();


  }

  protected override void Update()
  {
    base.Update();
  }
}
