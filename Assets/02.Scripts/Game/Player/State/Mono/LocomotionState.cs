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
    public void Init()
    {

    }
    public void ChangeState(LocomotionSubState state)
    {
        currentSubState = state;
    }
    private void Update()
    {
        if (!con.GroundCheck.IsGrounded)
            return;

        con.Animation.SetMoveX(con.Input.forward);
        con.Animation.SetMoveY(con.Input.side);

        if (con.Input.MoveInput != Vector3.zero)
        {
            if (con.Input.RunPressed)
                ChangeState(LocomotionSubState.Run);
            else
                ChangeState(LocomotionSubState.Walk);
        }
        else
            ChangeState(LocomotionSubState.Idle);
    }
    private void FixedUpdate()
    {
        switch (currentSubState)
        {
            case LocomotionSubState.Idle:
                break;
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput, false);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput, true);
                break;
        }
        if (con.Movement.JustLanded)
        {
            ChangeState(LocomotionSubState.Idle);
            con.Movement.ChangeJustLanded();
        }
    }
}