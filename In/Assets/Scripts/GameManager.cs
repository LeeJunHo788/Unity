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
      SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ����
    }

    else
    {
      Destroy(gameObject);
    }

   
  }

  void Start()
  {
    // ���� �� �̸��� ���� �������� ��ȣ ��������
    SetStageByScene(SceneManager.GetActiveScene().name);

    StartCoroutine(ShowTipCanvas());

  }

  // ���� ����� �� ȣ��Ǵ� �Լ�
  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    SetStageByScene(scene.name);
  }

 

  // �� �̸��� ���� �������� ��ȣ �ڵ� ����
  private void SetStageByScene(string sceneName)
  {
    switch (sceneName)    {
      case "Stage1": StageNumber = 1; break;
      case "Stage2": StageNumber = 2; break;
      case "Stage3": StageNumber = 3; break;
      default: StageNumber = 0; break; // �⺻��
    }

  }

  IEnumerator ShowTipCanvas()
  {
    yield return new WaitForSeconds(3f);

    GameObject tipcanv = Instantiate(tipCanvas);
    CanvasGroup tipCanvasGroup = tipcanv.GetComponent<CanvasGroup>();

    // ���̵� ��
    yield return StartCoroutine(FadeCanvasGroup(tipCanvasGroup, 0, 1, 1f));

    yield return new WaitForSeconds(4.5f);

    // ���̵� �ƿ�
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
    SceneManager.sceneLoaded -= OnSceneLoaded; // �� �ε� �̺�Ʈ ���� ����
  }
}
