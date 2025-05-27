using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderController : MonoBehaviour
{
    PlayerController pl;

    // Start is called before the first frame update
    void Start()
    {
        pl = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ��輱�� ������ �׿�����
            collision.transform.gameObject.GetComponent<PlayerController>().TakeDamage(pl.maxHp,pl.defIgnore);
        }
    }
}
