using System.Collections;
using UnityEngine;

// 어느 순간 말 그대로 StateMachine이 되어버린 www
public class PlayerStateMachine : PlayerBehaviour
{
    public PlayerState currentState { get; private set; }
    public bool isLadder { get; private set; }
    public bool isCliff { get; private set; }
    public bool isMantle { get; private set; } = false;
    public bool isBox { get; private set; }
    public RaycastHit Box {  get; private set; }
    public bool JustLand { get; private set; } = false;
    public bool Climb { get; private set; } = false;

    [SerializeField] private Transform Head;
    [SerializeField] private LayerMask interactableLayer;
    private RaycastHit hit;
    private Vector3 actionNormal;
    private float actionTimer;
    private float actionDelay = 0.2f;

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
    // State 변경
    private void ChangePlayerState(PlayerState state)
    {
        if (currentState == state)
            return;

        // Locomotion상태가 아니라면 Idle로 변경 -> 에니메이션이 꺼지도록 함
        if (state != PlayerState.LocomotionState)
            con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Idle);
        currentState = state;
    }

    public void TryChangeState(PlayerState state)
    {
        if (currentState == PlayerState.DeadState)
            return;
        if (currentState == PlayerState.KnockbackState)
            return;
        // 공중에서는 Locomotion이 아닌 다른 상태로 들어갈 수 없음
        if (!con.cc.isGrounded && state != PlayerState.LocomotionState)
            return;

        ChangePlayerState(state);
    }
    private void Update()
    {
        if (currentState == PlayerState.DeadState)
            con.Dead.Dead();

        // 에니메이션 값 설정
        SetAnimationValue();

        // Ray 확인
        CheckRay();

        // 사다리 진입
        EnterClimb();

        // 이 밑으로 Hang이면 들어갈 수 없음
        if (con.Locomotion.currentSubState == LocomotionState.LocomotionSubState.Hang)
            return;

        // Roll 진입
        EnterRoll();

        // 이 밑으로 Airborne상태면 들어갈 수 없음
        if (con.Locomotion.currentSubState == LocomotionState.LocomotionSubState.Airborne)
            return;

        // 상자 진입
        EnterBox();

        // 벽잡기 진입 (Hang 진입)
        EnterHang();

        // 턱 진입 (Mantle 진입)
        EnterMantle();

        // Action 진입
        EnterAction();

        // 활 진입
        EnterBow();

        // 패링
        if (con.Input.ParryingPressed && con.Player.currentWeaponType == Player.WeaponType.Sword)
            con.ActionState.TryChangeType(ActionState.ActionType.Parrying);
    }
    private void SetAnimationValue()
    {
        con.Animation.SetGrounded(con.cc.isGrounded);
        con.Animation.SetMoveX(con.Input.forward);
        con.Animation.SetMoveY(con.Input.side);
    }
    public void RequestRoll()
    {
        JustLand = true;
    }
    private void CheckRay()
    {
        isLadder = false;
        isBox = false;
        isCliff = false;
        isMantle = false;

        if (Physics.Raycast(Head.position, transform.forward, out hit, 0.4f, interactableLayer))
        {
            if (hit.transform.CompareTag("Ladder"))
                isLadder = true;
            if (hit.transform.CompareTag("Moveable"))
            {
                Box = hit;
                isBox = true;
            }
            if (hit.transform.CompareTag("Climbable"))
            {
                actionNormal = hit.normal;
                isCliff = true;
            }
            if (hit.transform.CompareTag("Mantleable"))
            {
                actionNormal = hit.normal;
                isMantle = true;
            }
        }
    }
    private void EnterClimb()
    {
        // 기본상태 or 활 대기 상태가 아닌 상태에서 isLadder = true시 진입
        if (currentState == PlayerState.LocomotionState && !con.BowAttack.Standby && isLadder)
        {
            TryChangeState(PlayerState.InteractionState);

            // 2중 if문은 오류를 막기 위해 썼지만 이유가 기억나지 않음 있어도 문제는 아님
            if (currentState == PlayerState.InteractionState)
            {
                con.Climb.SetLadder(hit.transform);
                con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Climb);
            }
        }
    }
    private void EnterRoll()
    {
        // Roll상태에서 떨어질때
        if (con.ActionState.currentType == ActionState.ActionType.Roll && !con.GroundCheck.IsGrounded)
        {
            // Roll 중지 코드
            // Airborn -> Hang 가능하게 만들어줌
            con.Roll.RequestStopRoll();
        }

        // Roll 실행 코드
        // 프리룩 + 입력 + 쿨타임이 없을 때 + 땅에 있을때 + 기본 상태 
        // Airborn에서 구르기 호출할 때만
        if ((!con.Input.IsLockOn && con.Input.RollPressed && !con.Roll.isRollCoolTime && con.cc.isGrounded && currentState == PlayerState.LocomotionState) || JustLand)
        {
            TryChangeState(PlayerState.ActionState);
            con.ActionState.TryChangeType(ActionState.ActionType.Roll);
            JustLand = false;
            return;
        }

    }
    private void EnterAction()
    {
        // 입력 혹은 칼 공격 시
        if (con.Input.ActionPressed || (con.Input.AttackPressed && con.Player.currentWeaponType == Player.WeaponType.Sword))
        {
            // 기본 상태가 아니면 불가능
            if (currentState != PlayerState.LocomotionState)
                return;

            TryChangeState(PlayerState.ActionState);

            // 분기
            if (con.Input.AttackPressed)
                con.ActionState.TryChangeType(ActionState.ActionType.Attack);
            if (con.Input.IsLockOn && con.Input.DodgeBuffered)
                con.ActionState.TryChangeType(ActionState.ActionType.Dodge);
        }
    }
    private void EnterBow()
    {
        // 입력 + 기본 상태
        if (con.Input.BowCharging && currentState == PlayerState.LocomotionState)
        {
            con.ActionState.TryChangeType(ActionState.ActionType.Attack);
            // 무기 변경 후 진입
            if (con.Player.currentWeaponType == Player.WeaponType.Bow)
                TryChangeState(PlayerState.ActionState);
        }
    }
    private void EnterHang()
    {
        if (con.Locomotion.currentSubState == LocomotionState.LocomotionSubState.Hang)
            return;

        if (!isCliff || currentState != PlayerState.LocomotionState)
        {
            actionTimer = 0f;
            return;
        }

        if (con.Input.MoveInput.sqrMagnitude < 0.01f)
        {
            actionTimer = 0f;
            return;
        }

        Vector3 move = con.Input.MoveInput.normalized;

        float dot = Vector3.Dot(move, -actionNormal);

        if (dot > 0.9f)
        {
            actionTimer += Time.deltaTime;

            if (actionTimer >= actionDelay)
            {
                con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Hang);
                actionTimer = 0f;
            }
        }
        else
        {
            actionTimer = 0f;
        }
    }
    private void EnterMantle()
    {
        if (!isMantle || currentState != PlayerState.LocomotionState)
        {
            actionTimer = 0f;
            return;
        }

        if (con.Input.MoveInput.sqrMagnitude < 0.01f)
        {
            actionTimer = 0f;
            return;
        }

        Vector3 move = con.Input.MoveInput.normalized;

        float dot = Vector3.Dot(move, -actionNormal);

        if (dot > 0.9f)
        {
            actionTimer += Time.deltaTime;

            if (actionTimer >= actionDelay)
            {
                ChangePlayerState(PlayerState.LocomotionState);

                con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Mantle);
                actionTimer = 0f;
            }
        }
        else
        {
            actionTimer = 0f;
        }
    }
    private void EnterBox()
    {
        if (con.InteractionState.CurrentType == InteractionState.InteractionType.Box)
            return;

        if (!isBox || currentState != PlayerState.LocomotionState)
        {
            actionTimer = 0f;
            return;
        }

        Vector3 direction = con.Player.transform.forward;

        float dot = Vector3.Dot(direction, -actionNormal);

        if (dot > 0.8f)
        {
            if (con.Input.InteractionPressed)
            {
                ChangePlayerState(PlayerState.InteractionState);
                con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Box);
            }
        }
    }
}
