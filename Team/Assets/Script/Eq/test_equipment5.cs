using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment5 : Equipmentbase
{
    public float increaseCri = 0.33f; // 치명타확률증가
    public float decreaseAttack = 1f; // 공격력 감소



    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.criticalChance =player.criticalChance+(player.criticalChance* increaseCri); // 치명타확률 증가
        player.attackDem -= decreaseAttack; // 공격력 감소
    }

    public override void Enemydebuff(EnemyController enemy)
    {
    }

}