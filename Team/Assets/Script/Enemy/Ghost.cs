using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Blue
{
  protected override void Awake()
  {
    base.Awake();

    //Blue처럼 돌진형 적, 체력많고, 공격력 높게 설정
    maxHp *= 2f;
    currentHp = maxHp;
    moveSpeed *= 1.5f;
    attackDem *= 2f;
    defence *= 2f;
    defIgnore *= 2f;
  }
  protected override void Start()
  {
    base.Start();
  }
  protected override void Update()
  {
    base.Update();
  }
}
