using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
  public Transform cameraTransform;         // ���� ī�޶�
  public float parallaxFactor = 0.5f;       // ����� ������� ���� (0~1 ����)
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
      yield return null; // ���� �����ӱ��� ���
    }

    player = GameObject.FindWithTag("Player");

    // transform.position = player.transform.position;

    // ������ �� ī�޶� ��ġ ����
    if (cameraTransform == null)
      cameraTransform = Camera.main.transform;

    previousCameraPosition = cameraTransform.position;
  }

  void LateUpdate()
  {
    if (cameraTransform == null) return;

    // ī�޶� �̵��� �Ÿ� ���
    Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

    // ����� ������� ����� (Z ���� ����)
    transform.position += new Vector3(
        deltaMovement.x * parallaxFactor,
        deltaMovement.y * parallaxFactor,
        0f
    );

    // �̹� �������� ī�޶� ��ġ ����
    previousCameraPosition = cameraTransform.position;
  }
}
