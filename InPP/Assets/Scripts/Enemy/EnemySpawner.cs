using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public GameObject[] Wave1Enemy;  // 1���̺�
  public GameObject[] Wave2Enemy;  // 2���̺�
  public GameObject[] Wave3Enemy;  // 3���̺�


  GameObject[] enemy;   // ���� ���̺� ������ ��
  GameObject[][] enemyPool;


  bool isSpawning;  // ��ȯ�� ����
  public float minTime;    // ���� �ּ� ��Ÿ��
  public float maxTime;    // ���� �ִ� ��Ÿ��

  Collider2D[] cols; // ������ �ݶ��̴��� ��� �迭
  public float range = 30;   // �÷��̾� Ž�� ����
  public float stopSpawnDistance = 5f;  // ���� ���� ����
  bool isDetect = false;      // ���� �� �÷��̾� ���� ����
  float distanceToPlayer;

  GameObject tp;    // �ڷ����� ������Ʈ
  TeleporterManager tpManager; // �ڷ����͸Ŵ��� 

  int waveNum;   // ���̺� ��ȣ

  void Awake()
  {
    
  }

  private void Start()
  {

    waveNum = 1;

    // �迭 �ʱ�ȭ
    enemyPool = new GameObject[][] { Wave1Enemy, Wave2Enemy, Wave3Enemy };

    // ���̺꿡 �´� �� ���� �迭 ����
    enemy = enemyPool[waveNum -1];

    isDetect = false;
    isSpawning = false;

    minTime = 7;
    maxTime = 15;


    tp = GameObject.Find("Teleporter");
    tpManager = tp.GetComponent<TeleporterManager>();

  }

  private void Update()
  {
    // �߽����� ���� ���� ���� �ִ� �ݶ��̴�
    cols = Physics2D.OverlapCircleAll(transform.position, range);

    // ������ �ݶ��̴� �˻� ����
    foreach (Collider2D col in cols)
    {
      // �ݶ��̴��� �±װ� Player���
      if (col != null && col.CompareTag("Player"))
      {
        distanceToPlayer = Vector2.Distance(transform.position, col.transform.position);

        // �ʹ� ������ �����ص� �������� �ʵ��� ǥ��
        if (distanceToPlayer >= stopSpawnDistance)
        {
          isDetect = true;
        }
        break;
      }
    }

    // �÷��̾ ���� ���� ������ ������ Ȱ��ȭ
    if (isDetect)
    {
      StartCoroutine(SpawnEnemy());
    }

  }

  IEnumerator SpawnEnemy()
  {

    // ������ ������ �÷��̾ ���� ���� ������ Ȱ��ȭ
    while (isDetect && !isSpawning)
    {

      // �غ� ���̰ų� ������ ��ȯ�Ǿ����� ���� �ߴ�
      if (tpManager.isPreparing || tpManager.isBossSpawn || distanceToPlayer <= stopSpawnDistance)
      {
        yield break;
      }

      isSpawning = true;

      // �����ð� ��� ��
      float spawnTime = Random.Range(minTime, maxTime);
      yield return new WaitForSeconds(spawnTime);


      // �ڷ����Ͱ� �غ����̶�� ��ȯ �Ұ���
      if (tpManager.isPreparing == true || distanceToPlayer <= stopSpawnDistance)
      {
        isSpawning = false;
        yield break;
      }

      // �ð� ���� �Ŀ��� �ٽ� üũ
      if (tpManager.isPreparing || tpManager.isBossSpawn || distanceToPlayer <= stopSpawnDistance)
      {
        isSpawning = false;
        yield break;
      }


      // ���� ��ȯ
      GameObject selectedEnemy = GetRandomEnemy(enemy);
      GameObject spawnEnemy = Instantiate(selectedEnemy, transform.position, Quaternion.identity);
      spawnEnemy.name = selectedEnemy.name;   // �̸� �ڿ� Clone ����
      isSpawning = false;
    }
  }

  GameObject GetRandomEnemy(GameObject[] enemyArray)
  {
    float totalWeight = 0f;

    foreach (GameObject obj in enemyArray)
    {
      EnemyController ec = obj.GetComponent<EnemyController>();
      if (ec != null)
      {
        totalWeight += ec.spawnWeight;
      }
    }

    float randomValue = Random.Range(0f, totalWeight);
    float currentWeight = 0f;

    foreach (GameObject obj in enemyArray)
    {
      EnemyController ec = obj.GetComponent<EnemyController>();
      if (ec != null)
      {
        currentWeight += ec.spawnWeight;
        if (randomValue <= currentWeight)
        {
          return obj;
        }
      }
    }

    return enemyArray[0]; // fallback
  }



  // Ž�� ������ �����ִ� �޼���
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, stopSpawnDistance);
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, range);
  }

  // ���̺� ����� ��ȯ Ǯ ������Ʈ
  public void UpdateEnemyPool(int wave)
  {
    waveNum = wave; // ���� �ڷ������� ���̺� ��ȣ�� ��ġ��Ŵ

    if (waveNum < 4)
    {
      enemy = enemyPool[waveNum - 1];

    }
  }

  // ������Ʈ Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  private void OnEnable()
  {
    TeleporterManager.GoNextWave += UpdateEnemyPool;
  }

  // ������Ʈ ��Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  private void OnDisable()
  {
    TeleporterManager.GoNextWave -= UpdateEnemyPool;
  }

}
