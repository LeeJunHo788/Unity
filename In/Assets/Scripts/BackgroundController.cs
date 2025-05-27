using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
  public Transform cameraTransform;         // 따라갈 카메라
  public float parallaxFactor = 0.5f;       // 배경의 따라오는 정도 (0~1 사이)
  GameObject player;

  private Vector3 previousCameraPosition;

  void Start()
  {
    StartCoroutine(Starting());
  }


  IEnumerator Starting()
  {
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }

    player = GameObject.FindWithTag("Player");

    // transform.position = player.transform.position;

    // 시작할 때 카메라 위치 저장
    if (cameraTransform == null)
      cameraTransform = Camera.main.transform;

    previousCameraPosition = cameraTransform.position;
  }

  void LateUpdate()
  {
    if (cameraTransform == null) return;

    // 카메라가 이동한 거리 계산
    Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

    // 배경을 따라오게 만들기 (Z 방향 제외)
    transform.position += new Vector3(
        deltaMovement.x * parallaxFactor,
        deltaMovement.y * parallaxFactor,
        0f
    );

    // 이번 프레임의 카메라 위치 저장
    previousCameraPosition = cameraTransform.position;
  }
}
