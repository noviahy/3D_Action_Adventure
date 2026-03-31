using UnityEngine;

public class LocomotionState : MonoBehaviour, IPlayerState
{
    [SerializeField] PlayerController controller;
    [SerializeField] InputManager input;
    [SerializeField] PlayerMovement move;
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

        if (!controller.GroundCheck.IsGrounded)
            return;

        if (input.MoveInput != Vector3.zero)
        {
            if (input.RunPressed)
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
                controller.Movement.Move(input.MoveInput, false);
                break;
            case LocomotionSubState.Run:
                controller.Movement.Move(input.MoveInput, true);
                break;
        }
        if (move.JustLanded)
        {
            ChangeState(LocomotionSubState.Idle);
            move.ChangeJustLanded();
        }
    }
}