using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment2 : Equipmentbase
{
    public float defense = 10f; // 방어력
    public float speed = 10f; // 이동속도

    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.defence += defense; // 방어력 증가
        player.moveSpeed += speed; // 이동속도 증가
    }
    public override void Enemydebuff(EnemyController enemy)
    {
        
    }
}