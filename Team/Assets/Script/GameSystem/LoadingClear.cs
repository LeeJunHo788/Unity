using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingClear : MonoBehaviour
{
  void Start()
  {
    // 이름으로 오브젝트 찾기
    GameObject player = GameObject.Find("Player");
    GameObject ui = GameObject.Find("MainUI");

    // 있으면 삭제
    if (player != null) Destroy(player);         // 플레이어 삭제
    if (ui != null) Destroy(ui);                 // 메인 UI 삭제
  }
}
