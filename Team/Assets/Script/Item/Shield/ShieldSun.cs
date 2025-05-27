using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSun : MonoBehaviour
{
    public Shield Sd;
    public EnemyController Enemy;
    public GameObject Player;
    public bool isAttack = false;
    [Header("회전용")]
    private float currentAngle = 0f;    // 현재 각도    
    private Quaternion ini;
    private Quaternion iniClone;
    [Header("방패스펙")]
    public float dmg;
    public float attackrate;

    // Start is called before the first frame update
    void Start()
    {
        Sd = GameObject.Find("shield").GetComponent<Shield>();
        Player = GameObject.Find("Player");
        ini = transform.rotation;

        Vector3 offset = transform.position - Player.transform.position;
        currentAngle = Mathf.Atan2(offset.y, offset.x);
    }

    // Update is called once per frame
    void Update()
    {
        dmg = Sd.Dmg;
        attackrate = Sd.attackRate;
        // 시간에 따라 각도 증가
        currentAngle += attackrate * Mathf.Deg2Rad * Time.deltaTime;

        // 새로운 위치 계산
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * 2f;
        transform.position = Player.transform.position + offset;
        transform.rotation = ini;

        if (Sd.Level > 2&&!isAttack)
        {
            StartCoroutine(attack());
        }

    }

    // 방패에 닿으면 데미지
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy = collision.transform.gameObject.GetComponent<EnemyController>();
            Enemy.HitEnemy(dmg);
        }
        if (collision.CompareTag("Boss"))
        {
            Enemy = collision.transform.gameObject.GetComponent<EnemyController>();
            Enemy.HitEnemy(dmg);
        }
        // 공격이 부딪치면 삭제시킴
        if (collision.CompareTag("EnemyAttack"))
        {
            Destroy(collision.transform.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy = collision.transform.gameObject.GetComponent<EnemyController>();
            Enemy.HitEnemy(dmg);
        }
        if (collision.CompareTag("Boss"))
        {
            Enemy = collision.transform.gameObject.GetComponent<EnemyController>();
            Enemy.HitEnemy(dmg);
        }
        // 공격이 부딪치면 삭제시킴
        if (collision.CompareTag("EnemyAttack"))
        {
            Destroy(collision.transform.gameObject);
        }
    }

    IEnumerator attack()
    {
        isAttack = true;
        GameObject Sclone2 = Instantiate(gameObject, new Vector3(-transform.position.x, -transform.position.y, transform.position.z), Quaternion.Euler(0, 0, 0));
        Sclone2.GetComponent<ShieldSun>().isAttack = true;
        // 두번째 방패는 정반대방향에서 회전
        iniClone = Sclone2.transform.rotation;
        float mirroredAngle = currentAngle + Mathf.PI; // 180도 = π 라디안
        Vector3 mirrorOffset = new Vector3(Mathf.Cos(mirroredAngle), Mathf.Sin(mirroredAngle), 0) * 2f;
        Sclone2.transform.position = Player.transform.position + mirrorOffset;
        Sclone2.transform.rotation = iniClone;

        yield return null;
    }
}
