using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipmentbase : MonoBehaviour
{
    public int EquipmentIndex { get; set; } // 장비 인덱스
    public string equipmentName; // 장비 이름
    public string equipmentexplain; // 장비 설명
    public Sprite equipmentIcon; // 장비 아이콘

    public abstract void ApplyEffect(PlayerController player); // 장비 효과 적용 메서드(플레이어 버프)
    public abstract void Enemydebuff(EnemyController enemy); // 장비 효과 적용 메서드(적 디버프)
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
