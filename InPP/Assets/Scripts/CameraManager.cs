using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  
  // 카메라 움직임을 멈추는 트리거 상하좌우
  GameObject triggerUp;       
  GameObject triggerDown;
  GameObject triggerLeft;
  GameObject triggerRight;


  bool canMoveUp = true;
  bool canMoveDown = true;
  bool canMoveRight = true;
  bool canMoveLeft = true;

  public GameObject player;
  Vector3 camPos = new Vector3();

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

    // 트리거 찾기
    triggerUp = GameObject.Find("CameraTriggerUp");
    triggerDown = GameObject.Find("CameraTriggerDown");
    triggerLeft = GameObject.Find("CameraTriggerLeft");
    triggerRight = GameObject.Find("CameraTriggerRight");

    camPos.x = player.transform.position.x;
    camPos.y = player.transform.position.y;
    camPos.z = -10.0f;
    Camera.main.orthographicSize = 4.5f;
  }

  private void Update()
  {
    if (player == null) return; // player가 아직 없으면 Update 건너뜀

    Vector3 targetPos = player.transform.position;

    // 이동 제한 확인
    if(canMoveRight && canMoveLeft)
    {
      camPos.x = targetPos.x;
    }

    // 왼쪽 이동 제한
    else if(!canMoveLeft)
    {
      camPos.x = Mathf.Max(camPos.x, targetPos.x);
    }

    else if(!canMoveRight)
    {
      camPos.x = Mathf.Min(camPos.x, targetPos.x);
    }


    if (canMoveUp && canMoveDown)
    {
      camPos.y = targetPos.y;
    }
    else if (!canMoveUp) // 위쪽 이동 제한
    {
      camPos.y = Mathf.Min(camPos.y, targetPos.y);
    }
    else if (!canMoveDown) // 아래쪽 이동 제한
    {
      camPos.y = Mathf.Max(camPos.y, targetPos.y);
    }

    transform.position = camPos;


    
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject == triggerUp)
    {
      canMoveUp = false;
    }
    else if (other.gameObject == triggerDown)
    {
      canMoveDown = false;
    }
    else if (other.gameObject == triggerLeft)
    {
      canMoveLeft = false;
    }
    else if (other.gameObject == triggerRight)
    {
      canMoveRight = false;
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.gameObject == triggerUp)
    {
      canMoveUp = true;
    }
    else if (other.gameObject == triggerDown)
    {
      canMoveDown = true;
    }
    else if (other.gameObject == triggerLeft)
    {
      canMoveLeft = true;
    }
    else if (other.gameObject == triggerRight)
    {
      canMoveRight = true;
    }
  }



}
