using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{

  public static PlayerController Instance { get; private set; }

  [HideInInspector]
  public InputSystem_Actions controls;


  [HideInInspector]
  public Rigidbody2D rb;

  [Header("�Ҵ� ������Ʈ")]
  public GameObject directionObj;

  [Header("����")]
  public float att = 10;
  public float defIg = 0;


  bool isReady = true;
  Transform startPos;



  float moveSpeed = 10;
  float angle = 90f;         // ���� ����
  float angleSpeed = 60f;   // ȸ�� �ӵ�

  public event Action OnPlayerReady;

  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else
      Destroy(gameObject);  // �ߺ��� ������Ʈ�� ����


    controls = new InputSystem_Actions();
    controls.Player.Enable();
    controls.Player.Fire.performed += ct => BallFire();
  }

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    rb.linearVelocity = new Vector2(1, 1).normalized * moveSpeed;
    startPos = transform;

    transform.rotation = Quaternion.Euler(0, 0, angle);
  }


  private void Update()
  {
    SetDirection();
  }

  private void SetDirection()
  {
    float input = controls.Player.Move.ReadValue<Vector2>().x; // ��� Ű �Է°� (-1~1)

    // �Է��� ���� �� ������ ����
    if (input != 0 && isReady)
    {
      angle += -input * angleSpeed * Time.deltaTime;
       
      angle = Mathf.Clamp(angle, 20, 160);
     
      transform.rotation = Quaternion.Euler(0, 0, angle);
    }
  }

  void BallFire()
  {
    if(isReady)
    {
      float angleRad = angle * Mathf.Deg2Rad;
      Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
      
      rb.linearVelocity = dir.normalized * moveSpeed;
      directionObj.gameObject.SetActive(false);
      isReady = false;
    }
  }


  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.collider.CompareTag("DSideBar"))
    {
      startPos.position = collision.GetContact(0).point;
      rb.linearVelocity = Vector2.zero;
      transform.rotation = Quaternion.Euler(0, 0, 90);

      angle = 90f;
      isReady = true;
      directionObj.gameObject.SetActive(true);

      OnPlayerReady?.Invoke();
    }
  }
}
