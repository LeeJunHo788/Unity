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
    // 싱글톤 패턴
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // 씬 전환 시 유지

      // 씬 전환 이벤트 등록
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject); // 중복 제거
    }

    if (player == null)
      player = GameObject.FindWithTag("Player");
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {

    if (scene.name == "StageSelect")    // 씬 이름이 조건
    {
      // 씬에 존재하는 모든 Player 태그 오브젝트 모으기
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
      SetActive(false); // 로딩씬에서는 숨기기
    }
    else
    {
      SetActive(true);  // 게임씬에서는 보이기
      pc?.StatInit();   // 스탯 초기화
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
