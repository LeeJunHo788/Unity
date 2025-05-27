using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (target != null)
            {
                Vector2 pos = Vector2.Lerp(player.transform.position,
                    target.transform.position, 0.5f);
                transform.position = new Vector3(pos.x, pos.y, -10);
            }
            else
            {
                transform.position = new Vector3(player.transform.position.x,
                    player.transform.position.y, -10);
            }
        }
    }
}
