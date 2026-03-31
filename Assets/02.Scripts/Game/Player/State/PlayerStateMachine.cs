using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] InputManager input;
    [SerializeField] DeadState deadState;
    public PlayerState currentState { get; private set; }

    public enum PlayerState
    {
        LocomotionState,
        ActionState,
        KnockbackState,
        DeadState,
    }
    public void ChangePlayerState(PlayerState state)
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
    private void Update() // Player AttackType도 여기서 변경
    {
        // 무기를 들고있는 상태이기 때문에 여기 넣어도 될 것 같음 
        // 딱히 액션이 아님

        if (input.AttackPressed || input.ParryingPressed || input.RollPressed || input.BowCharging || input.InteractionPressed) // 일단 보류
        {
            if (input.InteractionPressed) // 아이템 종류 생각! 수정 필요
                controller.Player.ChangeAttackType(Player.AttackType.Bomb);
            controller.StateMachine.TryChangeState(PlayerState.ActionState);
            return;
        }
        controller.StateMachine.TryChangeState(PlayerState.LocomotionState);

        if (currentState == PlayerState.DeadState)
            deadState.Dead();
    }
}
