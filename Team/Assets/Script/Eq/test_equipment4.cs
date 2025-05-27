using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment4 : Equipmentbase
{
    public float increaseSpeed = 0.5f; // 속도 증가 



    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.moveSpeed = player.moveSpeed+(player.moveSpeed*increaseSpeed); // 이동속도 증가
        player.attackSpeed = player.attackSpeed + (player.attackSpeed * increaseSpeed); // 공격속도 증가
        player.transform.localScale *= 0.77f; // 플레이어 크기 축소
    }

    public override void Enemydebuff(EnemyController enemy)
    {
    }

}