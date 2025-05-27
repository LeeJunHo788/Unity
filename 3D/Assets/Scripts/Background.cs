using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // ��� �̹���
    public Material bgMaterial;

    // ��ũ�� �ӵ�
    public float scrollSpeed = 0.2f;


    private void Update()
    {
        // ��ũ�� ����
        Vector2 dir = Vector2.up;

        // ���׸��� ���Ե� �ؽ��� ��ũ��
        bgMaterial.mainTextureOffset += dir * scrollSpeed * Time.deltaTime;
    }
}
