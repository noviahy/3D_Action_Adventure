using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerStateMachine : PlayerBehaviour
{
    public PlayerState currentState { get; private set; }
    public bool isLadder { get; private set; }
    [SerializeField] private LayerMask LadderMask;

    public enum PlayerState
    {
        LocomotionState,
        ActionState,
        InteractionState,
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
    // 주로 여기서 Action State 세부 사항을 변경해주는듯
    // 물론 여기서 하지 않고 ActionState나 하위 코드에서 해줘도 되겠지만 딱히 이 코드에서 이걸 빼면 하는게 없음
    // 또한 ActionState은 순수c#이고 다른 하위 코드에선 변경보다 변경값을 가지고 실행을 하는걸 맡고 있기때문에 여기에서 변경해줘도 될 것 같음
    // Action 빼면 아무것도 안 남음 DeadState는 마지막에 확인하는거고 Knockback도 한번만 바꿔주면 되는거고
    private void Update()
    {
        if (con.Input.CurrentInputMode == InputManager.InputMode.UI || con.Input.CurrentInputMode == InputManager.InputMode.Dialogue)
            return;

        if (currentState == PlayerState.DeadState)
            con.Dead.Dead();

        con.Animation.SetMoveX(con.Input.forward);
        con.Animation.SetMoveY(con.Input.side);

        // 여기선 다시 Locomotion으로 바꿔주지 않습니다
        // 다른 코드에서 상태가 끝날때 꼭 Locomotion으로 바꿔줘야한다는걸 기억해야해요
        if (con.Input.ActionPressed || (con.Input.AttackPressed && con.Player.currentWeaponType == Player.WeaponType.Sword))
        {
            TryChangeState(PlayerState.ActionState);

            if (con.Input.AttackPressed)
                con.ActionState.TryChangeType(ActionState.ActionType.Attack);
            if (con.Input.IsLockOn && con.Input.DodgeBuffered)
                con.ActionState.TryChangeType(ActionState.ActionType.Dodge);
        }
        // 여기 지금 상태가 Action으로 안 넘어가고 있지 않아/
        // 저 안에 넣지 않은 이유가 분명히 있지만 왜인진 모름, 시간날떄 이유를 찾아야겠음
        // 활 공격
        if (con.Input.BowCharging)
            con.ActionState.TryChangeType(ActionState.ActionType.Attack);
        // 패링
        if (con.Input.ParryingPressed && con.Player.currentWeaponType == Player.WeaponType.Sword)
            con.ActionState.TryChangeType(ActionState.ActionType.Parrying);

        // 레이케스트를 던져서 앞에 사다리가 아직도 있는지 확인
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        isLadder = Physics.Raycast(origin, transform.forward, 0.3f, LadderMask);
        if (currentState == PlayerState.LocomotionState && isLadder)
        {
            TryChangeState(PlayerState.InteractionState);

            if (currentState == PlayerState.InteractionState)
                con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Climb);
        }

    }
}
