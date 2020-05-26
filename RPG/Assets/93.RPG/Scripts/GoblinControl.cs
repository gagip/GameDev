using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class GoblinControl : MonoBehaviour {

    public enum GoblinState {  None, Idle, Patrol, Wait, MoveToTarget, Attack, Damage, Die }

    [Header("기본 속성")]
    public GoblinState goblinState = GoblinState.None;

    public float moveSpeed = 1.0f;
    public GameObject targetPlayer;
    public Transform targetTransform;
    public Vector3 targetPosition = Vector3.zero;

    private Animation anim;
    private Transform myTransform;

    [Header("애니메이션 클립")]
    public AnimationClip idleAnimClip;
    public AnimationClip moveAnimClip;
    public AnimationClip attackAnimClip;
    public AnimationClip damageAnimClip;
    public AnimationClip dieAnimClip;


    [Header("전투 속성")]
    public int hp = 100;
    public float attackRange = 1.5f;
    public GameObject damageEffect;
    public GameObject dieEffect;
    private Tweener effectTweener;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    // Use this for initialization
    void Start () {
        goblinState = GoblinState.Idle;
        // 캐싱
        anim = GetComponent<Animation>();
        myTransform = GetComponent<Transform>();
        // 애니메이션 클립들 기본 세팅
        anim[idleAnimClip.name].wrapMode = WrapMode.Loop;
        anim[moveAnimClip.name].wrapMode = WrapMode.Loop;
        anim[attackAnimClip.name].wrapMode = WrapMode.Once;
        anim[damageAnimClip.name].wrapMode = WrapMode.Once;
        anim[damageAnimClip.name].layer = 10;
        anim[dieAnimClip.name].wrapMode = WrapMode.Once;
        anim[dieAnimClip.name].layer = 10;
        // 애니메이션 이벤트 추가
        AddAnimationEvent(attackAnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(damageAnimClip, "OnDamageAnimFinished");
        AddAnimationEvent(dieAnimClip, "OnDieAnimFinished");

        skinnedMeshRenderer = myTransform.Find("body").GetComponent<SkinnedMeshRenderer>();
    }
	
    void CheckState()
    {
        switch (goblinState)
        {
            case GoblinState.Idle:
                IdleUpdate();
                break;
            case GoblinState.MoveToTarget:
            case GoblinState.Patrol:
                MoveUpdate();
                break;
            case GoblinState.Attack:
                AttackUpdate();
                break;
        }
    }

    /// <summary>
    /// 대기 상태일때 동작
    /// </summary>
    void IdleUpdate()
    {
        // 만약 타겟 플레이어가 없다면, 임의의 지점을 랜덤하게 선택해서 레이캐스트를 이용하여
        // 임의의 지점의 높이값까지 구해서 그 임의의 지점으로 이동시켜주도록 한다
        if(targetPlayer == null)
        {
            targetPosition = new Vector3(myTransform.position.x + Random.Range(-10.0f, 10.0f),
                                            myTransform.position.y + 1000.0f,
                                            myTransform.position.z + Random.Range(-10.0f, 10.0f));
            Ray ray = new Ray(targetPosition, Vector3.down);
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray, out hit, Mathf.Infinity) == true)
            {
                // 임의의 위치 높이값
                targetPosition.y = hit.point.y;
            }
            goblinState = GoblinState.Patrol;
        }
        else
        {
            goblinState = GoblinState.MoveToTarget;
        }
    }

    /// <summary>
    /// 이동 상태에서 동작
    /// </summary>
    void MoveUpdate()
    {
        Vector3 diff = Vector3.zero;
        Vector3 lookAtPosition = Vector3.zero;

        switch (goblinState)
        {
            case GoblinState.Patrol:
                if(targetPosition != Vector3.zero)
                {
                    diff = targetPosition - myTransform.position;
                    // 목표지점까지 거의 왔으면
                    if(diff.magnitude < attackRange)
                    {
                        StartCoroutine(WaitUpdate());
                        return;
                    }

                    lookAtPosition = new Vector3(targetPosition.x,
                                                myTransform.position.y,
                                                targetPosition.z);
                }
                break;
            case GoblinState.MoveToTarget:
                if(targetPosition != null)
                {
                    diff = targetPlayer.transform.position - myTransform.position;
                    // 타겟과 충분히 가까워졌다면
                    if(diff.magnitude < attackRange)
                    {
                        goblinState = GoblinState.Attack;
                        return;
                    }
                    lookAtPosition = new Vector3(targetPlayer.transform.position.x,
                                                myTransform.position.y,
                                                targetPlayer.transform.position.z);
                }
                break;
        }

        Vector3 direction = diff.normalized;
        direction = new Vector3(direction.x, 0.0f, direction.z);
        Vector3 moveAmount = direction * moveSpeed * Time.deltaTime;
        myTransform.Translate(moveAmount, Space.World);

        myTransform.LookAt(lookAtPosition);
    }

    /// <summary>
    /// 대기 동작
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitUpdate()
    {
        goblinState = GoblinState.Wait;
        float waitTime = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(waitTime);
        goblinState = GoblinState.Idle;
    }

    void AnimationControl()
    {
        switch (goblinState)
        {
            case GoblinState.Wait:
            case GoblinState.Idle:
                anim.CrossFade(idleAnimClip.name);
                break;
            case GoblinState.Patrol:
            case GoblinState.MoveToTarget:
                anim.CrossFade(moveAnimClip.name);
                break;
            case GoblinState.Attack:
                anim.CrossFade(attackAnimClip.name);
                break;
            case GoblinState.Die:
                anim.CrossFade(dieAnimClip.name);
                break;
        }
    }

	// Update is called once per frame
	void Update () {
        CheckState();

        AnimationControl();
	}

    /// <summary>
    /// 인지범위 안에 다른 트리거나 플레이어가 들어왔다면 호출
    /// </summary>
    /// <param name="target"></param>
    void OnSetTarget(GameObject target)
    {
        targetPlayer = target;
        targetTransform = targetPlayer.transform;
        goblinState = GoblinState.MoveToTarget;
    }

    /// <summary>
    /// 공격 상태 
    /// </summary>
    void AttackUpdate()
    {
        float distance = Vector3.Distance(targetTransform.position, myTransform.position);
        if(distance > attackRange + 0.5f)
        {
            // 타겟과의 거리가 멀어졌다면 타겟으로 이동
            goblinState = GoblinState.MoveToTarget;
        }
    }

    /// <summary>
    /// 충돌 검출
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") == true)
        {
            hp -= 10;
            if(hp > 0)
            {
                Instantiate(damageEffect, other.transform.position, Quaternion.identity);
                anim.CrossFade(damageAnimClip.name);
                DamageTweenEffect();
            }
            else
            {
                goblinState = GoblinState.Die;
            }
        }
    }

    void DamageTweenEffect()
    {
        // 트윈이 재생중이면 중복 트윈 세팅 X
        if(effectTweener != null && effectTweener.isComplete == false)
        {
            return;
        }
        Color colorTo = Color.red;
        effectTweener = HOTween.To(skinnedMeshRenderer.material, 0.2f, new TweenParms()
            .Prop("color", colorTo)
            .Loops(1, LoopType.Yoyo)
            .OnStepComplete(OnDamageTweenFinished));
    }

    void OnDamageTweenFinished()
    {
        skinnedMeshRenderer.material.color = Color.white;
    }

    void OnAttackAnimFinished()
    {
        Debug.Log("Attack Animation Finished");
    }

    void OnDamageAnimFinished()
    {
        Debug.Log("Damage Animation Finished");
    }

    void OnDieAnimFinished()

    {
        Debug.Log("Die Animation Finished");
        Instantiate(dieEffect, myTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// 애니메이션 이벤트를 추가해주는 함수
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="funcName"></param>
    void AddAnimationEvent(AnimationClip clip, string funcName)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = funcName;
        newEvent.time = clip.length - 0.1f;
        clip.AddEvent(newEvent);
    }
}
