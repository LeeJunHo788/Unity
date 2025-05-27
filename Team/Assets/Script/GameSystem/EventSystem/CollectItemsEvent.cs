using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectItemsEvent : EventSystemObject
{
  public GameObject arrowPrefab;
  public GameObject itemPrefab;
  public int itemCount = 5;
  public float minSpawnDistance = 2f;
  public float maxSpawnDistance = 5f;

  private int collectedCount = 0;
  private GameObject currentItem;

  protected override void Start()
  {
    base.Start();
  }

  public override void TriggerEvent()
  {
    base.TriggerEvent();
    collectedCount = 0;
    eventNameText.text = $"아이템 수집 {collectedCount}  5";
    SpawnNewItem();
  }

  private void SpawnNewItem()
  {
    Vector3 randomDirection = Random.insideUnitCircle.normalized;
    float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
    Vector3 randomPos = transform.position + (Vector3)(randomDirection * randomDistance);

    currentItem = Instantiate(itemPrefab, randomPos, Quaternion.identity);

    CollectableItem ci = currentItem.AddComponent<CollectableItem>();
    ci.Setup(this);

    CreateArrowIndicator(currentItem.transform);
  }

  private void CreateArrowIndicator(Transform target)
  {
    GameObject arrowInstance = Instantiate(arrowPrefab, GameObject.Find("MainUI").transform);
    ArrowIndicator arrowIndicator = arrowInstance.GetComponent<ArrowIndicator>();
    arrowIndicator.magnetTarget = target;

    Image img = arrowInstance.GetComponentInChildren<Image>();
    if (img != null)
    {
      img.color = Color.red;
    }
  }

  public void OnItemCollected()
  {
    if (!eventInProgress) return;

    collectedCount++;
    eventNameText.text = $"아이템 수집 {collectedCount} / 5";

    if (collectedCount >= itemCount)
    {
      Success(); // 수집 성공시 Success 호출
    }
    else
    {
      SpawnNewItem();
    }
  }

  protected override void Success()
  {
    Debug.Log("성공!");
    base.Success(); // 기본 성공 처리 호출
  }

  protected override void Fail()
  {
    Debug.Log("실패");
    base.Fail(); // 기본 실패 처리 호출
  }
}