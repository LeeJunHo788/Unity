using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Haloghost : EnemyController
{
  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 2.8f; // �̵��ӵ�
    att = 13;     // ���ݷ�
    def = 0;     // ����
    hp = 30;      // ü��
    exp = 20;     // ����ġ
    gold = 10;     // ��
    defIgn = 20;   // ���� ����

    spawnWeight = 7f;
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
