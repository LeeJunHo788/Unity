using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Equipmentbase
{
    public float attackrate = 2.0f;
    public float slow = 1.0f;
    public GameObject pl;

    private void Start()
    {
        pl = GameObject.Find("Player");
    }

    private void Update()
    {
        transform.position = pl.transform.position;
        Debuf();
    }

    public override void ApplyEffect(PlayerController player)
    {
        player.attackSpeed += attackrate;
    }
    public override void Enemydebuff(EnemyController enemy)
    {
         
    }

    public void Debuf()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyController>().moveSpeed = slow;
            }

            if (hit.CompareTag("Boss"))
            {
                return;
            }
        }
    }
}
