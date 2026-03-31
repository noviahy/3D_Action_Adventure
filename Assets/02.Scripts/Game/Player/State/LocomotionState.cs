using UnityEngine;

public class LocomotionState : MonoBehaviour, IPlayerState
{
    [SerializeField] PlayerController controller;
    [SerializeField] InputManager input;
    public LocomotionSubState currentSubState { get; private set; }
    public enum LocomotionSubState
    {
        Idle,
        Walk,
        Run,
        Jump // Jump는 Trigger로 처리
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
            {
                ChangeState(LocomotionSubState.Run);
            }
            else
            {
                ChangeState(LocomotionSubState.Walk);
            }
        }
        else
        {
            ChangeState(LocomotionSubState.Idle);
        }
    }
    private void FixedUpdate()
    {
        switch (currentSubState)
        {
            case LocomotionSubState.Idle:
                break;
            case LocomotionSubState.Walk:
                controller.Movement.Walk(input.MoveInput);
                break;
            case LocomotionSubState.Run:
                controller.Movement.Run(input.MoveInput);
                break;
            case LocomotionSubState.Jump:
                controller.Movement.Jump();
                break;
        }
    }
}