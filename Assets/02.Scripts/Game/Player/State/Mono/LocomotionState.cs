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
        if (currentSubState == state) return;
        currentSubState = state;
    }
    private void Start()
    {
        currentSubState = LocomotionSubState.Idle;
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
    private void FixedUpdate()
    {
        if (con.Movement.JustLanded)
        {
            ChangeState(LocomotionSubState.Idle);
            con.Movement.ChangeJustLanded();
            Debug.Log("!");
        }

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
    }
}