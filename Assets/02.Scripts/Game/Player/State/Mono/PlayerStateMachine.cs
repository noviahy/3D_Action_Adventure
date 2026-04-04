using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] PlayerController con;
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
        if (!con.GroundCheck.IsGrounded)
            return;

        ChangePlayerState(state);
    }
    private void Update() // Player AttackType도 여기서 변경
    {
        // 무기를 들고있는 상태이기 때문에 여기 넣어도 될 것 같음 
        // 딱히 액션이 아님
        if (currentState == PlayerState.DeadState)
            con.Dead.Dead();

        if (con.Input.LocomotionPressed)
        {
            if (con.Input.InteractionPressed) // 아이템 종류 생각! 수정 필요
                con.Player.ChangeAttackType(Player.AttackType.Bomb);

            con.StateMachine.TryChangeState(PlayerState.ActionState);
            return;
        }
        con.StateMachine.TryChangeState(PlayerState.LocomotionState);
    }
}
