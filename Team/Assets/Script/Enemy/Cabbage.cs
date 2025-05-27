using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabbage : Red
{
  protected override void Awake()
  {
    base.Awake();

    maxHp *= 2f;
    currentHp = maxHp;
    moveSpeed *= 1.2f;
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
