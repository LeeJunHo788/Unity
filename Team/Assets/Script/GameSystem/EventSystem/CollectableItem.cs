using UnityEngine;

public class CollectableItem : MonoBehaviour
{
  private CollectItemsEvent eventSystem;

  private ArrowIndicator attachedArrow;
  private bool isSetup = false; 

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
      eventSystem.OnItemCollected();

      if (attachedArrow != null)
        Destroy(attachedArrow.gameObject);

      Destroy(gameObject);
    }
  }
}