using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard_VoidShield : MonoBehaviour
{
  GameObject player;

  private void Start()
  {
    player = GameObject.FindWithTag("Player");
  }

  private void Update()
  {
    // �÷��̾ ���󰡰� 
    transform.position = player.transform.position + new Vector3(0, 0.2f);
    
  }

}
