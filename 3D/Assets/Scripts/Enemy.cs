using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // 이동 속도
    public float speed = 5;

    // 방향 벡터
    Vector3 dir;

    // 폭발 이펙트
    public GameObject explosionPrefab;

    private void Update()
    {
        // 이동 (등속 운동)
        transform.position += dir * speed * Time.deltaTime;
    }

    private void Start()
    {

        // 플레이어를 향해 이동하기 위한 녀석을 설정하기 위해 랜덤 돌리기
        int rndValue = Random.Range(0, 100);

        // 결과 값이 3보다 작을 때 플레이어를 향해 이동한다
        if (rndValue < 3)
        {
            GameObject target = GameObject.Find("Player");
            // 방향 구하기
            dir = target.transform.position - transform.position;   // 벡터 - 벡터 = 향하는 방향

            // dir 값이 1보다 크기 때문에 정규화
            dir.Normalize();

        }

        // 결과가 3보다 크면 아래로 이동한다
        else
        {
            dir = Vector3.down;
        }
    }

    // 인수 = 해당 스크립트를 가지고 있는 콜라이더와 부딪히는 콜라이더
    private void OnCollisionEnter(Collision collision)
    {
        // 적을 처치할 때마다 점수를 갱신한다
        ScoreManager.Instance.Score++;


        // 폭발 이펙트 생성
        GameObject explosion = Instantiate(explosionPrefab);
        
        // 폭발 이펙트를 발생시킨다
        explosion.transform.position = transform.position;
        // 폭발 이펙트 크기 변경
        explosion.transform.localScale *= 0.4f;   

        
        // 충돌 객체가 Bullet일떄
        if(collision.gameObject.name.Contains("Bullet"))
        {
            // Bullet 비활성화
            collision.gameObject.SetActive(false);
        }
        else
        {
            // 충돌한 객체 삭제
            Destroy(collision.gameObject);
        }

        // 비활성화
        gameObject.SetActive(false);

    }
}
 
