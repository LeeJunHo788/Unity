using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingClear : MonoBehaviour
{
  void Start()
  {
    // �̸����� ������Ʈ ã��
    GameObject player = GameObject.Find("Player");
    GameObject ui = GameObject.Find("MainUI");

    // ������ ����
    if (player != null) Destroy(player);         // �÷��̾� ����
    if (ui != null) Destroy(ui);                 // ���� UI ����
  }
}
