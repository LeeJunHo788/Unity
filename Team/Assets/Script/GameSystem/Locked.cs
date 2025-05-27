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

    //���⵵ �ι����ڵ��߰�
    GameObject gmObj = GameObject.Find("GameManager");
    if (gmObj != null)
    {
      gm = gmObj.GetComponent<GameManager>();
      if (gm == null)
        Debug.LogWarning("GameManager ������Ʈ�� ����");
    }
    else
    {
      Debug.LogWarning("GameManager ������Ʈ�� ã�� �� �����ϴ�");
    }
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
