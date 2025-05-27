using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSun : MonoBehaviour
{
    public Shield Sd;
    public EnemyController Enemy;
    public GameObject Player;
    public bool isAttack = false;
    [Header("ȸ����")]
    private float currentAngle = 0f;    // ���� ����    
    private Quaternion ini;
    private Quaternion iniClone;
    [Header("���н���")]
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
        // �ð��� ���� ���� ����
        currentAngle += attackrate * Mathf.Deg2Rad * Time.deltaTime;

        // ���ο� ��ġ ���
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * 2f;
        transform.position = Player.transform.position + offset;
        transform.rotation = ini;

        if (Sd.Level > 2&&!isAttack)
        {
            StartCoroutine(attack());
        }

    }

    // ���п� ������ ������
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
        // ������ �ε�ġ�� ������Ŵ
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
        // ������ �ε�ġ�� ������Ŵ
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
        // �ι�° ���д� ���ݴ���⿡�� ȸ��
        iniClone = Sclone2.transform.rotation;
        float mirroredAngle = currentAngle + Mathf.PI; // 180�� = �� ����
        Vector3 mirrorOffset = new Vector3(Mathf.Cos(mirroredAngle), Mathf.Sin(mirroredAngle), 0) * 2f;
        Sclone2.transform.position = Player.transform.position + mirrorOffset;
        Sclone2.transform.rotation = iniClone;

        yield return null;
    }
}
