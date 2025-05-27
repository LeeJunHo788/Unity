using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Equipmentbase
{
    public float attackInhance = 2.0f;

    public override void ApplyEffect(PlayerController player)
    {
        // �÷��̾ �̵��� ���ϸ� ���ݷ� ����
        if(player.GetComponent<Rigidbody2D>().velocity == new Vector2(0,0))
        {
            player.attackDem *= attackInhance;
        }
        if (player.GetComponent<Rigidbody2D>().velocity != new Vector2(0, 0))
        {
            player.attackDem /= attackInhance;
        }
    }
    public override void Enemydebuff(EnemyController enemy)
    {
    } // ��� ȿ�� ���� �޼���(�� �����)
}
