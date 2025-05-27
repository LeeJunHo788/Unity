using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class test_equipment1 : Equipmentbase
{
    public float currentHpIncrease = 10f; // 체력 증가
    public float maxhpIncrease= 10f; // 최대 체력 증가

    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화        
        player.maxHp += maxhpIncrease; // 최대 체력 증가
        player.currentHp += currentHpIncrease; // 현재 체력 증가
        player.transform.localScale *= 1.5f; // 플레이어 크기 증가

    }
    public override void Enemydebuff(EnemyController enemy)
    {
       
    }
}