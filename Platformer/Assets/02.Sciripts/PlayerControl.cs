using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [Header("조작 관련 변수")]
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;

    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Demeaged
    }
    public PlayerState playerState;
    public bool jumping = false;
    public int maxJump = 2;
    public float maxJumpPower = 1.0f;
    public int jumpCount;
    Animator animator;
    SpriteRenderer spriteRenderer;


    private void Awake()
    {
        jumpCount = maxJump;
        playerState = PlayerState.Idle;
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        Move();
        Jump();
    }


    private void Update()
    {

        // 좌우 위치 바꾼다.
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        // 상태를 확인하고
        CheckState();
        // 상태에 따라 애니메이션
        PlayerAnimation();
    }


    void Move()
    {
        // 가로 이동
        float h = Input.GetAxisRaw("Horizontal"); // -1, 1 input
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // velocity.x 조정
        if (rigid.velocity.x > maxSpeed)  // Velocity : 리지드바디의 현재 속도
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); // y값은 그대로
        }
        else if (rigid.velocity.x < -maxSpeed)
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }
    } 

    void Jump()
    {
        // 점프할 수 있으면 점프 버튼 활성화
        if (jumpCount > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
                jumpCount--;
            }
        }
        
        // 오래동안 누르면 점프 종료
        if (jumping)
        {
            float v = Input.GetAxisRaw("Jump");
            rigid.AddForce(Vector2.up * v, ForceMode2D.Impulse);
            if (rigid.velocity.y > maxJumpPower) // 오래동안 누를 경우
            {
                jumping = false;
            }
            else if (Input.GetButtonUp("Jump")) // 버튼을 뗀 경우
            {
                jumping = false;
            }
         }

        //Landing Platform
        // 착지할 때만
        if (rigid.velocity.y < 0)
        { 
            // 오브젝트 아래로 선 그리기
            Debug.DrawRay(new Vector2(rigid.position.x + 0.3f, rigid.position.y), Vector3.down, new Color(0, 1, 0));
            // 바닥 찾는 raycast 
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
                {
                    jumping = false;
                    jumpCount = maxJump;
                }
            }
        }
        // 이단 점프 가능하게?
    }

    /// <summary>
    /// 상태 업데이트 함수
    /// </summary>
    void CheckState()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                // Velocity 증가하면 Run 상태로
                if (Mathf.Abs(rigid.velocity.x) >= 1f)
                    playerState = PlayerState.Run;
                if (rigid.velocity.y > 0)
                    playerState = PlayerState.Jump;
                break;
            case PlayerState.Run:
                // Velocity 낮아지면 Idle 상태로
                if (Mathf.Abs(rigid.velocity.x) < 1f)
                    playerState = PlayerState.Idle;
                if (rigid.velocity.y > 0)
                    playerState = PlayerState.Jump;
                break;
            case PlayerState.Jump:
                if (rigid.velocity.y == 0f)
                {
                    if (Mathf.Abs(rigid.velocity.x) >= 1f)
                        playerState = PlayerState.Run;
                    else
                        playerState = PlayerState.Idle;
                }
                break;
            case PlayerState.Demeaged:
                break;
        }
    }

    /// <summary>
    /// 상태에 따라 애니메이션 발동
    /// </summary>
    void PlayerAnimation()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                animator.SetBool("isRun", false);
                animator.SetBool("isJump", false);
                break;
            case PlayerState.Run:
                animator.SetBool("isRun", true);
                animator.SetBool("isJump", false);
                break;
            case PlayerState.Jump:
                animator.SetBool("isJump", true);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) //몬스터 위에 있음 + 아래로 낙하중 = 밟음
            {
                OnAttack(collision.transform);
                //gameManager.stagePoint += 150;
            }
            else
            {
                StartCoroutine(OnDamaged(collision.transform.position));
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        // audio
        //PlaySound("ATTACK");

        // reaction force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // enemy die
        EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        enemyControl.StartCoroutine(enemyControl.OnDamaged());
    }

    IEnumerator OnDamaged(Vector2 targetPos)
    {
        // gameManager.HealthDown();

        // 캐릭터 무적 상태
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //animator.SetTrigger("doDamaged");
        //PlaySound("DAMAGED");
        yield return new WaitForSeconds(3.0f);

        // 3초 뒤에 다시 캐릭터 활성화
        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
