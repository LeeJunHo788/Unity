using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrewAxe : MonoBehaviour
{
    // 임시 공격력
    public float Dmg;
    EnemyController enemy;
    PlayerController player;
    BossController bc;
    GameObject Axe;
    Axe AxeStat;

    void Start()
    {
        // 플레이어를 찾고 스텟 받아올 준비
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        // 본체를 찾고 강화수치 받아올 준비
        Axe = GameObject.Find("Axe");
        AxeStat = Axe.GetComponent<Axe>();       
        // 강화수치에 따른 공격력 배율증가
        switch (AxeStat.howUpgrade)
        {
            case 0:
                Dmg = 1.0f*player.attackDem;
                break;
            case 1:
                Dmg = 1.25f*player.attackDem;
                break;
            case 2:
                Dmg = 1.575f*player.attackDem;
                break;
            case 3:
                Dmg = 1.70f*player.attackDem;
                break;
            case 4:
                Dmg = 2.05f*player.attackDem;
                break;
            case 5:
                Dmg = 3.00f*player.attackDem;
                break;
        }

        // 투사체 삭체 코루틴
        StartCoroutine(Destroy());      
    }

    // Update is called once per frame
    void Update()
    {
        // 회전시키기
        transform.Rotate(new Vector3(0, 0, 1000f * Time.deltaTime));
        player.GetComponent<PlayerController>();
        // 플레이어 공격력과 도끼배율 가져오기
        Dmg = player.attackDem * AxeStat.axeDmg;
    }
  
    // 3.5초 후 게임오브젝트 삭제
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }

    // 에네미와 충돌하였을 시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 충돌체의 에네미 컨트롤러를 받아오고
            // 데미지를 준다           
            collision.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);          
        }
        else if (collision.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);

        }
    }
}
