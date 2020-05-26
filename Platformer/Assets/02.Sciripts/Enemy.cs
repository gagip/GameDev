using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D capsuleCollider;
    protected PlayerControl player;
    protected bool isNewState;

    public int nextMove;






    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    protected void Think()
    {
        nextMove = Random.Range(-1, 2); // [-1, 2)

        float nextThinkTime = Random.Range(2f, 2f);
        Invoke("Think", nextThinkTime);

        // 좌우반전
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }

    protected void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    // 기본적으로 적들은 해당 함수들을 구현해야 함
    abstract protected IEnumerator StartState();
    abstract protected IEnumerator Move();
    abstract protected IEnumerator OnDamaged();
}
