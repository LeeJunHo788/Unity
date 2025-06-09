using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class WolfGameNetworkManager : NetworkManager
{
  public override void OnServerAddPlayer(NetworkConnectionToClient conn)
  {
    base.OnServerAddPlayer(conn);

    // 플레이어 이름 임시 지정 (나중에 UI에서 받을 수 있음)
    LobbyPlayer playerInfo = conn.identity.GetComponent<LobbyPlayer>();
    playerInfo.SetPlayerName($"Player {numPlayers}");
  }

  [ContextMenu("Start Game")]

  public void StartGame()
  {
    Debug.Log("게임 시작!");

    List<string> roles = new List<string> { "WereWolf", "Minion", "Citizen", "Mason", "Seer", "Robber", "TroubleMaker", "Drunk", "Insomniak", "Doppelganger" };

    int i = 0;
    foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
    {
      GamePlayer playerInfo = conn.identity.GetComponent<GamePlayer>(); 

      string assignedRole = roles[i % roles.Count];
      playerInfo.RpcSetRole(assignedRole);

      // Debug.Log($"플레이어 {playerInfo.playerName} → 역할 {assignedRole}");
      i++;
    }
  }
}
