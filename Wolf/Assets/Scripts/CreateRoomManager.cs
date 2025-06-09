using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateRoomManager : MonoBehaviour
{
  [SerializeField]
  private List<Image> roleImage;

  private CreateRoomData roomData;
}

public class CreateRoomData
{
  public int maxPlayerCount;
  public int minPlayerCount;
  public int currentPlayerCount;
}
