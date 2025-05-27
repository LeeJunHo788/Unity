using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public LayerMask enemyLayer;    // 적 레이어
    public ThrewKnife thn;
    Vector2 dir;                    // 공격 방향
    float angle;                    // 방향에 따른 각도    
    EnemyController enemy;
    public float dmg;
    public Rigidbody2D rb;
    public PlayerController player;
    GameObject nearEnemy;       // 가장 가까운 적
    float a;
    float b;

    private void Start()
    {
        thn = GameObject.Find("knifeThrew").GetComponent<ThrewKnife>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        a = player.attackDem;
        b = thn.knifeDmg;
    }

    void Update()
    {
        // 플레이어 공격력에 나이프 계수곱셈
        dmg = a*b;       
    }  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindEnemy();            
        }
        if (collision.CompareTag("Enemy"))
        {
            enemy = collision.transform.gameObject.GetComponent<EnemyController>();
            enemy.HitEnemy(dmg);
            StartCoroutine(des());
        }
        if (collision.CompareTag("Boss"))
        {
            collision.transform.gameObject.GetComponent<BossController>().HitEnemy(dmg);

        }
    }

    // 적 탐지 메서드
    void FindEnemy()
    {

        float minDistance = Mathf.Infinity;   // 최소 거리

        nearEnemy = null;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 30f, enemyLayer);   // 플레이어 주변 탐지범위 내의 적 탐지

        foreach (var collider in hitColliders)
        {
            float dist = Vector2.Distance(transform.position, collider.transform.position);   // 거리 확인

            if (dist < minDistance)
            {
                minDistance = dist;       // 최소 거리 갱신
                nearEnemy = collider.gameObject;
            }
        }

        if (nearEnemy != null)
        {
            dir = (nearEnemy.transform.position - transform.position).normalized;       // 방향 설정
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);                         // 회전 설정

            if (dir.x < 0)    // 적이 왼쪽에 있는 경우
            {
                transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);     // 스프라이트 반전
                rb.AddForce(dir*7f, ForceMode2D.Impulse);
            }

            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);      // 스프라이트 정상화
                rb.AddForce(dir*7f, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator des()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
