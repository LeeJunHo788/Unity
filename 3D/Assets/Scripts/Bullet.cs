using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // �̵� �ӵ�
    public float speed = 5;

    private void Update()
    {
        // ���� ���ϴ� ���� (���� ���ϱ�)
        Vector3 dir = Vector3.up;
        // ��ӿ�� �������� �Ͽ� ������Ʈ �̵� (�̵��ϱ�)
        transform.position += dir * speed * Time.deltaTime;

    }

    
}
