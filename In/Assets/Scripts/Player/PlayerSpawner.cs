using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
  public Transform spawnPoint; // ĳ���Ͱ� ������ ��ġ

  void Start()
  {
    if (CharaSelectData.selectedCharacterPrefab != null)
    {
      GameObject player = Instantiate(CharaSelectData.selectedCharacterPrefab, spawnPoint.position, Quaternion.identity);
      player.name = CharaSelectData.selectedCharacterPrefab.name;
    }
    else
    {
      Debug.LogWarning("���õ� ĳ���� �������� �����ϴ�!");
    }


    // DontDestroyOnLoad�� ����ִ� ���� �÷��̾� ã��
    GameObject originplayer = GameObject.FindWithTag("Player");

    if (originplayer != null)
    {
      // �÷��̾ spawnPoint ��ġ�� �̵�
      originplayer.transform.position = spawnPoint.position;
    }

    
    
  }
}
