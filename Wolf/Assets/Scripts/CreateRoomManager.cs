using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CreateRoomManager : MonoBehaviour
{
  [SerializeField]
  private List<Image> roleImage;

  private CreateRoomData roomData;

  public void CreateRoom()
  {
    var manager = WolfGameRoomManager.singleton;

    // 规 积己 累诀 贸府
    //
    //


    manager.StartHost();
  }
}

public class CreateRoomData
{
  public int maxPlayerCount;
  public int minPlayerCount;
  public int currentPlayerCount;
}
