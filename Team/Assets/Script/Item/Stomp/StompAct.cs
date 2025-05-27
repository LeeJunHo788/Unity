using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompAct : MonoBehaviour
{
    public EnemyController Enemy;
    public PlayerController player;
    public BossController bc;
    public float dmg;   
    Stomp stomp;
    public float atkRad;        // ���� ����
    public float attackRate = 0.7f;    // ƽ����
    public bool isAttack = false;
    public int howUpgrade;

    private void Start()
    {
        stomp = GameObject.Find("Stomp").GetComponent<Stomp>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();        
    }

    private void Update()
    {
        dmg = player.attackDem * stomp.stompDmg;    // ���ݷ�
        atkRad = stomp.attackRad;               // ���ݹ���
        switch (howUpgrade)
        {
            case 0:
                attackRate = 0.7f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            case 1:
                attackRate = 0.7f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                break;
            case 2:
                attackRate = 0.6f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                break;
            case 3:
                attackRate = 0.6f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                break;
            case 4:
                attackRate = 0.5f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                break;
            case 5:
                attackRate = 0.3f * (1 - (0.1f * player.attackSpeed));
                transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
                break;
        }
        attack();
    }

    public void attack()
    {
        if (!isAttack)
        {
            StartCoroutine(hitdmg());
        }
    }

    IEnumerator hitdmg()
    {
        isAttack = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, atkRad);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                // �浹�� ���� ������Ʈ ���׹� ��Ʈ�ѷ��� ��������
                Enemy = hit.transform.gameObject.GetComponent<EnemyController>();
                // ���������� ����
                Enemy.HitEnemy(dmg);
            }
            else if (hit.CompareTag("Boss"))
            {
                // �浹�� ���� ������Ʈ ���׹� ��Ʈ�ѷ��� ��������
                Enemy = hit.transform.gameObject.GetComponent<EnemyController>();
                // ���������� ����
                Enemy.HitEnemy(dmg);
            }
        }
        yield return new WaitForSeconds(attackRate);
        isAttack = false;
    }
}
