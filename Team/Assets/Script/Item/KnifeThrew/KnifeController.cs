using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public LayerMask enemyLayer;    // �� ���̾�
    public ThrewKnife thn;
    Vector2 dir;                    // ���� ����
    float angle;                    // ���⿡ ���� ����    
    EnemyController enemy;
    public float dmg;
    public Rigidbody2D rb;
    public PlayerController player;
    GameObject nearEnemy;       // ���� ����� ��
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
        // �÷��̾� ���ݷ¿� ������ �������
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

    // �� Ž�� �޼���
    void FindEnemy()
    {

        float minDistance = Mathf.Infinity;   // �ּ� �Ÿ�

        nearEnemy = null;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 30f, enemyLayer);   // �÷��̾� �ֺ� Ž������ ���� �� Ž��

        foreach (var collider in hitColliders)
        {
            float dist = Vector2.Distance(transform.position, collider.transform.position);   // �Ÿ� Ȯ��

            if (dist < minDistance)
            {
                minDistance = dist;       // �ּ� �Ÿ� ����
                nearEnemy = collider.gameObject;
            }
        }

        if (nearEnemy != null)
        {
            dir = (nearEnemy.transform.position - transform.position).normalized;       // ���� ����
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);                         // ȸ�� ����

            if (dir.x < 0)    // ���� ���ʿ� �ִ� ���
            {
                transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);     // ��������Ʈ ����
                rb.AddForce(dir*7f, ForceMode2D.Impulse);
            }

            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);      // ��������Ʈ ����ȭ
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
