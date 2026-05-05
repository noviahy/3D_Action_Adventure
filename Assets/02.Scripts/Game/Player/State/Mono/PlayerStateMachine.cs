using UnityEngine;

public class PlayerStateMachine : PlayerBehaviour
{
    public PlayerState currentState { get; private set; }

    public enum PlayerState
    {
        LocomotionState,
        ActionState,
        KnockbackState,
        DeadState,
    }
    private void Start()
    {
        currentState = PlayerState.LocomotionState;
    }
    public void ChangePlayerState(PlayerState state)
    {
        if (currentState == state) return;
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
        
        con.Animation.SetMoveX(con.Input.forward);
        con.Animation.SetMoveY(con.Input.side);

        if (con.Input.ActionPressed)
        {
            con.StateMachine.TryChangeState(PlayerState.ActionState);

            /*if (con.Input.InteractionPressed) // 아이템 종류 생각! 수정 필요
                 con.Player.ChangeWeaponType(Player.WeaponType.Bomb);*/

            if (con.Input.AttackPressed)
            {
                con.ActionState.TryChangeType(ActionState.ActionType.Attack);
            }

            if (con.Input.IsLockOn && con.Input.DodgeBuffered)
            {
                con.ActionState.TryChangeType(ActionState.ActionType.Dodge);
            }
        }
    }
}
