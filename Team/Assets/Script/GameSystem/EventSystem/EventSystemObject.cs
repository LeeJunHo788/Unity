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
  public float interactionRange = 3f; // ��ȣ�ۿ� ����
  public float timeLimit = 30f;        // ���� �ð�

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

    activeEvents.Add(this); // ����Ʈ�� ���
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
        eventTimerText.text = $"���� �ð�: {timeLeft:F1}��";

        // ���� �ð� 10�� ������ �� ���� ����
        if (timeLeft <= 10f)
          eventTimerText.color = Color.red;
        else
          eventTimerText.color = Color.black;
      }

      // �ð� �ʰ� üũ
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
        rtName.anchoredPosition = new Vector2(-150f, 40f); // Ÿ�̸Ӻ��� ���ʿ� ��ġ
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
      float randomDistance = Random.Range(5f, 7f); // 5~7 �Ÿ� ����
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
    activeEvents.Remove(this); // �ı� �� ����Ʈ���� ����
  }

  public void ForceEndEvent()
  {
    EndEvent();
  }
}