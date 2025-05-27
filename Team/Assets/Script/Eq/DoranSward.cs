using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoranSward : Equipmentbase
{
    public float attackIncrease = 5f; // 공격력 상승량
    public float defDecrease = 2f;  // 방어력 감소
    

    public override void ApplyEffect(PlayerController player)
    {
        player.attackDem += attackIncrease; //  공격력 증가
        player.defence -= defDecrease;  // 방어력 감소                                    
    }
    public override void Enemydebuff(EnemyController enemy)
    {
    } // 장비 효과 적용 메서드(적 디버프)
}
