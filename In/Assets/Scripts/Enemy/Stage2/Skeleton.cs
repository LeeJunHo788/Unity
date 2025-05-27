using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Skeleton : EnemyController
{
  protected override void Awake()
  {
    range = 50.0f; // Ž�� ����
    speed = 2.7f; // �̵��ӵ�
    att = 7;     // ���ݷ�
    def = 20;     // ����
    hp = 30;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 15;   // ���� ����
    spawnWeight = 10;
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
