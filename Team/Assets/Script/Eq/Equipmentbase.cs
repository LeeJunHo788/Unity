using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipmentbase : MonoBehaviour
{
    public int EquipmentIndex { get; set; } // ��� �ε���
    public string equipmentName; // ��� �̸�
    public string equipmentexplain; // ��� ����
    public Sprite equipmentIcon; // ��� ������

    public abstract void ApplyEffect(PlayerController player); // ��� ȿ�� ���� �޼���(�÷��̾� ����)
    public abstract void Enemydebuff(EnemyController enemy); // ��� ȿ�� ���� �޼���(�� �����)
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
