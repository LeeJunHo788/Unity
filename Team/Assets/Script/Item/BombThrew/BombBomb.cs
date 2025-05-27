using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBomb : MonoBehaviour
{    
    public BombThrew Bomb;
    public float Dmg;    
    public Rigidbody2D rb;
    public SpriteRenderer sp;

    [Header("�̵����ÿ��")]
    public Transform startPoint; // A ����
    public Transform endPoint;   // B ����
    public int bounceCount = 3;
    public float height = 2.0f;
    public float duration = 2.0f; // ��ü �̵� �ð�

    [Header("���߰��ÿ��")]
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
            transform.localScale = new Vector3(1, 1, 1);     // ��������Ʈ ����
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1);     // ��������Ʈ ����
        }
    }

    private void Update()
    {
        
    }

    // ��ź�� ƨ��� �ڷ�ƾ
    IEnumerator MoveWithBounces()
    {
        Vector3 A = startPoint.position;
        Vector3 B = endPoint.position;

        // ƨ��� ������ �迭 ����
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

            // ��ź�� ������ �������� �������� ����
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

    // ��ź ƨ�� �߰����� ���
    IEnumerator MoveParabola(Vector3 start, Vector3 end, float height, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            // ������ ���: ���� ���� + ���� ����
            Vector3 mid = Vector3.Lerp(start, end, t);
            mid.y += height * Mathf.Sin(t * Mathf.PI); // ���� ����
            transform.position = mid;           

            time += Time.deltaTime;
            yield return null;
        }
        transform.position = end;       
    }
}
