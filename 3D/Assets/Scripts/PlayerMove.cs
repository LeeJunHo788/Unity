using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도
    public float speed = 5;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        print("H : " + h + ", V : " + v);

        // 벡터의 더하기
        Vector3 dir = new Vector3(h, v, 0);

        // P = P0 + vt 의 등속운동 공식을 사용하여 작성
        // transform.Translate(dir * speed + Time.deltaTime);
        // transform.position = transform.position + dir * speed * Time.deltaTime;
        transform.position += dir * speed * Time.deltaTime;
    }
}
