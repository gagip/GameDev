using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterControl : MonoBehaviour {

    [Header("이동관련속성")]
    [Tooltip("기본이동속도")] public float moveSpeed = 2.0f; // 이동속도
    public float runSpeed = 3.5f; // 달리기
    public float directionRotateSpeed = 100.0f; // 이동방향을 변경하기 위한 속도
    public float bodyRotateSpeed = 2.0f; // 몸통의 방향을 변경하기 위한 속도
    [Range(0.1f, 5.0f)] public float velocityChangeSpeed = 0.1f; // 속도가 변경되기 위한 속도
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private float gravity = 9.8f; // 중력값
    private float verticalSpeed = 0.0f; // 수직 속도
    private bool cannotMove = false;

    [Header("애니메이션관련속성")]
    public AnimationClip idleAnimClip;
    public AnimationClip walkAnimClip;
    public AnimationClip runAnimClip;
    public AnimationClip attack1AnimClip;
    public AnimationClip attack2AnimClip;
    public AnimationClip attack3AnimClip;
    public AnimationClip attack4AnimClip;
    public AnimationClip skillAnimClip;

    private Animation myAnimation;

    public enum FighterState { None, Idle, Walk, Run, Attack, Skill}
    [Header("캐릭터상태")]
    public FighterState myState = FighterState.None;
    
    public enum FighterAttackState { Attack1, Attack2, Attack3, Attack4 }
    public FighterAttackState attackState = FighterAttackState.Attack1;
    // 다음 공격 활성화 여부를 확인하는 플래그
    public bool nextAttack = false;

    [Header("전투 관련")]
    public TrailRenderer attackTrailRenderer;
    public CapsuleCollider attackCapsuleCollider;
    public GameObject skillEffect;

    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();

        myAnimation = GetComponent<Animation>();
        myAnimation.playAutomatically = false; // 자동 재생 끄고
        myAnimation.Stop(); // 애니메이션 정지

        myState = FighterState.Idle;
        myAnimation[idleAnimClip.name].wrapMode = WrapMode.Loop; // 대기 애니메이션은 반복모드
        myAnimation[walkAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[runAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[attack1AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[attack2AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[attack3AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[attack4AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[skillAnimClip.name].wrapMode = WrapMode.Once;


        AddAnimationEvent(attack1AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(attack2AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(attack3AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(attack4AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(skillAnimClip, "OnSkillAnimFinished");
    }
	
	// Update is called once per frame
	void Update () {
        Move();

        BodyDirectionChange();

        AnimationControl();

        CheckState();

        InputControl();

        ApplyGravity();

        AttackComPonentControl();
	}

    /// <summary>
    /// 이동 관련 함수
    /// </summary>
    void Move()
    {
        if (cannotMove == true) return;

        Transform cameraTransform = Camera.main.transform;
        // 카메라가 바라보는 방향이 월드상에서는 어떤 방향인가?
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x); // forward와 직교하는 벡터

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        // 우리가 이동하고자 하는 방향
        Vector3 targetDirection = horizontal * right + vertical * forward;
        // 현재 이동하는 방향에서 원하는 방향으로 조금씩 회전
        moveDirection = Vector3.RotateTowards(moveDirection, targetDirection,
            directionRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        // 방향이기 때문에 크기는 없애고 방향만 가져온다
        moveDirection = moveDirection.normalized;

        // 이동 속도
        float speed = moveSpeed;
        if(myState == FighterState.Run)
        {
            speed = runSpeed;
        }
        // 중력 벡터
        Vector3 gravityVec = new Vector3(0.0f, verticalSpeed, 0.0f);

        // 이번 프레임에 움직일 양
        Vector3 moveAmount = (moveDirection * speed * Time.deltaTime) + gravityVec;
        // 실제 이동
        collisionFlags = characterController.Move(moveAmount);


    }

    private void OnGUI()
    {
        // 충돌 정보
        GUILayout.Label("충돌 : " + collisionFlags.ToString());

        GUILayout.Label("현재 속도 : " + GetVelocitySpeed().ToString());
        // 캐릭터컨트롤러 컴포넌트를 찾았고, 현재 내 캐릭터의 이동속도가 0이 아니라면
        if(characterController != null && characterController.velocity != Vector3.zero)
        {
            // 현재 내 캐릭터가 이동하는 방향 (+크기)
            GUILayout.Label("current Velocity Vector : " + characterController.velocity.ToString());
            // 현재 내 속도
            GUILayout.Label("current Velocity Magnitude : " + characterController.velocity.magnitude.ToString());
        }
    }
    /// <summary>
    /// 현재 내 캐릭터의 이동속도를 얻기
    /// </summary>
    /// <returns></returns>
    float GetVelocitySpeed()
    {
        if(characterController.velocity == Vector3.zero)
        {
            currentVelocity = Vector3.zero;
        }
        else
        {
            Vector3 goalVelocity = characterController.velocity;
            goalVelocity.y = 0.0f;
            currentVelocity = Vector3.Lerp(currentVelocity, goalVelocity,
                velocityChangeSpeed * Time.fixedDeltaTime);
        }

        // currentVelocity의 크기를 리턴
        return currentVelocity.magnitude;
    }

    /// <summary>
    /// 몸통의 방향을 이동방향으로 
    /// </summary>
    void BodyDirectionChange()
    {
        if(GetVelocitySpeed() > 0.0f)
        {
            Vector3 newForward = characterController.velocity;
            newForward.y = 0.0f;
            transform.forward = Vector3.Lerp(transform.forward, newForward, bodyRotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 애니메이션을 재생시키는 함수
    /// </summary>
    /// <param name="clip"></param>
    void AnimationPlay(AnimationClip clip)
    {
        myAnimation.clip = clip;
        myAnimation.CrossFade(clip.name);
    }

    /// <summary>
    /// 내 상태에 맞추어 애니메이션 재생
    /// </summary>
    void AnimationControl()
    {
        switch (myState)
        {
            case FighterState.Idle:
                AnimationPlay(idleAnimClip);
                break;
            case FighterState.Walk:
                AnimationPlay(walkAnimClip);
                break;
            case FighterState.Run:
                AnimationPlay(runAnimClip);
                break;
            case FighterState.Attack:
                AttackAnimationControl();
                break;
            case FighterState.Skill:
                AnimationPlay(skillAnimClip);
                break;
        }
    }

    /// <summary>
    /// 상태를 변경해주는 함수
    /// </summary>
    void CheckState()
    {
        float currentSpeed = GetVelocitySpeed();
        switch (myState)
        {
            case FighterState.Idle:
                if(currentSpeed > 0.0f)
                {
                    myState = FighterState.Walk;
                }
                break;
            case FighterState.Walk:
                if(currentSpeed > 0.5f)
                {
                    myState = FighterState.Run;
                }
                else if (currentSpeed < 0.1f)
                {
                    myState = FighterState.Idle;
                }
                break;
            case FighterState.Run:
                if (currentSpeed < 0.5f)
                {
                    myState = FighterState.Walk;
                }
                if (currentSpeed < 0.1f)
                {
                    myState = FighterState.Idle;
                }
                break;
            case FighterState.Attack:
                cannotMove = true;
                break;
            case FighterState.Skill:
                cannotMove = true;
                break;
        }
    }

    /// <summary>
    /// 마우스 왼쪽 버튼으로 공격을 합니다
    /// </summary>
    void InputControl()
    {
        // 0 마우스 왼쪽, 1 오른쪽, 2 휠
        if(Input.GetMouseButtonDown(0) == true)
        {
            // 내가 공격중이 아니라면 공격 시작
            if(myState != FighterState.Attack)
            {
                myState = FighterState.Attack;
                attackState = FighterAttackState.Attack1;
            }
            else
            {
                // 공격 중이라면 애니메이션이 일정 이상 재생이 되었다면 다음 공격 활성화
                switch (attackState)
                {
                    case FighterAttackState.Attack1:
                        if(myAnimation[attack1AnimClip.name].normalizedTime > 0.1f)
                        {
                            nextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack2:
                        if(myAnimation[attack2AnimClip.name].normalizedTime > 0.1f)
                        {
                            nextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack3:
                        if (myAnimation[attack3AnimClip.name].normalizedTime > 0.1f)
                        {
                            nextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack4:
                        if (myAnimation[attack4AnimClip.name].normalizedTime > 0.1f)
                        {
                            nextAttack = true;
                        }
                        break;
                }
            }
        }
        if(Input.GetMouseButtonDown(1) == true)
        {
            if(myState == FighterState.Attack)
            {
                attackState = FighterAttackState.Attack1;
                nextAttack = false;
            }
            myState = FighterState.Skill;
        }
    }

    /// <summary>
    /// 공격 애니메이션 재생이 끝나면 호출되는 애니메이션 이벤트 함수
    /// </summary>
    void OnAttackAnimFinished()
    {
        if(nextAttack == true)
        {
            nextAttack = false;
            switch (attackState)
            {
                case FighterAttackState.Attack1:
                    attackState = FighterAttackState.Attack2;
                    break;
                case FighterAttackState.Attack2:
                    attackState = FighterAttackState.Attack3;
                    break;
                case FighterAttackState.Attack3:
                    attackState = FighterAttackState.Attack4;
                    break;
                case FighterAttackState.Attack4:
                    attackState = FighterAttackState.Attack1;
                    break;
            }
        }
        else
        {
            cannotMove = false;
            myState = FighterState.Idle;
            attackState = FighterAttackState.Attack1;
        }
    }

    void OnSkillAnimFinished()
    {
        Vector3 position = transform.position;
        position += transform.forward * 2.0f;
        Instantiate(skillEffect, position, Quaternion.identity);
        myState = FighterState.Idle;
    }
    
    /// <summary>
    /// 애니메이션 클립 재생이 끝날때쯤 애니메이션 이벤트 함수를 호출 시켜주도록 추가
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="FuncName"></param>
    void AddAnimationEvent(AnimationClip clip, string FuncName)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = FuncName;
        newEvent.time = clip.length - 0.1f; // 어느 타이밍에 호출할래?
        clip.AddEvent(newEvent);
    }

    /// <summary>
    /// 공격 애니메이션을 재생
    /// </summary>
    void AttackAnimationControl()
    {
        switch (attackState)
        {
            case FighterAttackState.Attack1:
                AnimationPlay(attack1AnimClip);
                break;
            case FighterAttackState.Attack2:
                AnimationPlay(attack2AnimClip);
                break;
            case FighterAttackState.Attack3:
                AnimationPlay(attack3AnimClip);
                break;
            case FighterAttackState.Attack4:
                AnimationPlay(attack4AnimClip);
                break;
        }
    }

    /// <summary>
    /// 중력적용
    /// </summary>
    void ApplyGravity()
    {
        // CollidedBelow가 세팅되었다면 => 바닥에 붙었다면
        if ((collisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            verticalSpeed = 0.0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// 공격 관련 컴포넌트 제어
    /// </summary>
    void AttackComPonentControl()
    {
        switch (myState)
        {
            case FighterState.Attack:
            case FighterState.Skill:
                attackTrailRenderer.enabled = true;
                attackCapsuleCollider.enabled = true;
                break;
            default:
                attackTrailRenderer.enabled = false;
                attackCapsuleCollider.enabled = false;
                break;

        }
    }
}

