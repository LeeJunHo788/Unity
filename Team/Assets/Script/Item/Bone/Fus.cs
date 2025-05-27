using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fus : MonoBehaviour
{
    public PlayerController pl;    
    EnemyController Enemy;
    BossController Boss;
    GameObject target;
    public GameObject Player;
    public Rigidbody2D rb;
    public DragonBone Db;
    public float fusdmg;
    

    private void Start()
    {
        Player = GameObject.Find("Player");
        pl = Player.GetComponent<PlayerController>();
        Db = GameObject.Find("Dragonbone").GetComponent<DragonBone>();
    }

    private void Update()
    {
        transform.position = Player.transform.position;
        fusdmg = Db.dmg;
        push();
    }

    public void push()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Player.transform.position, 4.0f);


        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))  // 태그 확인
            {
                // 적과 충돌시 정보를 받아오고
                Enemy = hit.transform.gameObject.GetComponent<EnemyController>();
                rb = hit.GetComponent<Rigidbody2D>();
                Enemy.HitEnemy(fusdmg);

                // 적을 좀 많이 밀어낸다
                Vector2 dir = (Enemy.transform.position - Player.transform.position).normalized;
                rb.AddForce(dir * 6f, ForceMode2D.Impulse);
            }
            if (hit.CompareTag("Boss"))
            {
                
                Enemy = hit.transform.gameObject.GetComponent<EnemyController>();
                Enemy.HitEnemy(fusdmg);
            }
        }
    }
    


    IEnumerator StopEnemy(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(1.0f);  // 1초 후에
        rb.velocity = Vector2.zero;             // 정지
    }
}
