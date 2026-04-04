using UnityEngine;

public class LocomotionState : MonoBehaviour, IPlayerState
{
    [SerializeField] PlayerController con;

    public LocomotionSubState currentSubState { get; private set; }
    public enum LocomotionSubState
    {
        Idle,
        Walk,
        Run,
    }
    public void ChangeState(LocomotionSubState state)
    {
        currentSubState = state;
    }
    private void Update()
    {
        if (!con.GroundCheck.IsGrounded)
            return;

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