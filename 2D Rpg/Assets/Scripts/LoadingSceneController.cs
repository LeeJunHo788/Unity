using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
  GameObject player;
  Rigidbody2D rb;

  public TextMeshProUGUI tipText;          // 팁 텍스트
  public Slider loadingSlider;  // 로딩바
  float fakeProgress = 0f;

  string[] tips = new string[]
  {
    "몬스터도 방어력과 방어력 무시를 가지고 있습니다.",
    "최대 수치가 정해져있는 스탯이 있습니다.",
    "데미지 공식은 다음과 같습니다.\n최종데미지 = 공격력-( 공격력 * ( 방어력 - ( 방어력 * 방어력 무시%))%",
    "맵 어딘가에 있는 텔레포터를 가동시켜야 합니다.",
    "텔레포터가 가동되면 상점으로 다시 갈 수 없습니다.",
    "적을 처치하면 드문 확률로 아이템이 떨어집니다."

  };

  void Start()
  {
    ShowRandomTip(); // 시작할 때 팁 표시
    StartCoroutine(LoadNextScene());
  }

  void ShowRandomTip()
  {
    int index = Random.Range(0, tips.Length);
    tipText.text = tips[index];
  }

  IEnumerator LoadNextScene()
  {
    player = GameObject.FindWithTag("Player");
    if(player != null)
    {
      rb = player.GetComponent<Rigidbody2D>();
      rb.gravityScale = 0f;
    }

    // 1초 기다렸다가 로딩 시작 (애니메이션용 딜레이)
    yield return new WaitForSeconds(0.5f);

    // 프리팹에서 가져오기
    string nextSceneName = PlayerPrefs.GetString("NextScene");
    AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
    op.allowSceneActivation = false; // 씬 전환 비활성화

    // 실제 로딩은 0 ~ 0.9까지만 차오름
    while (fakeProgress < 0.9f)
    {
      fakeProgress += Time.deltaTime * 0.7f; // 천천히 증가 (속도 조절 가능)
      loadingSlider.value = fakeProgress;
      yield return null;
    }

    // 실제 로딩 완료될 때까지 대기
    while (op.progress < 0.9f)
    {
      yield return null;
    }

    // 0.9 넘었으면 나머지 0.1 천천히 올리기
    while (fakeProgress < 1f)
    {
      fakeProgress += Time.deltaTime * 0.3f;
      loadingSlider.value = fakeProgress;
      yield return null;
    }

    // 살짝 멈췄다가 씬 전환!
    yield return new WaitForSeconds(0.5f);
    op.allowSceneActivation = true;

    if (rb != null) 
    rb.gravityScale = 2f;


    /*
    // 로딩 완료될 때까지 기다리기
    while (!op.isDone)
    {
      yield return null;
    }*/
  }
}
