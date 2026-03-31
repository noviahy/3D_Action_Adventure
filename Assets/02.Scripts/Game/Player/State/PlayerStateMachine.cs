using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    public PlayerState currentState { get; private set; }

    public enum PlayerState
    {
        LocomotionState,
        ActionState,
        KnockbackState,
        DeadState,
    }
    private void ChangePlayerState(PlayerState state)
    {
        currentState = state;
    }

    public void TryChangeState(PlayerState state)
    {
        if (currentState == PlayerState.DeadState)
            return;
        if (currentState == PlayerState.KnockbackState)
            return;
        if (!controller.GroundCheck.IsGrounded)
            return;

        ChangePlayerState(state);
    }
    private void Update()
    {
        if (controller.Input) // 일단 보류
        {
            controller.StateMachine.TryChangeState(PlayerState.ActionState);
            return;
        }
        controller.StateMachine.TryChangeState(PlayerState.LocomotionState);
    }
}
