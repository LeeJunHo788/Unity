using UnityEngine;

public class CollectableItem : MonoBehaviour
{
  private CollectItemsEvent eventSystem;

  private ArrowIndicator attachedArrow;
  private bool isSetup = false;

  private AudioSource audioSource;

  private void Start()
  {
    audioSource = GetComponent<AudioSource>();
  }

  public void Setup(CollectItemsEvent e)
  {
    eventSystem = e;
    attachedArrow = FindArrow();
    isSetup = true;
  }

  private ArrowIndicator FindArrow() 
  {
    ArrowIndicator[] arrows = FindObjectsOfType<ArrowIndicator>();
    foreach (var arrow in arrows)
    {
      if (arrow.magnetTarget == transform)
        return arrow;
    }
    return null;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (!isSetup) return;

    if (collision.CompareTag("Player"))
    {
      PlayGetItem(); //효과음재생

      eventSystem.OnItemCollected();

      if (attachedArrow != null)
        Destroy(attachedArrow.gameObject);

      Destroy(gameObject);
    }
  }

  private void PlayGetItem()
  {
    if (audioSource != null && SFXManager.instance != null && SFXManager.instance.item != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.item);
    }
  }
}