using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class WolfGameNetworkManager : NetworkManager
{
  public override void OnServerAddPlayer(NetworkConnectionToClient conn)
  {
    base.OnServerAddPlayer(conn);

    // �÷��̾� �̸� �ӽ� ���� (���߿� UI���� ���� �� ����)
    LobbyPlayer playerInfo = conn.identity.GetComponent<LobbyPlayer>();
    playerInfo.SetPlayerName($"Player {numPlayers}");
  }

  [ContextMenu("Start Game")]

  public void StartGame()
  {
    Debug.Log("���� ����!");

    List<string> roles = new List<string> { "WereWolf", "Minion", "Citizen", "Mason", "Seer", "Robber", "TroubleMaker", "Drunk", "Insomniak", "Doppelganger" };

    int i = 0;
    foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
    {
      GamePlayer playerInfo = conn.identity.GetComponent<GamePlayer>(); 

      string assignedRole = roles[i % roles.Count];
      playerInfo.RpcSetRole(assignedRole);

      // Debug.Log($"�÷��̾� {playerInfo.playerName} �� ���� {assignedRole}");
      i++;
    }
  }
}
