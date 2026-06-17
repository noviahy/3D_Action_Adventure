using System.Collections;
using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    private Coroutine coroutine;
    private float duration = 0.5f;

    public enum LocomotionSubState
    {
        Idle,
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
        
        // 다음 수정 부분
        // Update에 절때 두지 마
        if (state == LocomotionSubState.Airborne && con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
            coroutine = StartCoroutine(MoveToFall());


        currentSubState = state;
    }
    private void Start()
    {
        currentSubState = LocomotionSubState.Idle;
    }
    private void Update()
    {
        Debug.Log(currentSubState);
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
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput, true);
                break;
            case LocomotionSubState.Airborne:
                con.Movement.Airborne();
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

    IEnumerator MoveToFall()
    {
        Vector3 startPos = con.Player.transform.position;
        Vector3 targetPos = startPos + con.Player.transform.forward * 0.5f;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            con.Movement.FallMove(pos);
            yield return null;
        }
    }
}