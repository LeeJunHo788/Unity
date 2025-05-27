using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots : Equipmentbase
{
    public float movespeed = 0.5f;
    public float attackspeed = 2.0f;


    public override void ApplyEffect(PlayerController player)
    {
        player.moveSpeed -= movespeed;
        player.attackSpeed *= attackspeed;
    }
    public override void Enemydebuff(EnemyController enemy)
    {
    } // 장비 효과 적용 메서드(적 디버프)
}
