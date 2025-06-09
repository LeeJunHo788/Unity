using UnityEngine;
using Mirror;
using System.Data;

public class GamePlayer : NetworkBehaviour
{
  [SyncVar]
  public string role;


  [ClientRpc]
  public void RpcSetRole(string newRole)
  {
    role = newRole;
    Debug.Log($"[Client] ���� �Ҵ��: {role}");

    // ���ҿ� ���� �ٸ� UI ����
  }

}
