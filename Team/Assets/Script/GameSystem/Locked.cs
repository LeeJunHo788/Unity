using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locked : MonoBehaviour
{
    PlayerController pc;
    GameManager gm;


    // ���� ���������� Ŭ�������� �ʾ��� �� �������� �Ѿ�� ���ϰ� ����
    private void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        /*
        if (true)
        {
            gameObject.SetActive(false);    // ��������1 Ŭ������ǥ�� ���� �� �ڹ��� ����
        }
        */

      if(gm != null && gm.clearedStage1) //��������1 Ŭ���� Ȯ��
       {
         gameObject.SetActive(false); //�ڹ�������
       }
    }
}
