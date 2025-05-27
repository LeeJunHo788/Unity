using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Itembase
{
    public PlayerController pc;
    public GameObject Player;

    [Header("무기스펙")]
    public string iteminfo;
    public float Dmg;
    public float attackRate;
    public int howUpgrade;

    [Header("방패 하위오브젝트")]
    public GameObject shieldsun;

    bool isAttack = false;
    bool isShiled = false;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        pc.defence += (5 * howUpgrade);

    }

  // Update is called once per frame
  void Update()
    {
        howUpgrade = Level;
        enemycheck();
        if (isAttack&&!isShiled)
        {
            // 공격
            StartCoroutine(attack());
        }
      
        switch (howUpgrade)
        {
            case 0:
                Dmg = 1f * pc.attackDem;
                attackRate = 3f * (20f + (10f * pc.attackSpeed));
                
                break;
            case 1:
                Dmg = 2f * pc.attackDem;
                attackRate = 3f * (20f + (15f * pc.attackSpeed));
                
                break;
            case 2:
                Dmg = 4f * pc.attackDem;
                attackRate = 3f * (20f + (20f * pc.attackSpeed));
                break;
            case 3:
                Dmg = 6f * pc.attackDem;
                attackRate = 3f * (20f + (25f * pc.attackSpeed));
                break;
            case 4:
                Dmg = 10f * pc.attackDem;
                attackRate = 3f * (20f + (30f * pc.attackSpeed));
                break;
            case 5:
                Dmg = 15f * pc.attackDem;
                attackRate = 3f * (20f + (50f * pc.attackSpeed));
                break;
        }
    }

    public void enemycheck()
    {        
        Collider2D[] hits = Physics2D.OverlapCircleAll(Player.transform.position, 5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                isAttack = true;
            }

            if (hit.CompareTag("Boss"))
            {
                isAttack = true;
            }
        }
    }

    IEnumerator attack()
    {             
        GameObject Sclone = Instantiate(shieldsun, transform.position, Quaternion.Euler(0, 0, 0));
        if (Sclone.activeSelf)
        {
            isShiled = true;
        }       
        yield return null;
    }
}
