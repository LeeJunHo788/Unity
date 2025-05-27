using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment6 : Equipmentbase
{
    public float increaseCriHIt = 0.5f; // 치명타데미지 증가
    public float decreaseAttackSpeed = 1f; // 공격속도 감소



    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.criticalDem = player.criticalDem + (player.criticalDem * increaseCriHIt); // 치명타확률 증가
        player.attackSpeed -= decreaseAttackSpeed; // 공격속도 감소
    }

    public override void Enemydebuff(EnemyController enemy)
    {
    }

}