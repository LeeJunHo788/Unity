using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
  //각 Wave에 들어갈 UI
  public GameObject readyPanel;      //5초쉬는시간준비패널
  public GameObject gameStartPanel; //게임시작UI
  public GameObject gameOverPanel; //게임오버UI
  public GameObject gameClearPanel;  //게임클리어UI

  //UI들의 트랜지션애니메이션(연출효과)
  public Animator readyAnimator;      //게임준비
  public Animator gameClearAnimator; //게임클리어
  public Animator gameOverAnimator; //게임오버

  //보스 전 진입 연출 애니메이터
  public GameObject warningImage; //경고이미지
  public Animator warningAnimator; //경고애니메이터
  public GameObject background; //검정화면연출
  public Animator backgroundAnimator; //검정화면연출애니메이터

  private bool isTransitioning = false; //UI트랜지션 중복 실행 방지용

  //효과음용 변수
  private AudioSource audioSource;

  private void Awake()
  {
    isTransitioning = false;
    audioSource = GetComponent<AudioSource>(); //오디오소스 컴포넌트 가져오기
    SetAllPanelsInactive();
  }
  private void SetAllPanelsInactive()
  {
    if (gameStartPanel != null) gameStartPanel.SetActive(false);
    if (gameClearPanel != null) gameClearPanel.SetActive(false);
    if (gameOverPanel != null) gameOverPanel.SetActive(false);
    if (readyPanel != null) readyPanel.SetActive(false);
  }
  public void ShowReadyPanel()
  {
    if (isTransitioning) return;
    readyAnimator = readyPanel.GetComponent<Animator>();
    if(readyPanel != null)
    {
      readyPanel.SetActive(true); //활성화
    }

    if(readyAnimator != null)
    {
      readyAnimator.SetTrigger("ReadyTransition"); //연출 애니메이션 실행
      isTransitioning = true; //중복실행방지

      Invoke("OnTransitionEnd", 4.5f);
    }
  }
  public void ShowGameStartUI()
  {
    if (isTransitioning) return;
    if(gameStartPanel != null)
    {
      gameStartPanel.SetActive(true); //활성화

      //효과음재생
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.uiAppear != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.uiAppear);
      }

      isTransitioning = true;

      Invoke("OnTransitionEnd", 1f);
    }
  }
  public void ShowGameClearUI()
  {
    if (isTransitioning) return;
  
    gameClearAnimator = gameClearPanel.GetComponent<Animator>();
    if(gameClearPanel!= null)
    {
      gameClearPanel.SetActive(true); //활성화
      Debug.Log("활성화");

      //효과음재생
      if(audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.buttonClicked);
      }
    }
    if(gameClearAnimator != null)
    {
      gameClearAnimator.SetTrigger("GameClear"); //트랜지션 애니메이션 실행
      StartCoroutine(DelayPause(2f)); //3초후 일시정지
      isTransitioning = true; //중복 실행 방지
    }
  }
  private IEnumerator DelayPause(float delay)
  {
    yield return new WaitForSeconds(delay);
    Time.timeScale = 0f; // 게임 일시정지
  }

  public void ShowGameOverUI()
  {
    if (isTransitioning) return;
    if(gameOverPanel != null)
    {
      gameOverPanel.SetActive(true); //활성화

      //효과음재생
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.gameOver != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.gameOver);
      }

      gameOverAnimator = gameOverPanel.GetComponent<Animator>();

      if (gameOverAnimator != null)
      {
        gameOverAnimator.SetTrigger("GameOverTransition"); //트랜지션 애니메이션 실행
        isTransitioning = true; //중복실행방지
      }
    }
  }

  public void OnTransitionEnd()
  {
    isTransitioning = false;
    SetAllPanelsInactive(); //연출 끝나면 다른 패널 끄기
  }

  public void PlayBossIntro()
  {
    if (isTransitioning) return;

    isTransitioning = true;

    if(warningAnimator != null)
    {
      warningImage.SetActive(true);
      
      //경고음 재생
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.bossTransition != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.bossTransition);
      }
      warningAnimator.SetTrigger("StartTransition"); //경고애니메이션 먼저 재생

      //경고 연출 끝난 후 검정 화면 연출 시작
      StartCoroutine(PlayBackground(1.5f));
      StartCoroutine(DisableAfterDelay(1.5f, warningImage));
      StartCoroutine(EndTransitionAfterDelay(3.0f));//isTransitioning = false;로바꿔주기
      
    }
  }

  private IEnumerator PlayBackground(float delay)
  {
    yield return new WaitForSeconds(delay);

    if(backgroundAnimator != null)
    {
      background.SetActive(true);
      backgroundAnimator.SetTrigger("BackgroundTransition");

      //검정화면 연출 끝나면 비활성화
      StartCoroutine(DisableAfterDelay(1.5f, background));
    }
  }

  IEnumerator DisableAfterDelay(float delay, GameObject obj)
  {
    yield return new WaitForSeconds(delay);
    obj.SetActive(false); //비활성화
  }
  private IEnumerator EndTransitionAfterDelay(float delay)
  {
    yield return new WaitForSeconds(delay);
    OnTransitionEnd(); 
  }

}
