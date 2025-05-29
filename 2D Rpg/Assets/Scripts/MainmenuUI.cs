using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainmenuUI : MonoBehaviour
{
  public GameObject canvasMainMenu;        // 1번 캔버스
  public GameObject canvasCharacterSelect; // 2번 캔버스

  public GameObject[] charaObj;            // 캐릭터 애니메이션 오브젝트

  // 초기 선택되어있는 버튼
  public GameObject startButton;

  public TextMeshProUGUI descriptionText; // 하단 텍스트 출력할 곳

  void Start()
  {
    canvasMainMenu.SetActive(true);

    EventSystem.current.SetSelectedGameObject(null);
    EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    DestroyAllPersistentObjects(); 

  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {

      if (canvasMainMenu.activeSelf)
      {
        // 팝업이 켜져있으면 팝업 닫고 PauseMenu로 돌아감
        OnClickQuitGame();
      }

      else if (canvasCharacterSelect.activeSelf)
      {
        GoMain();
      }
    }
  }

  public void OnClickStartGame()
  {
    EventSystem.current.SetSelectedGameObject(null);

    // 메인 메뉴 끄고 캐릭터 선택 켜기
    canvasMainMenu.SetActive(false);
    canvasCharacterSelect.SetActive(true);

    for (int i = 0; i < charaObj.Length; i++)
    {
      charaObj[i].SetActive(true);

    }

  }

  public void GoMain()
  {
    // 메인 메뉴 끄고 캐릭터 선택 켜기
    canvasMainMenu.SetActive(true);
    canvasCharacterSelect.SetActive(false);
    for (int i = 0; i < charaObj.Length; i++)
    {
      charaObj[i].SetActive(false);

    }
  }

  public void OnClickQuitGame()
  {
    Application.Quit();
    Debug.Log("게임 종료"); // 에디터에서는 종료 안 되므로 확인용 로그
  }

  public void OnClickSelectCharacter(GameObject charaPrefab)
  {
    CharaSelectData.selectedCharacterPrefab = charaPrefab;


    // PlayerPrefs를 통해 다음 씬 이름을 임시 저장
    PlayerPrefs.SetString("NextScene", "Stage1");

    //로딩 씬 먼저 불러오기
    SceneManager.LoadScene("LoadingScene");
  }

  // 검사 클릭 메서드
  public void OnClickSelectSwordMan(GameObject swordManPrefab)
  {
    SaveManager saveManager = FindObjectOfType<SaveManager>();
    if (saveManager != null && saveManager.IsCharacterUnlocked("SwordMan"))
    {
      // 검사 해금된 경우
      OnClickSelectCharacter(swordManPrefab);
    }
    else
    {
      // 아직 해금되지 않은 경우 - 메시지 패널 활성화
      descriptionText.text = "해금 조건 : 게임 1번 클리어";
      StartCoroutine(AnimateTextScale());
    }
  }


  // 텍스트가 커졌다가 줄어드는 연출
  private IEnumerator AnimateTextScale()
  {
    // 원래 크기로 항상 고정
    descriptionText.transform.localScale = Vector3.one;


    Vector3 originalScale = descriptionText.transform.localScale;
    Vector3 targetScale = originalScale * 1.02f; // 약간 크게

    float duration = 0.075f;
    float timer = 0f;

    // 커지기
    while (timer < duration)
    {
      timer += Time.unscaledDeltaTime;
      float t = timer / duration;
      descriptionText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
      yield return null;
    }

    // 다시 작아지기
    timer = 0f;
    while (timer < duration)
    {
      timer += Time.unscaledDeltaTime;
      float t = timer / duration;
      descriptionText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
      yield return null;
    }

    descriptionText.transform.localScale = originalScale;
  }

  public void DestroyAllPersistentObjects()
  {
    // PersistentObject 스크립트를 가진 모든 오브젝트를 찾아서
    PersistentObject[] persistentObjects = FindObjectsOfType<PersistentObject>();

    foreach (PersistentObject obj in persistentObjects)
    {
      Destroy(obj.gameObject); // 해당 오브젝트 자체를 삭제
    }
  }
}