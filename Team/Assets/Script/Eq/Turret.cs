using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Equipmentbase
{
    public float attackInhance = 2.0f;

    public override void ApplyEffect(PlayerController player)
    {
        // 플레이어가 이동을 안하면 공격력 증가
        if(player.GetComponent<Rigidbody2D>().velocity == new Vector2(0,0))
        {
            player.attackDem *= attackInhance;
        }
        if (player.GetComponent<Rigidbody2D>().velocity != new Vector2(0, 0))
        {
            player.attackDem /= attackInhance;
        }
    }
    public override void Enemydebuff(EnemyController enemy)
    {
    } // 장비 효과 적용 메서드(적 디버프)
}
