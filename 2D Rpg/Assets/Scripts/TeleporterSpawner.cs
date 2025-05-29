using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSpawner : MonoBehaviour
{
  public GameObject teleporter; // �ڷ�����
  public Transform[] spawnPoints; // �ڷ����Ͱ� ������ �� �ִ� ��ġ�� (3��)

  void Start()
  {
    // 1~3 ��ġ �� �ϳ��� �����ϰ� �����մϴ�.
    int randomIndex = Random.Range(0, spawnPoints.Length);

    // �ڷ�������ġ ����                             
    teleporter.transform.position = spawnPoints[randomIndex].position;       
  }

}
