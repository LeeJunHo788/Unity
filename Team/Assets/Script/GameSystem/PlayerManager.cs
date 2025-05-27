using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
  public static PlayerManager Instance;
  GameObject player;
  PlayerController pc;

  void Awake()
  {
    // �̱��� ����
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����

      // �� ��ȯ �̺�Ʈ ���
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject); // �ߺ� ����
    }

    if (player == null)
      player = GameObject.FindWithTag("Player");
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {

    if (scene.name == "StageSelect")    // �� �̸��� ����
    {
      // ���� �����ϴ� ��� Player �±� ������Ʈ ������
      GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

      foreach (GameObject p in players)
      {
        if (p.scene.name == "DontDestroyOnLoad")         
        {
          Destroy(p);                                    
        }
        else                                             
        {
          player = p;                                    
          pc = player.GetComponent<PlayerController>();
          pc?.StatInit();
        }
      }
    }

    else
    {
      GameObject newPlayer = GameObject.FindWithTag("Player");
      if (newPlayer != null)
      {
        if (player != null && newPlayer != player)
        {
          Destroy(newPlayer);                            
        }
        else
        {
          player = newPlayer;
          pc = player.GetComponent<PlayerController>();
        }
      }
    }
    
    if (scene.name == "LoadingScene")
    {
      SetActive(false); // �ε��������� �����
    }
    else
    {
      SetActive(true);  // ���Ӿ������� ���̱�
      pc?.StatInit();   // ���� �ʱ�ȭ
    }

  }

  public void SetActive(bool isActive)
  {

    if (player == null)
    {
      player = GameObject.FindWithTag("Player");
    }


    if (player != null)
    {
      player.SetActive(isActive);

      if (isActive)
      {
        pc = player.GetComponent<PlayerController>();
        if (pc != null)
          pc.InitializeMainUI();
      }

    }
  }
}
