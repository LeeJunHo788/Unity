using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
  //로딩바 관련 변수
  public Image loadingBar;    //Fill타입 이미지
  public Text loadingText;    //퍼센트 텍스트
  public RectTransform loadingBarRect; //로딩바의 RectTransform

  //냥이 연출 변수
  public Transform catTransform;
  public Animator catAnimator;

  //임시지정 씬이름
  private string nextSceneName;

  private void Start()
  {
    DecideNextScene();

    catAnimator.SetTrigger("Jump"); //애니메이션 재생

    StartCoroutine(TransitionNextScene());
  }

  void DecideNextScene()
  {
    // Title에서 StageSelect로 가는 전용 분기
    if (PlayerPrefs.HasKey("NextScene"))
    {
      nextSceneName = PlayerPrefs.GetString("NextScene");
      PlayerPrefs.DeleteKey("NextScene");  // 다음번 방지용
      return;
    }

    //저장된적없으면 Stgae1_Junho가 기본값으로 적용
    string savedStage = PlayerPrefs.GetString("NextStage", "Stage01");
    Debug.Log("SavedStage:" + savedStage);

    //값에 따라 분기
    if (savedStage == "Stage01")
      nextSceneName = "Stage1";
    else if (savedStage == "Stage02")
      nextSceneName = "Stage2";
    else
    {
      Debug.LogWarning("잘못된 NextStage값 :" + savedStage);
      nextSceneName = "Stage1";
      PlayerPrefs.SetString("NextStage", "Stage01"); //방어코드
    }
  }
  IEnumerator TransitionNextScene()
  { 
    AsyncOperation ao = SceneManager.LoadSceneAsync(nextSceneName); //비동기방식
    ao.allowSceneActivation = false;

    float timer = 0f;
    float fakeProgress = 0f;
    float minLoadingTime = 3.0f; //최소3초 로딩시간 지속
    float barWidth = loadingBarRect.rect.width;

    while(!ao.isDone)
    {
      timer += Time.deltaTime;

      //가짜 progress bar
      if (timer < minLoadingTime)
      {
        fakeProgress = timer / minLoadingTime;
      }
      else
      {
        fakeProgress = Mathf.Clamp01(ao.progress / 0.9f);
      }

      loadingBar.fillAmount = fakeProgress;
      loadingText.text = (fakeProgress * 100f).ToString("F0") + "% 완료";

      // 로딩바 기준 UI 위치 계산
      Vector3 uiStartPos = loadingBarRect.position - new Vector3(barWidth / 2f, 0f, 0f);
      Vector3 uiTargetPos = uiStartPos + new Vector3(barWidth * fakeProgress, 190f, 0f); 

      // UI 좌표>월드 좌표로 변환
      Vector3 worldPos = Camera.main.ScreenToWorldPoint(uiTargetPos);
      worldPos.z = 0f; // z 고정

      // 고양이 위치 이동
      catTransform.position = worldPos;

      if (ao.progress >= 0.9f && timer >= minLoadingTime)
      {
        ao.allowSceneActivation = true; //씬전환 시작
      }
      yield return null;
    }
  }
}
