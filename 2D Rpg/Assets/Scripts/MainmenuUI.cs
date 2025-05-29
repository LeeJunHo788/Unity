using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainmenuUI : MonoBehaviour
{
  public GameObject canvasMainMenu;        // 1�� ĵ����
  public GameObject canvasCharacterSelect; // 2�� ĵ����

  public GameObject[] charaObj;            // ĳ���� �ִϸ��̼� ������Ʈ

  // �ʱ� ���õǾ��ִ� ��ư
  public GameObject startButton;

  public TextMeshProUGUI descriptionText; // �ϴ� �ؽ�Ʈ ����� ��

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
        // �˾��� ���������� �˾� �ݰ� PauseMenu�� ���ư�
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

    // ���� �޴� ���� ĳ���� ���� �ѱ�
    canvasMainMenu.SetActive(false);
    canvasCharacterSelect.SetActive(true);

    for (int i = 0; i < charaObj.Length; i++)
    {
      charaObj[i].SetActive(true);

    }

  }

  public void GoMain()
  {
    // ���� �޴� ���� ĳ���� ���� �ѱ�
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
    Debug.Log("���� ����"); // �����Ϳ����� ���� �� �ǹǷ� Ȯ�ο� �α�
  }

  public void OnClickSelectCharacter(GameObject charaPrefab)
  {
    CharaSelectData.selectedCharacterPrefab = charaPrefab;


    // PlayerPrefs�� ���� ���� �� �̸��� �ӽ� ����
    PlayerPrefs.SetString("NextScene", "Stage1");

    //�ε� �� ���� �ҷ�����
    SceneManager.LoadScene("LoadingScene");
  }

  // �˻� Ŭ�� �޼���
  public void OnClickSelectSwordMan(GameObject swordManPrefab)
  {
    SaveManager saveManager = FindObjectOfType<SaveManager>();
    if (saveManager != null && saveManager.IsCharacterUnlocked("SwordMan"))
    {
      // �˻� �رݵ� ���
      OnClickSelectCharacter(swordManPrefab);
    }
    else
    {
      // ���� �رݵ��� ���� ��� - �޽��� �г� Ȱ��ȭ
      descriptionText.text = "�ر� ���� : ���� 1�� Ŭ����";
      StartCoroutine(AnimateTextScale());
    }
  }


  // �ؽ�Ʈ�� Ŀ���ٰ� �پ��� ����
  private IEnumerator AnimateTextScale()
  {
    // ���� ũ��� �׻� ����
    descriptionText.transform.localScale = Vector3.one;


    Vector3 originalScale = descriptionText.transform.localScale;
    Vector3 targetScale = originalScale * 1.02f; // �ణ ũ��

    float duration = 0.075f;
    float timer = 0f;

    // Ŀ����
    while (timer < duration)
    {
      timer += Time.unscaledDeltaTime;
      float t = timer / duration;
      descriptionText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
      yield return null;
    }

    // �ٽ� �۾�����
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
    // PersistentObject ��ũ��Ʈ�� ���� ��� ������Ʈ�� ã�Ƽ�
    PersistentObject[] persistentObjects = FindObjectsOfType<PersistentObject>();

    foreach (PersistentObject obj in persistentObjects)
    {
      Destroy(obj.gameObject); // �ش� ������Ʈ ��ü�� ����
    }
  }
}