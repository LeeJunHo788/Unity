using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // 오브젝트 풀의 크기
    public int poolSize = 10;

    // 오브젝트를 담을 배열
    GameObject[] enemyObjectPool;

    // 적을 생성할 위치 (SpawnPoint)
    public Transform[] spawnPoint;



    // 최소 시간
    float minTime = 1;

    // 최대 시간
    float maxTime = 1.5f;

    // 현재 시간
    float currentTime;

    // 일정 시간
    public float creatTime = 1;

    // 적 오브젝트
    public GameObject enemyPrefab;

    private void Start()
    {
        // 생성 과정에서 적의 생성 시간을 설정
        creatTime = Random.Range(minTime, maxTime);

        // 오브젝트 풀의 크기 지정
        enemyObjectPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            // 오브젝트 풀에 삽입할 객체를 정의한다
            GameObject enemy = Instantiate(enemyPrefab);
            // 객체를 삽입한다
            enemyObjectPool[i] = enemy;
            // 비활성화 처리
            enemy.SetActive(false);
        }
    }


    private void Update()
    {
        // 시간의 경과 저장
        currentTime += Time.deltaTime;

        // 일정 시간이 경과하면
        if(currentTime > creatTime)
        {
            // 오브젝트 풀 내부에 존재하는 비활성화된 객체 중에
            for (int i = 0; i < poolSize; i++)
            {
                // 하나를 골라 활성화시키고 화면에 랜덤 배치
                GameObject enemy = enemyObjectPool[i];
                if (enemy.activeSelf == false)
                {
                    enemy.SetActive(true);

                    // 배치 지점을 랜덤으로 결정
                    int index = Random.Range(0, spawnPoint.Length);
                    enemy.transform.position = spawnPoint[index].position;

                    // 처리가 완료되면 정지
                    break;
                }
            }

            // 현재 시점을 경과한 시간을 초기화 한다
            currentTime = 0;

            // 생성 과정에서 적의 생성 시간을 재설정
            creatTime = Random.Range(minTime, maxTime);
        }
    }

}
