using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBomb : MonoBehaviour
{    
    public BombThrew Bomb;
    public float Dmg;    
    public Rigidbody2D rb;
    public SpriteRenderer sp;

    [Header("이동관련요소")]
    public Transform startPoint; // A 지점
    public Transform endPoint;   // B 지점
    public int bounceCount = 3;
    public float height = 2.0f;
    public float duration = 2.0f; // 전체 이동 시간

    [Header("폭발관련요소")]
    public GameObject bombAct;
    public GameObject lastbombAct;

    private void Start()
    {
        Bomb = GameObject.Find("BombThrew").GetComponent<BombThrew>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        startPoint = gameObject.transform;
        endPoint = Bomb.nearEnemy.transform;
        StartCoroutine(MoveWithBounces());
        if (startPoint.transform.position.x - endPoint.transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);     // 스프라이트 고정
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1);     // 스프라이트 반전
        }
    }

    private void Update()
    {
        
    }

    // 폭탄을 튕기는 코루틴
    IEnumerator MoveWithBounces()
    {
        Vector3 A = startPoint.position;
        Vector3 B = endPoint.position;

        // 튕기는 지점에 배열 생성
        Vector3[] points = new Vector3[bounceCount + 1];
        for (int i = 0; i <= bounceCount; i++)
        {
            float t = (float)i / bounceCount;
            points[i] = Vector3.Lerp(A, B, t);
        }
            float segmentTime = duration / bounceCount;
        

        for (int i = 0; i < bounceCount; i++)
        {
            yield return StartCoroutine(MoveParabola(points[i], points[i + 1], height, segmentTime));

            // 폭탄이 도착한 시점에서 폭발장판 생성
            if (i < bounceCount - 1)
            {
                if (bombAct != null)
                    Instantiate(bombAct, points[i + 1], Quaternion.identity);
            }
            else
            {
                if (lastbombAct != null)
                    Instantiate(lastbombAct, points[i + 1], Quaternion.identity);
            Destroy(gameObject);
            }
        }
    }

    // 폭탄 튕길 중간지점 계산
    IEnumerator MoveParabola(Vector3 start, Vector3 end, float height, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            // 포물선 계산: 선형 보간 + 높이 보정
            Vector3 mid = Vector3.Lerp(start, end, t);
            mid.y += height * Mathf.Sin(t * Mathf.PI); // 높이 제어
            transform.position = mid;           

            time += Time.deltaTime;
            yield return null;
        }
        transform.position = end;       
    }
}
