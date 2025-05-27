using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoranSward : Equipmentbase
{
    public float attackIncrease = 5f; // ���ݷ� ��·�
    public float defDecrease = 2f;  // ���� ����
    

    public override void ApplyEffect(PlayerController player)
    {
        player.attackDem += attackIncrease; //  ���ݷ� ����
        player.defence -= defDecrease;  // ���� ����                                    
    }
    public override void Enemydebuff(EnemyController enemy)
    {
    } // ��� ȿ�� ���� �޼���(�� �����)
}
