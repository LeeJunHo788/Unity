using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // 다른 물체가 충돌했을 경우
    private void OnTriggerEnter(Collider other)
    {
        // 객체가 Bullet일 경우
        if(other.gameObject.name.Contains("Bullet") || other.gameObject.name.Contains("Enemy"))
        {
            // 오브젝트 풀에 되돌려 주기 위해비활성화
            other.gameObject.SetActive(false);
        }
        
    }
}
