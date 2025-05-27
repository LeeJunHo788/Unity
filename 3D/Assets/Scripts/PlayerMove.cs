using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // �̵� �ӵ�
    public float speed = 5;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        print("H : " + h + ", V : " + v);

        // ������ ���ϱ�
        Vector3 dir = new Vector3(h, v, 0);

        // P = P0 + vt �� ��ӿ ������ ����Ͽ� �ۼ�
        // transform.Translate(dir * speed + Time.deltaTime);
        // transform.position = transform.position + dir * speed * Time.deltaTime;
        transform.position += dir * speed * Time.deltaTime;
    }
}
