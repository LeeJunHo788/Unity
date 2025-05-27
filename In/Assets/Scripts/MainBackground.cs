using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBackground : MonoBehaviour
{
  public float scrollSpeed;     // �����̴� �ӵ�
  public float tilemapWidth;     // �ϳ��� ���� ����

  public Transform Back1;             // ù ��° 
  public Transform Back2;             // �� ��° 

  


  void Update()
  {
    // �������� �̵�
    Back1.position += Vector3.left * scrollSpeed * Time.deltaTime;
    Back2.position += Vector3.left * scrollSpeed * Time.deltaTime;

    if (Back1.position.x <= -tilemapWidth)
    {
      Back1.position = new Vector3(Back2.position.x + tilemapWidth, Back1.position.y, Back1.position.z);
    }

    if (Back2.position.x <= -tilemapWidth)
    {
      Back2.position = new Vector3(Back1.position.x + tilemapWidth, Back2.position.y, Back2.position.z);
    }
  }
}