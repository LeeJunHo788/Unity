using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
  public Transform spawnPoint; // 캐릭터가 생성될 위치

  void Start()
  {
    if (CharaSelectData.selectedCharacterPrefab != null)
    {
      GameObject player = Instantiate(CharaSelectData.selectedCharacterPrefab, spawnPoint.position, Quaternion.identity);
      player.name = CharaSelectData.selectedCharacterPrefab.name;
    }
    else
    {
      Debug.LogWarning("선택된 캐릭터 프리팹이 없습니다!");
    }


    // DontDestroyOnLoad로 살아있는 기존 플레이어 찾기
    GameObject originplayer = GameObject.FindWithTag("Player");

    if (originplayer != null)
    {
      // 플레이어를 spawnPoint 위치로 이동
      originplayer.transform.position = spawnPoint.position;
    }

    
    
  }
}
