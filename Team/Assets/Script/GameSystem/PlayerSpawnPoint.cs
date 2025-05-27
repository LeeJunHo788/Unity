using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
  GameObject player;

  void Start()
  {
    player = GameObject.FindWithTag("Player");

    player.transform.position = transform.position;
  }


}
