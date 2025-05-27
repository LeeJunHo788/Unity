using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // �̵� �ӵ�
    public float speed = 5;

    // ���� ����
    Vector3 dir;

    // ���� ����Ʈ
    public GameObject explosionPrefab;

    private void Update()
    {
        // �̵� (��� �)
        transform.position += dir * speed * Time.deltaTime;
    }

    private void Start()
    {

        // �÷��̾ ���� �̵��ϱ� ���� �༮�� �����ϱ� ���� ���� ������
        int rndValue = Random.Range(0, 100);

        // ��� ���� 3���� ���� �� �÷��̾ ���� �̵��Ѵ�
        if (rndValue < 3)
        {
            GameObject target = GameObject.Find("Player");
            // ���� ���ϱ�
            dir = target.transform.position - transform.position;   // ���� - ���� = ���ϴ� ����

            // dir ���� 1���� ũ�� ������ ����ȭ
            dir.Normalize();

        }

        // ����� 3���� ũ�� �Ʒ��� �̵��Ѵ�
        else
        {
            dir = Vector3.down;
        }
    }

    // �μ� = �ش� ��ũ��Ʈ�� ������ �ִ� �ݶ��̴��� �ε����� �ݶ��̴�
    private void OnCollisionEnter(Collision collision)
    {
        // ���� óġ�� ������ ������ �����Ѵ�
        ScoreManager.Instance.Score++;


        // ���� ����Ʈ ����
        GameObject explosion = Instantiate(explosionPrefab);
        
        // ���� ����Ʈ�� �߻���Ų��
        explosion.transform.position = transform.position;
        // ���� ����Ʈ ũ�� ����
        explosion.transform.localScale *= 0.4f;   

        
        // �浹 ��ü�� Bullet�ϋ�
        if(collision.gameObject.name.Contains("Bullet"))
        {
            // Bullet ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
        }
        else
        {
            // �浹�� ��ü ����
            Destroy(collision.gameObject);
        }

        // ��Ȱ��ȭ
        gameObject.SetActive(false);

    }
}
 
