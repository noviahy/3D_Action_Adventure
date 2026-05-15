using System.Collections;
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
    private void ChangePlayerState(PlayerState state)
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
    private void Update()
    {
        if (currentState == PlayerState.DeadState)
            con.Dead.Dead();
        
        con.Animation.SetMoveX(con.Input.forward);
        con.Animation.SetMoveY(con.Input.side);

        // 여기선 다시 Locomotion으로 바꿔주지 않습니다
        // 다른 코드에서 상태가 끝날때 꼭 Locomotion으로 바꿔줘야한다는걸 기억해야해요
        if (con.Input.ActionPressed || (con.Input.AttackPressed && con.Player.currentWeaponType == Player.WeaponType.Sword))
        {
            TryChangeState(PlayerState.ActionState);
            /*if (con.Input.InteractionPressed) // 아이템 종류 생각! 수정 필요
                 con.Player.ChangeWeaponType(Player.WeaponType.Bomb);*/

            if (con.Input.AttackPressed)
                con.ActionState.TryChangeType(ActionState.ActionType.Attack);
            if (con.Input.IsLockOn && con.Input.DodgeBuffered)
                con.ActionState.TryChangeType(ActionState.ActionType.Dodge);
            if(con.Input.ParryingPressed && con.Player.currentWeaponType == Player.WeaponType.Sword)
                con.ActionState.TryChangeType(ActionState.ActionType.Parrying);
        }
        if (con.Input.BowCharging)
            con.ActionState.TryChangeType(ActionState.ActionType.Attack);
    }
}
