using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
  //�� Wave�� �� UI
  public GameObject readyPanel;      //5�ʽ��½ð��غ��г�
  public GameObject gameStartPanel; //���ӽ���UI
  public GameObject gameOverPanel; //���ӿ���UI
  public GameObject gameClearPanel;  //����Ŭ����UI

  //UI���� Ʈ�����Ǿִϸ��̼�(����ȿ��)
  public Animator readyAnimator;      //�����غ�
  public Animator gameClearAnimator; //����Ŭ����
  public Animator gameOverAnimator; //���ӿ���

  //���� �� ���� ���� �ִϸ�����
  public GameObject warningImage; //����̹���
  public Animator warningAnimator; //���ִϸ�����
  public GameObject background; //����ȭ�鿬��
  public Animator backgroundAnimator; //����ȭ�鿬��ִϸ�����

  private bool isTransitioning = false; //UIƮ������ �ߺ� ���� ������

  private void Awake()
  {
    isTransitioning = false;

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
      readyPanel.SetActive(true); //Ȱ��ȭ
    }

    if(readyAnimator != null)
    {
      readyAnimator.SetTrigger("ReadyTransition"); //���� �ִϸ��̼� ����
      isTransitioning = true; //�ߺ��������

      Invoke("OnTransitionEnd", 4.5f);
    }
  }
  public void ShowGameStartUI()
  {
    if (isTransitioning) return;
    if(gameStartPanel != null)
    {
      gameStartPanel.SetActive(true); //Ȱ��ȭ
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
      gameClearPanel.SetActive(true); //Ȱ��ȭ
      Debug.Log("Ȱ��ȭ");
    }
    if(gameClearAnimator != null)
    {
      gameClearAnimator.SetTrigger("GameClear"); //Ʈ������ �ִϸ��̼� ����
      StartCoroutine(DelayPause(2f)); //3���� �Ͻ�����
      isTransitioning = true; //�ߺ� ���� ����
    }
  }
  private IEnumerator DelayPause(float delay)
  {
    yield return new WaitForSeconds(delay);
    Time.timeScale = 0f; // ���� �Ͻ�����
  }

  public void ShowGameOverUI()
  {
    if (isTransitioning) return;
    if(gameOverPanel != null)
    {
      gameOverPanel.SetActive(true); //Ȱ��ȭ
      gameOverAnimator = gameOverPanel.GetComponent<Animator>();

      if (gameOverAnimator != null)
      {
        gameOverAnimator.SetTrigger("GameOverTransition"); //Ʈ������ �ִϸ��̼� ����
        isTransitioning = true; //�ߺ��������
      }
    }
  }

  public void OnTransitionEnd()
  {
    isTransitioning = false;
    SetAllPanelsInactive(); //���� ������ �ٸ� �г� ����
  }

  public void PlayBossIntro()
  {
    if (isTransitioning) return;

    isTransitioning = true;

    if(warningAnimator != null)
    {
      warningImage.SetActive(true);
      warningAnimator.SetTrigger("StartTransition"); //���ִϸ��̼� ���� ���

      //��� ���� ���� �� ���� ȭ�� ���� ����
      StartCoroutine(PlayBackground(1.5f));
      StartCoroutine(DisableAfterDelay(1.5f, warningImage));
      StartCoroutine(EndTransitionAfterDelay(3.0f));//isTransitioning = false;�ιٲ��ֱ�
      
    }
  }

  private IEnumerator PlayBackground(float delay)
  {
    yield return new WaitForSeconds(delay);

    if(backgroundAnimator != null)
    {
      background.SetActive(true);
      backgroundAnimator.SetTrigger("BackgroundTransition");

      //����ȭ�� ���� ������ ��Ȱ��ȭ
      StartCoroutine(DisableAfterDelay(1.5f, background));
    }
  }

  IEnumerator DisableAfterDelay(float delay, GameObject obj)
  {
    yield return new WaitForSeconds(delay);
    obj.SetActive(false); //��Ȱ��ȭ
  }
  private IEnumerator EndTransitionAfterDelay(float delay)
  {
    yield return new WaitForSeconds(delay);
    OnTransitionEnd(); 
  }

}
