using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearStage1UI : MonoBehaviour
{
  public GameObject clearUI;  //무한모드 해금 UI
  GameManager gm;
  private AudioSource audioSource; //효과음용 변수 

  //게임 켜는 동안은 저장하나, 게임 종료되면 리셋되도록
  private static bool hasOpen = false;

  private void Start()
  {
    //게임 매니저 널방지 코드 추가
    GameObject gmObj = GameObject.Find("GameManager");

    if(gmObj != null)
    {
      gm = gmObj.GetComponent<GameManager>();
      if (gm == null)
        Debug.Log("GameManager컴포넌트가 없음");
    }
    else
    {
      Debug.Log("게임매니저 오브젝트를 찾을 수 없음");
    }
      audioSource = GetComponent<AudioSource>(); //오디오매니저찾기
  }

  private void Update()
  {
    if (hasOpen) return;

    if(gm!= null && gm.clearedStage1)
    {
      clearUI.SetActive(true); //스테이지1클리어시 해금UI활성화
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.buttonClicked);
      }
      else
      {
        Debug.LogWarning("효과음 재생 실패 셋중하나가 null");
      }
      hasOpen = true;
      Debug.Log("static으로 져-쟝!");
    }
  }

  //버튼에 연결할 함수
  public void CloseClearPanel()
  {
    clearUI.SetActive(false); //비활성화
  }
}
