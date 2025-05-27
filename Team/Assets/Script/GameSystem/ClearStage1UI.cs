using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearStage1UI : MonoBehaviour
{
  public GameObject clearUI;  //무한모드 해금 UI
  GameManager gm;
  private AudioSource audioSource; //효과음용 변수 
  bool hasOpen = false;

  private void Start()
  {
    gm = GameObject.Find("GameManager").GetComponent<GameManager>(); //게임매니저찾기
    audioSource = GetComponent<AudioSource>(); //오디오매니저찾기
  }

  private void Update()
  {
    if (hasOpen) return;

    if(gm!= null && gm.clearedStage1)
    {
      clearUI.SetActive(true); //스테이지1클리어시 해금UI활성화
      //audioSource.PlayOneShot(SFXManager.instance.uiApear); //효과음
      hasOpen = true;
    }
  }

  //버튼에 연결할 함수
  public void CloseClearPanel()
  {
    clearUI.SetActive(false); //비활성화
  }
}
