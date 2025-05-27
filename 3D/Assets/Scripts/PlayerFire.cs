using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // 총알 오브젝트
    public GameObject bulletPrefab;

    // 탄창에 삽입 가능한 총알의 개수
    public int poolSize = 10;

    // 오브젝트 풀 배열
    GameObject[] bulletObjectPool;


    private void Start()
    {
        // 게임에서 사용할 총알을 보관할 배열(오브젝트 풀)을 초기화 한다
        bulletObjectPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            // 배열에 삽입할 객체를 초기화한다
            GameObject bullet = Instantiate(bulletPrefab);

            // 배열에 객체를 삽입한다
            bulletObjectPool[i] = bullet;

            // 공격버튼을 누를 때마다 활성화할 예정이므로 당장은 비활성화 상태로 전환한다
            bullet.SetActive(false);
        }
    }


    // 총알이 생성될 위치
    public GameObject firePosition;


    private void Update()
    {
        // 사용자가 특정 키를 입력한 경우 발사 처리를 진행한다
        if (Input.GetButtonDown("Fire1"))
        {
            // 오브젝트 풀 내부에서 비활성화된 탄을 찾는다
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = bulletObjectPool[i];
                if(bullet.activeSelf == false)
                {
                    // 활성화 후 발사
                    bullet.SetActive(true);
                    bullet.transform.position = firePosition.transform.position;
                    // 발사가 완료되었으므로 검색을 중단한다
                    break;
                }
            }
        }
    }
}
