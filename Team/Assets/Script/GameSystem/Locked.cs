using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locked : MonoBehaviour
{
    PlayerController pc;
    GameManager gm;


    // 이전 스테이지를 클리어하지 않았을 시 다음으로 넘어가지 못하게 막음
    private void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();

    //여기도 널방지코드추가
    GameObject gmObj = GameObject.Find("GameManager");
    if (gmObj != null)
    {
      gm = gmObj.GetComponent<GameManager>();
      if (gm == null)
        Debug.LogWarning("GameManager 컴포넌트가 없음");
    }
    else
    {
      Debug.LogWarning("GameManager 오브젝트를 찾을 수 없습니다");
    }
  }

    private void Update()
    {
        /*
        if (true)
        {
            gameObject.SetActive(false);    // 스테이지1 클리어증표가 있을 시 자물쇠 해제
        }
        */

      if(gm != null && gm.clearedStage1) //스테이지1 클리어 확인
       {
         gameObject.SetActive(false); //자물쇠해제
       }
    }
}
