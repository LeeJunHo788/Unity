using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment : Equipmentbase
{
    public float attackIncrease = 5f; // 공격력 상승량
    public float attackSpeedDecrease = 0.5f; // 공격속도 감소량

    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.attackDem += attackIncrease; //  공격력 증가
        player.attackSpeed -= attackSpeedDecrease; // 공격속도 감소
    }
    public override void Enemydebuff(EnemyController enemy)
    {
        // 대체 적한테 어떻게 하면 디버프를 적용할 수 있을까?        
    }
}