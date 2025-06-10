using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkRoomPlayer
{
  [SyncVar]
  public string playerName;

  public static LobbyPlayer LocalPlayer { get; private set; }

  private void Awake()
  {
    if (isLocalPlayer)
    {
      LocalPlayer = this;
    }
  }

  public override void OnStartLocalPlayer()
  {
    base.OnStartLocalPlayer();

    CmdSetPlayerName(NameManager.PlayerName);
  }

  [Command]
  public void CmdSetPlayerName(string name)
  {
    if (isServer)
    {
      playerName = name;
    }
  }
}
