using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_equipment3 : Equipmentbase
{
    public float decreaseDefence = 3f; // 방어력 감소수치    
    

    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
        player.defIgnore += decreaseDefence; // 방어력 감소
    }
    public override void Enemydebuff(EnemyController enemy)
    {
        
    }

}