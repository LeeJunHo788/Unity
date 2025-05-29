using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  public int StageNumber;

  public GameObject tipCanvas;

  void Awake()
  {
    if(Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
    }

    else
    {
      Destroy(gameObject);
    }

   
  }

  void Start()
  {
    // 현재 씬 이름에 따라 스테이지 번호 가져오기
    SetStageByScene(SceneManager.GetActiveScene().name);

    StartCoroutine(ShowTipCanvas());

  }

  // 씬이 변경될 때 호출되는 함수
  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    SetStageByScene(scene.name);

    if(scene.name != "EndingScene")
    StartCoroutine(ShowTipCanvas());
  }

 

  // 씬 이름에 따라 스테이지 번호 자동 설정
  private void SetStageByScene(string sceneName)
  {
    switch (sceneName)    {
      case "Stage1": StageNumber = 1; break;
      case "Stage2": StageNumber = 2; break;
      case "Stage3": StageNumber = 3; break;
      default: StageNumber = 0; break; // 기본값
    }

  }

  IEnumerator ShowTipCanvas()
  {
    yield return new WaitForSeconds(3f);

    GameObject tipcanv = Instantiate(tipCanvas);
    CanvasGroup tipCanvasGroup = tipcanv.GetComponent<CanvasGroup>();

    // 페이드 인
    yield return StartCoroutine(FadeCanvasGroup(tipCanvasGroup, 0, 1, 1f));

    yield return new WaitForSeconds(4.5f);

    // 페이드 아웃
    yield return StartCoroutine(FadeCanvasGroup(tipCanvasGroup, 1, 0, 1f));

    Destroy(tipcanv);
  }

  IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
  {
    float elapsed = 0f;
    while (elapsed < duration)
    {
      cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
      elapsed += Time.deltaTime;
      yield return null;
    }
    cg.alpha = end;
  }

  void OnDestroy()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 구독 해제
  }
}
