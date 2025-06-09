using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkRoomPlayer
{
  [SyncVar]
  public string playerName;

  public void SetPlayerName(string name)
  {
    if (isServer)
    {
      playerName = name;
    }
  }

  



}
