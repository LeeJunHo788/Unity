using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Rat : EnemyController
{
  protected override void Awake()
  {
    range = 70f; // Ž�� ����
    speed = 2.5f; // �̵��ӵ�
    att = 3;     // ���ݷ�
    def = 0;     // ����
    hp = 10;      // ü��
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
