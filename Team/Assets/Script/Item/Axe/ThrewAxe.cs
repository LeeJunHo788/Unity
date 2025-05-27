using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrewAxe : MonoBehaviour
{
    // �ӽ� ���ݷ�
    public float Dmg;
    EnemyController enemy;
    PlayerController player;
    BossController bc;
    GameObject Axe;
    Axe AxeStat;

    void Start()
    {
        // �÷��̾ ã�� ���� �޾ƿ� �غ�
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        // ��ü�� ã�� ��ȭ��ġ �޾ƿ� �غ�
        Axe = GameObject.Find("Axe");
        AxeStat = Axe.GetComponent<Axe>();       
        // ��ȭ��ġ�� ���� ���ݷ� ��������
        switch (AxeStat.howUpgrade)
        {
            case 0:
                Dmg = 1.0f*player.attackDem;
                break;
            case 1:
                Dmg = 1.25f*player.attackDem;
                break;
            case 2:
                Dmg = 1.575f*player.attackDem;
                break;
            case 3:
                Dmg = 1.70f*player.attackDem;
                break;
            case 4:
                Dmg = 2.05f*player.attackDem;
                break;
            case 5:
                Dmg = 3.00f*player.attackDem;
                break;
        }

        // ����ü ��ü �ڷ�ƾ
        StartCoroutine(Destroy());      
    }

    // Update is called once per frame
    void Update()
    {
        // ȸ����Ű��
        transform.Rotate(new Vector3(0, 0, 1000f * Time.deltaTime));
        player.GetComponent<PlayerController>();
        // �÷��̾� ���ݷ°� �������� ��������
        Dmg = player.attackDem * AxeStat.axeDmg;
    }
  
    // 3.5�� �� ���ӿ�����Ʈ ����
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }

    // ���׹̿� �浹�Ͽ��� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // �浹ü�� ���׹� ��Ʈ�ѷ��� �޾ƿ���
            // �������� �ش�           
            collision.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);          
        }
        else if (collision.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);

        }
    }
}
