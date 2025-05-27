using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // ������Ʈ Ǯ�� ũ��
    public int poolSize = 10;

    // ������Ʈ�� ���� �迭
    GameObject[] enemyObjectPool;

    // ���� ������ ��ġ (SpawnPoint)
    public Transform[] spawnPoint;



    // �ּ� �ð�
    float minTime = 1;

    // �ִ� �ð�
    float maxTime = 1.5f;

    // ���� �ð�
    float currentTime;

    // ���� �ð�
    public float creatTime = 1;

    // �� ������Ʈ
    public GameObject enemyPrefab;

    private void Start()
    {
        // ���� �������� ���� ���� �ð��� ����
        creatTime = Random.Range(minTime, maxTime);

        // ������Ʈ Ǯ�� ũ�� ����
        enemyObjectPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            // ������Ʈ Ǯ�� ������ ��ü�� �����Ѵ�
            GameObject enemy = Instantiate(enemyPrefab);
            // ��ü�� �����Ѵ�
            enemyObjectPool[i] = enemy;
            // ��Ȱ��ȭ ó��
            enemy.SetActive(false);
        }
    }


    private void Update()
    {
        // �ð��� ��� ����
        currentTime += Time.deltaTime;

        // ���� �ð��� ����ϸ�
        if(currentTime > creatTime)
        {
            // ������Ʈ Ǯ ���ο� �����ϴ� ��Ȱ��ȭ�� ��ü �߿�
            for (int i = 0; i < poolSize; i++)
            {
                // �ϳ��� ��� Ȱ��ȭ��Ű�� ȭ�鿡 ���� ��ġ
                GameObject enemy = enemyObjectPool[i];
                if (enemy.activeSelf == false)
                {
                    enemy.SetActive(true);

                    // ��ġ ������ �������� ����
                    int index = Random.Range(0, spawnPoint.Length);
                    enemy.transform.position = spawnPoint[index].position;

                    // ó���� �Ϸ�Ǹ� ����
                    break;
                }
            }

            // ���� ������ ����� �ð��� �ʱ�ȭ �Ѵ�
            currentTime = 0;

            // ���� �������� ���� ���� �ð��� �缳��
            creatTime = Random.Range(minTime, maxTime);
        }
    }

}
