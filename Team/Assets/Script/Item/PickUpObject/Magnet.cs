using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
  private Transform playerTransform;
  public GameObject arrowPrefab;
  private GameObject myArrow;
  private AudioSource audioSource;

  void Start()
  {
    GameObject mainUIObject = GameObject.Find("MainUI");
    Transform mainUI = mainUIObject.transform;

    // 프리팹을 복제
    myArrow = Instantiate(arrowPrefab, mainUI);

    ArrowIndicator indicator = myArrow.GetComponent<ArrowIndicator>();
    indicator.magnetTarget = this.transform;

    audioSource = GetComponent<AudioSource>();

    Destroy(gameObject, 30f);
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    // 플레이어와 접촉했을 때
    if (other.CompareTag("Player"))
    {
      //효과음재생
      PlayGetItem();

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
  private void PlayGetItem()
  {
    if (SFXManager.instance != null && SFXManager.instance.item != null)
    {
      AudioSource.PlayClipAtPoint(SFXManager.instance.item, transform.position);
    }
  }

}
