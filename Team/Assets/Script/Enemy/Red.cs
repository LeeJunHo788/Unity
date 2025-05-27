using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Red : EnemyController
{
  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    maxHp = 20f + sm.currentWave;
    currentHp = maxHp;
    moveSpeed = 3f;
    attackDem = 5f + sm.currentWave;
    defence = 15f + sm.currentWave;
    defIgnore = Mathf.Min(100f, 10f + sm.currentWave); // 최대 100으로 제한
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
   
