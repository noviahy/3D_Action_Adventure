using System.Collections;
using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    public LocomotionSubState preSubState { get; private set; }
    private Coroutine layerCoroutine;
    private float originRadius;
    public enum LocomotionSubState
    {
        Idle,
        SlowWalk,
        Walk,
        Run,
        Airborne,
        Hang
    }
    public void ChangeState(LocomotionSubState state)
    {
        if (currentSubState == state)
            return;

        if(preSubState == LocomotionSubState.Airborne)
        {
            con.cc.radius = originRadius;
        }

        if (state == LocomotionSubState.Airborne && con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
        {
            con.Movement.SetStartY();

            if (currentSubState == LocomotionState.LocomotionSubState.Run)
            {
                con.Movement.StartJump();
            }
            if(currentSubState == LocomotionSubState.Walk)
            {
                if (con.EdgeCheck.EdgeValue > 1f)
                {
                    StartCoroutine(SetJumpLayer());
                    con.Animation.PlayFalling();
                }
            }
            originRadius = con.cc.radius;
            con.cc.radius = 0.05f;
        }
        preSubState = currentSubState;
        currentSubState = state;
    }
    private void Start()
    {
        currentSubState = LocomotionSubState.Idle;
    }
 
    private void Update()
    {
        Debug.Log(currentSubState);
        //Debug.Log(con.ActionState.currentType);
        //Debug.Log(con.StateMachine.currentState);
        Debug.Log(con.cc.isGrounded);
        bool locomotion =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.LocomotionState;

        bool bow = con.BowAttack.BowAimed;

        bool climb =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.InteractionState && con.Climb.currentState == Climb.ClimbState.Climbing;

        if (currentSubState == LocomotionSubState.Idle)
            con.Animation.SetMove(0);

        if (!locomotion && !bow && !climb)
            return;

        // 사다리 코드
        if (con.Climb.currentState == Climb.ClimbState.Climbing)
        {
            con.Movement.Climb(con.Input.forward, con.Input.RunPressed);
            return;
        }

        // 나머지 이동 코드
        switch (currentSubState)
        {
            case LocomotionSubState.SlowWalk:
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Airborne:
                if (!con.cc.isGrounded)
                {
                    con.Movement.Airborne();
                    if (!con.Movement.isJumping)
                        con.Movement.Move(con.Input.MoveInput);
                }
                if ((con.cc.isGrounded || con.GroundCheck.IsGrounded) && !con.Movement.hasLanded)
                {
                    con.Movement.ResetHasLand();
                    con.Movement.CheckFallDistance();
                    ChangeState(LocomotionSubState.Idle);
                }
                break;
            case LocomotionSubState.Hang:
                break;
        }

        if (!con.cc.isGrounded && currentSubState != LocomotionSubState.Hang && con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
        {
            ChangeState(LocomotionSubState.Airborne);
            return;
        }

        if (currentSubState != LocomotionSubState.Airborne)
        {
            if (con.Input.MoveInput != Vector3.zero)
            {
                if (currentSubState == LocomotionSubState.Airborne)
                    return;
                if (con.Input.RunPressed)
                {
                    ChangeState(LocomotionSubState.Run);
                    return;
                }
                if (currentSubState != LocomotionSubState.SlowWalk && con.Input.InputAmount <= 0.6f)
                {
                    ChangeState(LocomotionSubState.SlowWalk);
                    return;
                }
                if (currentSubState != LocomotionSubState.Walk && con.Input.InputAmount > 0.6f)
                {
                    ChangeState(LocomotionSubState.Walk);
                    return;
                }
            }
            if (con.Input.MoveInput == Vector3.zero)
                ChangeState(LocomotionSubState.Idle);
        }
    }

    public void RequestOnCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);
        layerCoroutine = StartCoroutine(SetLendLayerOn());
    }
    public void RequestOffCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);

        layerCoroutine = StartCoroutine(SetClimbLayerOff());
    }
    public void RequestJumpCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);
        layerCoroutine = StartCoroutine(SetJumpLayer());
    }
    IEnumerator SetJumpLayer()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(1, time);
            yield return null;
        }
    }
    // 절벽에서 떨어질때 사용하는 코드
    // Climb과 작동 방식은 비슷하지만
    // 에니메이션과 상태가 달라 따로 작성
    IEnumerator SetLendLayerOn()
    {
        float time = 0f;
        if (preSubState != LocomotionSubState.Walk)
        {
            while (time < 1f)
            {
                time += Time.deltaTime * 3f;
                con.Animation.SetLayerWeight(1, time);
                yield return null;
            }
        }

        // 에니메이션은 CheckFallDistance함수에서 설정
        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);

        time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(1, 1 - time);
            yield return null;
        }

        layerCoroutine = null;
        ChangeState(LocomotionSubState.Idle);
    }
    // Climb에서만 호출하는 코드
    // 착지 후 레이어를 끔
    // Climb 종료
    IEnumerator SetClimbLayerOff()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(1, 1 - time);
            yield return null;
        }
        layerCoroutine = null;
    }
}