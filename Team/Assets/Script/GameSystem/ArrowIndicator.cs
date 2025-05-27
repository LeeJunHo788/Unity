using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
  public Transform magnetTarget; // 플레이어 타겟
  public RectTransform arrowUI; // 화면에 표시할 화살표 UI
  private Camera mainCamera;
  private RectTransform canvasRect; // 캔버스 RectTransform

  void Start()
  {
    mainCamera = Camera.main;

    // canvasRect를 자동으로 찾기
    canvasRect = arrowUI.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

    if (arrowUI != null && canvasRect != null)
    {
      // 시작할 때 스크린 중앙에 맞춰놓기
      Vector2 localPos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
          canvasRect,
          new Vector2(Screen.width / 2f, Screen.height / 2f),
          mainCamera,
          out localPos
      );
      arrowUI.localPosition = localPos;
    }
  }

  void Update()
  {
    if (arrowUI == null || canvasRect == null) return;

    if (magnetTarget == null)
    {
      arrowUI.gameObject.SetActive(false);
      return;
    }

    Vector3 screenPos = mainCamera.WorldToScreenPoint(magnetTarget.position);

    if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
    {
      arrowUI.gameObject.SetActive(false);
    }
    else
    {
      arrowUI.gameObject.SetActive(true);

      Vector3 fromCenter = screenPos - new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
      float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
      arrowUI.rotation = Quaternion.Euler(0, 0, angle);

      Vector3 clamped = fromCenter.normalized;
      float maxX = (Screen.width / 2f) - 50f;  // 가로 가장자리 기준
      float maxY = (Screen.height / 2f) - 50f; // 세로 가장자리 기준

      float clampX = Mathf.Clamp(clamped.x * Mathf.Abs(fromCenter.x), -maxX, maxX);
      float clampY = Mathf.Clamp(clamped.y * Mathf.Abs(fromCenter.y), -maxY, maxY);

      Vector2 finalPosition = new Vector2(clampX, clampY);

      Vector2 localPos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
          canvasRect,
          new Vector2(Screen.width / 2f, Screen.height / 2f) + finalPosition,
          mainCamera,
          out localPos
      );
      arrowUI.localPosition = localPos;
    }
  }
}