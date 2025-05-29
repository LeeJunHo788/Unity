using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Ghost : EnemyController
{
  protected override void Awake()
  {
    range = 80.0f; // Ž�� ����
    speed = 2.3f; // �̵��ӵ�
    att = 8;     // ���ݷ�
    def = 0;     // ����
    hp = 25;      // ü��
    exp = 20;     // ����ġ
    gold = 10;     // ��
    defIgn = 20;   // ���� ����

    spawnWeight = 13f;

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
