using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBomb : MonoBehaviour
{
    public GameObject minibombAct;
    

    private void Start()
    {        
        StartCoroutine(des());
    }

    // 5초 후 삭제
    IEnumerator des()
    {        
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    // 충돌시 폭발을 남기고 오브젝트 삭제
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Instantiate(minibombAct, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Boss"))
        {
            Instantiate(minibombAct, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
