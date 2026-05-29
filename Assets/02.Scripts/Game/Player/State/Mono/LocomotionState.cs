using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    public enum LocomotionSubState
    {
        Idle,
        Walk,
        Run,
    }
    public void ChangeState(LocomotionSubState state)
    {
        if (currentSubState == state) 
            return;
        currentSubState = state;
    }
    private void Start()
    {
        currentSubState = LocomotionSubState.Idle;
    }
    private void Update()
    {
        // Debug.Log(currentSubState);
        bool locomotion = 
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.LocomotionState;

        bool bow = con.BowAttack.BowAimed;

        bool climb = 
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.InteractionState && con.Climb.currentState == Climb.ClimbState.Climbing;

        if (!locomotion && !bow && !climb)
            return;

        // Land 확인 코드
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
            case LocomotionSubState.Idle:
                con.Animation.SetMove(0);
                break;
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput, true);
                break;
        }

        if (!con.GroundCheck.IsGrounded)
            return;

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
}