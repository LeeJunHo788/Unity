using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
  private Transform playerTransform;
  public GameObject arrowPrefab;
  private GameObject myArrow;

  void Start()
  {
    GameObject mainUIObject = GameObject.Find("MainUI");
    Transform mainUI = mainUIObject.transform;

    // 프리팹을 복제
    myArrow = Instantiate(arrowPrefab, mainUI);

    ArrowIndicator indicator = myArrow.GetComponent<ArrowIndicator>();
    indicator.magnetTarget = this.transform;

    Destroy(gameObject, 30f);
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    // 플레이어와 접촉했을 때
    if (other.CompareTag("Player"))
    {
      other.GetComponent<PlayerController>().AttractExp();
      Destroy(gameObject);
    }
  }

  void OnDestroy()
  {
    if (myArrow != null)
    {
      Destroy(myArrow);
    }
  }


}
