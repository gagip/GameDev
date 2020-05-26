using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;

    [Header("오디오변수")]
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;
    CapsuleCollider2D capsuleCollider;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }
 
    // Update is called once per frame
    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            PlaySound("JUMP");

        }

        if (Input.GetButtonUp("Horizontal")) // 버튼을 땔 때 속력을 줄이자
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 방향전환
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // 애니메이션
        if (Mathf.Abs(rigid.velocity.normalized.x) < 0.5f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)  // Velocity : 리지드바디의 현재 속도
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -maxSpeed)
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }

        //Landing Platform
        // 착지할 때만
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
                {
                    animator.SetBool("isJumping", false);
                }
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) //몬스터 위에 있음 + 아래로 낙하중 = 밟음
            {
                OnAttack(collision.transform);
                gameManager.stagePoint += 150;
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            bool isBCoin = collision.gameObject.name.Contains("BCoin");
            bool isSCoin = collision.gameObject.name.Contains("SCoin");
            bool isGCoin = collision.gameObject.name.Contains("GCoin");

            if (isBCoin)
                gameManager.stagePoint += 50;
            else if (isSCoin)
                gameManager.stagePoint += 100;
            else
                gameManager.stagePoint += 300;

            PlaySound("ITEM");
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Finish"))
        {
            PlaySound("FINISH");
            gameManager.NextStage();
        }
        
    }

    void OnAttack(Transform enemy)
    {
        // audio
        PlaySound("ATTACK");

        // reaction force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // enemy die
        EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        enemyControl.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HealthDown();
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        animator.SetTrigger("doDamaged");
        PlaySound("DAMAGED");

        Invoke("OffDamaged", 3f);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        PlaySound("DIE");
        // sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // sprite filpY
        spriteRenderer.flipY = true;
        // collider off
        capsuleCollider.enabled = false;
        // die effect jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
