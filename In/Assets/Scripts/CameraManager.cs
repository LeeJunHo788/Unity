using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  
  // ī�޶� �������� ���ߴ� Ʈ���� �����¿�
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
      yield return null; // ���� �����ӱ��� ���
    }

    player = GameObject.FindWithTag("Player");

    // Ʈ���� ã��
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
    if (player == null) return; // player�� ���� ������ Update �ǳʶ�

    Vector3 targetPos = player.transform.position;

    // �̵� ���� Ȯ��
    if(canMoveRight && canMoveLeft)
    {
      camPos.x = targetPos.x;
    }

    // ���� �̵� ����
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
    else if (!canMoveUp) // ���� �̵� ����
    {
      camPos.y = Mathf.Min(camPos.y, targetPos.y);
    }
    else if (!canMoveDown) // �Ʒ��� �̵� ����
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
