using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // �Ѿ� ������Ʈ
    public GameObject bulletPrefab;

    // źâ�� ���� ������ �Ѿ��� ����
    public int poolSize = 10;

    // ������Ʈ Ǯ �迭
    GameObject[] bulletObjectPool;


    private void Start()
    {
        // ���ӿ��� ����� �Ѿ��� ������ �迭(������Ʈ Ǯ)�� �ʱ�ȭ �Ѵ�
        bulletObjectPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            // �迭�� ������ ��ü�� �ʱ�ȭ�Ѵ�
            GameObject bullet = Instantiate(bulletPrefab);

            // �迭�� ��ü�� �����Ѵ�
            bulletObjectPool[i] = bullet;

            // ���ݹ�ư�� ���� ������ Ȱ��ȭ�� �����̹Ƿ� ������ ��Ȱ��ȭ ���·� ��ȯ�Ѵ�
            bullet.SetActive(false);
        }
    }


    // �Ѿ��� ������ ��ġ
    public GameObject firePosition;


    private void Update()
    {
        // ����ڰ� Ư�� Ű�� �Է��� ��� �߻� ó���� �����Ѵ�
        if (Input.GetButtonDown("Fire1"))
        {
            // ������Ʈ Ǯ ���ο��� ��Ȱ��ȭ�� ź�� ã�´�
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = bulletObjectPool[i];
                if(bullet.activeSelf == false)
                {
                    // Ȱ��ȭ �� �߻�
                    bullet.SetActive(true);
                    bullet.transform.position = firePosition.transform.position;
                    // �߻簡 �Ϸ�Ǿ����Ƿ� �˻��� �ߴ��Ѵ�
                    break;
                }
            }
        }
    }
}
