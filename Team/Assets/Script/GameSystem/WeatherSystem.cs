using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public EnemyController Enemy;
    public GameObject Player;
    public GameObject springEffect;
    public GameObject summerEffect;
    public GameObject fallEffect;
    public GameObject winterEffect;

    enum Weather { spring,summer,fall,winter}
    Weather W_state;

    // Weather와 WeatherSystem은 메인카메라 하위로 넣고 비활성화시키면 됩니다.

    void Start()
    {
        Enemy = GetComponent<EnemyController>();
        Player = GameObject.Find("Player");
        int a = Random.Range(0, 3);
        if (a == 0)
        {
            W_state = Weather.spring;
        }
        if (a == 1)
        {
            W_state = Weather.summer;
        }
        if (a == 2)
        {
            W_state = Weather.fall;
        }
        if (a == 3)
        {
            W_state = Weather.winter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (W_state)
        {
            case Weather.spring:
                springEffect.SetActive(true);
                break;

            case Weather.summer:
                summerEffect.SetActive(true);
                break;

            case Weather.fall:
                fallEffect.SetActive(true);
                break;

            case Weather.winter:
                winterEffect.SetActive(true);
                break;
        }
    }

    // 날씨 효과에 따른 몹 강화요소
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            switch (W_state)
            {
                case Weather.spring:
                    collision.transform.gameObject.GetComponent<EnemyController>().maxHp *= 1.5f;
                    break;

                case Weather.summer:
                    collision.transform.gameObject.GetComponent<EnemyController>().attackDem += 2;
                    break;

                case Weather.fall:
                    collision.transform.gameObject.GetComponent<EnemyController>().moveSpeed++;
                    break;

                case Weather.winter:
                    collision.transform.gameObject.GetComponent<EnemyController>().maxHp *= 2f;
                    collision.transform.gameObject.GetComponent<EnemyController>().attackDem++;
                    break;
            }
        }        
    }

}
