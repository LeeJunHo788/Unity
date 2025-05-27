using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomp : Itembase
{
    GameObject Enemy;
    GameObject Player;
    PlayerController pc;
    public StompAct[] Sact;
    public GameObject attackArea;
    public int howUpgrade;
    public string ItemInfo;
    public float stompDmg;
    bool isAttack = false;
    public float attackRate = 3.0f;
    public float attackRad = 0.5f;

    private void Start()
    {
        Player = GameObject.Find("Player");
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        howUpgrade = Level;

        if (!isAttack)
        {
            insight();
        }
            // 강화정도에 따른 공격속도 및 공격력 공격범위
            switch (howUpgrade)
        {
            case 0:
                stompDmg = 1f;
                attackRate = 3.0f*(1-(0.1f*pc.attackSpeed));
                attackRad = 0.5f;
                break;
            case 1:
                stompDmg = 1.50f;
                attackRate = 2.5f * (1 - (0.1f * pc.attackSpeed));
                attackRad = 0.8f;
                break;
            case 2:
                stompDmg = 1.775f;
                attackRate = 2.0f * (1 - (0.1f * pc.attackSpeed));
                attackRad = 1.0f;
                break;
            case 3:
                stompDmg = 2.00f;
                attackRate = 2.0f * (1 - (0.1f * pc.attackSpeed));
                attackRad = 1.5f;
                break;
            case 4:
                stompDmg = 2.525f;
                attackRate = 1.5f * (1 - (0.1f * pc.attackSpeed));
                attackRad = 1.8f;
                break;
            case 5:
                stompDmg = 3.00f;
                attackRate = 1.0f * (1 - (0.1f * pc.attackSpeed));
                attackRad = 3.0f;
                break;
        }
    }
    // 적을 인식한다
    public void insight()
    {
        Collider2D hit = Physics2D.OverlapCircle(Player.transform.position, 3.0f);

        if (hit.CompareTag("Enemy"))
        {                        
             StartCoroutine(bo());                      
        }
        if (hit.CompareTag("Boss"))
        {
            StartCoroutine(bo());
        }
        if (hit.CompareTag("GameController"))
        {
             StartCoroutine(bo());                      
        }
    }

    // 공격 오브젝트를 통제하는 코루틴
    IEnumerator bo()
    {
        isAttack = true;
        attackArea.SetActive(true);
        for(int i = 0; i < Sact.Length; i++)
        {
            Sact[i].GetComponent<StompAct>().isAttack = false;
        }
        yield return new WaitForSeconds(3.0f);
        attackArea.SetActive(false);
        yield return new WaitForSeconds(attackRate);
        isAttack = false;
    }

}
