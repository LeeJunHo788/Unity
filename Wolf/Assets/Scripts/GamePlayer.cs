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
    Debug.Log($"[Client] 역할 할당됨: {role}");

    // 역할에 따라 다른 UI 띄우기
  }

}
