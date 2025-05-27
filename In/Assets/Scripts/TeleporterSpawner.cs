using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSpawner : MonoBehaviour
{
  public GameObject teleporter; // 텔레포터
  public Transform[] spawnPoints; // 텔레포터가 스폰될 수 있는 위치들 (3개)

  void Start()
  {
    // 1~3 위치 중 하나를 랜덤하게 선택합니다.
    int randomIndex = Random.Range(0, spawnPoints.Length);

    // 텔레포터위치 변경                             
    teleporter.transform.position = spawnPoints[randomIndex].position;       
  }

}
