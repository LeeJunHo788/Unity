using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 이동 속도
    public float speed = 5;

    private void Update()
    {
        // 위로 향하는 벡터 (방향 구하기)
        Vector3 dir = Vector3.up;
        // 등속운동을 바탕으로 하여 오브젝트 이동 (이동하기)
        transform.position += dir * speed * Time.deltaTime;

    }

    
}
