using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventSystemObject : MonoBehaviour
{
  public static List<EventSystemObject> activeEvents = new List<EventSystemObject>();

  protected GameObject player;
  protected PlayerController playerc;

  public GameObject panel;
  public GameObject eventTimer;
  public GameObject eventName;
  public GameObject infoPanel;
  protected TextMeshProUGUI eventNameText;
  public GameObject rewardItem;
  public float interactionRange = 3f; // 상호작용 범위
  public float timeLimit = 30f;        // 제한 시간

  protected bool isPlayerInRange = false;
  protected bool eventInProgress = false;
  protected float timer = 0f;
  protected TextMeshProUGUI eventTimerText;

  protected virtual void Start()
  {
    player = GameObject.FindWithTag("Player");
    playerc = player.GetComponent<PlayerController>();
    eventNameText = eventName.GetComponent<TextMeshProUGUI>();
    panel.SetActive(false);

    if (eventTimer != null)
    {
      eventTimer.SetActive(false);
    }

    if (eventName != null)
    {
      eventName.SetActive(false);
    }

    if (infoPanel != null)
    {
      infoPanel.SetActive(false);
    }

    activeEvents.Add(this); // 리스트에 등록
  }

  protected virtual void Update()
  {
    if (player == null) return;

    if (eventInProgress)
    {
      timer += Time.deltaTime;

      if (eventTimerText != null)
      {
        float timeLeft = Mathf.Max(0, timeLimit - timer);
        eventTimerText.text = $"남은 시간: {timeLeft:F1}초";

        // 남은 시간 10초 이하일 때 색상 변경
        if (timeLeft <= 10f)
          eventTimerText.color = Color.red;
        else
          eventTimerText.color = Color.black;
      }

      // 시간 초과 체크
      if (timer >= timeLimit)
      {
        Fail();
      }
    }

    float distance = Vector3.Distance(transform.position, player.transform.position);

    if (distance <= interactionRange)
    {
      if (!isPlayerInRange)
      {
        panel.SetActive(true);
        isPlayerInRange = true;
      }

      if (!eventInProgress && Input.GetKeyDown(KeyCode.Space))
      {
        TriggerEvent();
      }
    }
    else
    {
      if (isPlayerInRange)
      {
        panel.SetActive(false);
        isPlayerInRange = false;
      }
    }
  }

  public virtual void TriggerEvent()
  {
    if (eventInProgress) return;
    eventInProgress = true;
    timer = 0f;

    if (infoPanel != null)
    {
      Transform mainUI = GameObject.Find("MainUI").transform;
      infoPanel.transform.SetParent(mainUI, false);

      RectTransform rtName = infoPanel.GetComponent<RectTransform>();
      if (rtName != null)
      {
        rtName.anchorMin = new Vector2(1f, 0.5f);
        rtName.anchorMax = new Vector2(1f, 0.5f);
        rtName.pivot = new Vector2(0.5f, 0.5f);
        rtName.anchoredPosition = new Vector2(-150f, 20f);
      }

      infoPanel.SetActive(true);
    } 

    if (eventTimer != null)
    {
      Transform mainUI = GameObject.Find("MainUI").transform;
      eventTimer.transform.SetParent(mainUI, false);

      RectTransform rt = eventTimer.GetComponent<RectTransform>();
      if (rt != null)
      {
        rt.anchorMin = new Vector2(1f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(-150f, 0f);
      }

      eventTimer.SetActive(true);
      eventTimerText = eventTimer.GetComponentInChildren<TextMeshProUGUI>();
    }

    if (eventName != null)
    {
      Transform mainUI = GameObject.Find("MainUI").transform;
      eventName.transform.SetParent(mainUI, false);

      RectTransform rtName = eventName.GetComponent<RectTransform>();
      if (rtName != null)
      {
        rtName.anchorMin = new Vector2(1f, 0.5f);
        rtName.anchorMax = new Vector2(1f, 0.5f);
        rtName.pivot = new Vector2(0.5f, 0.5f);
        rtName.anchoredPosition = new Vector2(-150f, 40f); // 타이머보다 위쪽에 배치
      }

      eventName.SetActive(true);
    }

    
  }

  protected virtual void Success()
  {
    Debug.Log("Success!");
    if (rewardItem != null && player != null)
    {
      Vector2 randomDirection = Random.insideUnitCircle.normalized;
      float randomDistance = Random.Range(5f, 7f); // 5~7 거리 랜덤
      Vector3 spawnPosition = player.transform.position + (Vector3)(randomDirection * randomDistance);

      Instantiate(rewardItem, spawnPosition, Quaternion.identity);
    }

    EndEvent();
  }

  protected virtual void Fail()
  {
    EndEvent();
  }

  protected void EndEvent()
  {
    eventInProgress = false;

    if (eventTimer != null)
      eventTimer.SetActive(false);

    if (eventName != null)
      eventName.SetActive(false);

    Destroy(eventTimer);
    Destroy(eventName);
    Destroy(infoPanel);

    Destroy(gameObject);

  }

  protected void OnDestroy()
  {
    activeEvents.Remove(this); // 파괴 시 리스트에서 제거
  }

  public void ForceEndEvent()
  {
    EndEvent();
  }
}