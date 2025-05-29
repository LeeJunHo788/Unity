using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Button �̺�Ʈ��

public class ButtonManager : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
  private TextMeshProUGUI buttonText;
  private Vector3 originalScale;
  private Color originalColor;

  [Header("���� ����")]
  public Color highlightColor = new Color32(81, 255, 208, 255);

  [Header("���� ����")]
  public AudioClip clickSound;
  public AudioClip highlightSound;
  private AudioSource audioSource;

  void Awake()
  {
    buttonText = GetComponentInChildren<TextMeshProUGUI>();
    originalScale = buttonText.transform.localScale;
    originalColor = buttonText.color;

    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Ŭ�� ���� ����
    Button btn = GetComponent<Button>();
    if (btn != null)
    {
      btn.onClick.AddListener(PlayClickSound);
    }
  }

  public void OnSelect(BaseEventData eventData)
  {
    Highlight();
  }

  public void OnDeselect(BaseEventData eventData)
  {
    UnHighlight();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Highlight();
  }
  
  public void OnPointerExit(PointerEventData eventData)
  {
    UnHighlight();
  }

  private void Highlight()
  {
    if (buttonText == null) return;
    buttonText.color = highlightColor;
    buttonText.transform.localScale = originalScale * 1.1f;

    PlayHighlightSound();
  }

  private void UnHighlight()
  {
    if (buttonText == null) return;
    buttonText.color = originalColor;
    buttonText.transform.localScale = originalScale;
  }

  public void ForceUnHighlight()
  {
    if (buttonText == null) return; 
    buttonText.color = originalColor;
    buttonText.transform.localScale = originalScale;
  }

  private void PlayHighlightSound()
  {
    if (highlightSound != null && AudioManager.Instance != null)
    {
      AudioManager.Instance.PlaySFX(highlightSound); 
    }
  }

  private void PlayClickSound()
  {
    if (clickSound != null && AudioManager.Instance != null)
    {
      AudioManager.Instance.PlaySFX(clickSound); 
    }
  }
}