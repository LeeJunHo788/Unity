using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // �ٸ� ��ü�� �浹���� ���
    private void OnTriggerEnter(Collider other)
    {
        // ��ü�� Bullet�� ���
        if(other.gameObject.name.Contains("Bullet") || other.gameObject.name.Contains("Enemy"))
        {
            // ������Ʈ Ǯ�� �ǵ��� �ֱ� ���غ�Ȱ��ȭ
            other.gameObject.SetActive(false);
        }
        
    }
}
