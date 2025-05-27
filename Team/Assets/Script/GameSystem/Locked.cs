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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
