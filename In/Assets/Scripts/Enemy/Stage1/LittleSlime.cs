using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class LittleSlime : EnemyController
{
  protected override void Awake()
  {
    range = 70f; // Ž�� ����
    speed = 2.0f; // �̵��ӵ�
    att = 4;     // ���ݷ�
    def = 5;     // ����
    hp = 12;      // ü��
    exp = 10;     // ����ġ
    gold = 3;     // ��
    defIgn = 5;   // ���� ����
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
