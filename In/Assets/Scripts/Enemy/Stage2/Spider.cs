using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : EnemyController
{
  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 3.3f; // �̵��ӵ�
    att = 7;     // ���ݷ�
    def = 10;     // ����
    hp = 10;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 15;   // ���� ����
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
