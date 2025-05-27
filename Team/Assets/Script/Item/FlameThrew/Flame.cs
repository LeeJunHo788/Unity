using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public float attackRate;    // ȭ��ƽ��������
    public float Dmg;           // ������
    bool isAttack = false;
    FlameThrew fl;
    PlayerController Pc;


    // Start is called before the first frame update
    void Start()
    {
        fl = GameObject.Find("FlameThrew").GetComponent<FlameThrew>();
        Pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (fl.howUpgrade)
        {
            case 0:
                Dmg = Pc.attackDem * 1f;
                attackRate = 2.5f;
                break;
            case 1:
                Dmg = Pc.attackDem * 1.2f;
                attackRate = 2.5f;
                break;
            case 2:
                Dmg = Pc.attackDem * 1.5f;
                attackRate = 2.5f;
                break;
            case 3:
                Dmg = Pc.attackDem * 1.75f;
                attackRate = 1.5f;
                break;
            case 4:
                Dmg = Pc.attackDem * 2f;
                attackRate = 1.5f;
                break;
            case 5:
                Dmg = Pc.attackDem * 2.5f;
                attackRate = 1.0f;
                break;
        }

        if (!isAttack)
        {
            StartCoroutine(des());
        }
        StartCoroutine(troy());
    }

    // ����ó�� �ڷ�ƾ
    IEnumerator des()
    {
        isAttack = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);
            }
            if (hit.CompareTag("Boss"))
            {
                hit.transform.gameObject.GetComponent<BossController>().HitEnemy(Dmg);
            }
        }
        yield return new WaitForSeconds(attackRate);
        isAttack = false;        
    }  
    
    // �� ������Ʈ �����ڷ�ƾ
    IEnumerator troy()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
