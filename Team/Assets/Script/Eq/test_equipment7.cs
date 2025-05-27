using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/*
public class test_equipment7 : Equipmentbase
{/*
    public float slowtime = 2f; // 느려지는 시간
    public float slowdebuff = 0.5f; // 느려지는 비율
    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("장비가 플레이어에 추가되었습니다.");
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero; // 플레이어 기준 위치 초기화
    }
    public override void Enemydebuff(EnemyController enemy)
    {
        
        CircleCollider2D circle=gameObject.GetComponent<CircleCollider2D>();
        float radius = circle.radius*transform.lossyScale.x; // 적에게 적용할 범위
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy")); // 범위 내의 적들 가져오기
        
        foreach (Collider2D hit in colliders)
        {
            EnemyController enemys = hit.GetComponent<EnemyController>();
            if (enemys != null)
            {
                enemys.ApplyDebuff(EnemyController.debuffType.Slow, slowtime,slowdebuff); // 적에게 디버프 적용
            }
        }
}
    }*/