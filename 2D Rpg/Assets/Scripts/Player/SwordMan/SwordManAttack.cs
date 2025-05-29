using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManAttack : MonoBehaviour
{
  PlayerController pc;    // 플레이어 컨트롤러
  GameObject player;    // 플레이어의 캐릭터

  public GameObject[] AttackPrefabs;    // 공격 오브젝트 프리팹
  GameObject newAttack;   // 공격시 생기는 오브젝트

  Vector3 pos;    // 공격 오브젝트가 소환 될 위치

  void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    // 각 타수의 공격력 배율 설정
    PlayerPrefs.SetFloat("SwordAttack1", 1.0f);
    PlayerPrefs.SetFloat("SwordAttack2", 1.2f);
    PlayerPrefs.SetFloat("SwordAttack3", 1.5f);

  }

  void Update()
  {
    if (pc.isAttack == true && newAttack == null && !IsInvoking("SpawnAttack"))
    {
      Invoke("SpawnAttack", pc.attackTime * 0.25f);
    }
  }

  void SpawnAttack()
  {
    if (AttackPrefabs == null)
    {
      return;
    }
    
    // 공격 실행후 공격 오브젝트가 생성되기 전에 피격시 공격 취소
    if(pc.isKnockBack)
    {
      pc.isAttack = false;
      return;
    }

    pos = player.transform.position;

    // 플레이어가 바라보는 방향에 따라 공격 생성 위치 변경
    if (player.transform.rotation.y == 0)
    {
      pos.x += 1;
    }

    else
    {
      pos.x -= 0.5f;
    }

    int index = pc.comboStep - 1;

    if (index < 0 || index >= AttackPrefabs.Length)
    {
      return; // 배열 범위를 벗어나면 생성하지 않고 종료
    }

    newAttack = Instantiate(AttackPrefabs[index], pos, Quaternion.identity);
    newAttack.name = AttackPrefabs[index].name;    // 이름 뒤에 Clone제거

    StartCoroutine(AttackActiveFalse(newAttack, pc.attackTime / 1.5f));
    Destroy(newAttack, pc.attackTime);
  }

  // 공격 비활성화 메서드
  IEnumerator AttackActiveFalse(GameObject Attack, float time)
  {
    yield return new WaitForSeconds(time);
    Attack.SetActive(false);
  }  
}
