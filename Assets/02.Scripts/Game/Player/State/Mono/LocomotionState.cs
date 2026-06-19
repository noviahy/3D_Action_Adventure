using System.Collections;
using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    private Coroutine moveCoroutine;
    private Coroutine layerCoroutine;
    private float duration = 0.5f;

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

        if (state == LocomotionSubState.Airborne && !con.Movement.isJumping)
            con.Animation.PlayFalling();

        if (state == LocomotionSubState.Airborne && con.InteractionState.CurrentType != InteractionState.InteractionType.Climb && moveCoroutine == null)
        {
            // moveCoroutine = StartCoroutine(MoveToFall());
            layerCoroutine = StartCoroutine(SetAirborneLayerOn());
        }

        currentSubState = state;
    }
    private void Start()
    {
        currentSubState = LocomotionSubState.Idle;
    }
    private void Update()
    {
        //Debug.Log(con.ActionState.currentType);
        //Debug.Log(con.StateMachine.currentState);
        bool locomotion =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.LocomotionState;

        bool bow = con.BowAttack.BowAimed;

        bool climb =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.InteractionState && con.Climb.currentState == Climb.ClimbState.Climbing;

        if (currentSubState == LocomotionSubState.Idle)
            con.Animation.SetMove(0);

        if (!locomotion && !bow && !climb)
            return;

        // Jump Land 확인 코드
        if (con.Movement.JustLanded)
        {
            ChangeState(LocomotionSubState.Idle);
            con.Movement.ChangeJustLanded();
        }

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
                con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput, true);
                break;
            case LocomotionSubState.Airborne:
                con.Movement.Airborne();
                if (con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
                    con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Hang:
                break;
        }

        if (!con.GroundCheck.IsGrounded && currentSubState != LocomotionSubState.Hang)
        {
            ChangeState(LocomotionSubState.Airborne);
            return;
        }

        if (con.Input.MoveInput != Vector3.zero)
        {
            if (con.Input.RunPressed)
            {
                ChangeState(LocomotionSubState.Run);
                return;
            }
            if (currentSubState != LocomotionSubState.Walk)
            {
                ChangeState(LocomotionSubState.Walk);
                return;
            }
        }
        if (con.Input.MoveInput == Vector3.zero)
            ChangeState(LocomotionSubState.Idle);
    }

    public void RequestOnCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);
        layerCoroutine = StartCoroutine(SetAirborneLayerOn());
    }
    public void RequestOffCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);

        layerCoroutine = StartCoroutine(SetAirborneLayerOff());
    }
    IEnumerator SetAirborneLayerOn()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(1, time);
            yield return null;
        }
        layerCoroutine = null;
    }
    IEnumerator SetAirborneLayerOff()
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