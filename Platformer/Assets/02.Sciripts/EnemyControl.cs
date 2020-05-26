using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator animator;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Think();

        Invoke("Think", 5); // 5초 뒤에 실행
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        // Platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2); // [-1, 2)

        float nextThinkTime = Random.Range(2f, 2f);
        Invoke("Think", nextThinkTime);

        //animator.SetInteger("WalkSpeed", nextMove);

        // 좌우반전
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public IEnumerator OnDamaged()
    {
        // sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // sprite filpY
        spriteRenderer.flipY = true;
        // collider off
        capsuleCollider.enabled = false;
        // die effect jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // destroy
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
        Debug.Log("몬스터가 사망하였습니다");
    }
}
