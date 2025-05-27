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

    // 5�� �� ����
    IEnumerator des()
    {        
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    // �浹�� ������ ����� ������Ʈ ����
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
