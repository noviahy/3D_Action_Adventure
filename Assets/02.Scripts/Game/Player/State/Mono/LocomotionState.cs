using System.Collections;
using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    public LocomotionSubState preSubState { get; private set; }
    private Coroutine layerCoroutine;
    public float originRadius {  get; private set; }
    public float originHeight { get; private set; }

    public enum LocomotionSubState
    {
        Idle,
        SlowWalk,
        Walk,
        Run,
        Airborne,
        Hang,
        Mantle
    }
    public void ChangeState(LocomotionSubState state)
    {
        if (currentSubState == state)
            return;
        // Debug.Log($"{currentSubState} -> {state}");

        ExitState(currentSubState, state);

        preSubState = currentSubState;
        currentSubState = state;

        EnterState(preSubState ,state);
    }
    private void ExitState(LocomotionSubState from, LocomotionSubState to)
    {
        if (from == LocomotionSubState.Hang)
            con.Hang.SetPreWall();

        if ((from == LocomotionSubState.Airborne && to != LocomotionSubState.Hang)
            || from == LocomotionSubState.Hang)
        {
            con.cc.radius = originRadius;
        }
    }
    private void EnterState(LocomotionSubState from, LocomotionSubState to)
    {
        switch (to)
        {
            case LocomotionSubState.Airborne:
                con.Airborn.Enter(from);
                originRadius = con.cc.radius;
                con.cc.radius = 0.05f;
                break;

            case LocomotionSubState.Hang:
                con.Hang.Enter();
                if (layerCoroutine != null)
                    StopCoroutine(layerCoroutine);
                if (con.cc.isGrounded)
                {
                    originRadius = con.cc.radius;
                    con.cc.radius = 0.05f;
                }
                break;

            case LocomotionSubState.Mantle:
                con.Mantle.Enter();
                originRadius = con.cc.radius;
                originHeight = con.cc.height;
                con.cc.radius = 0.05f;
                con.cc.height = 0.05f;
                break;
        }
    }
    private void Awake()
    {
        currentSubState = LocomotionSubState.Idle;
    }

    private void Update()
    {
        //Debug.Log(currentSubState);
        //Debug.Log(con.ActionState.currentType);
        //Debug.Log(con.StateMachine.currentState);
        //Debug.Log(con.cc.isGrounded);

        bool locomotion =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.LocomotionState;

        bool bow = con.BowAttack.BowAimed;

        bool climb =
            con.StateMachine.currentState == PlayerStateMachine.PlayerState.InteractionState && con.Climb.currentState == Climb.ClimbState.Climbing;


        if (currentSubState == LocomotionSubState.Idle || 
            currentSubState == LocomotionSubState.Hang || 
            currentSubState == LocomotionSubState.Mantle)
            con.Animation.SetMove(0);

        if (!locomotion && !bow && !climb)
            return;

        // 사다리 코드
        if (con.Climb.currentState == Climb.ClimbState.Climbing)
        {
            con.Movement.Climb(con.Input.forward, con.Input.RunPressed);
            return;
        }

        // 나머지 이동 코드
        PlayStateCode();

        // 상태 변경 코드
        ChangeStateCode();
    }
    private void PlayStateCode()
    {
        switch (currentSubState)
        {
            case LocomotionSubState.SlowWalk:
            case LocomotionSubState.Walk:
            case LocomotionSubState.Run:
                PlayGround();
                break;
            case LocomotionSubState.Airborne:
                PlayAirborne();
                break;
            case LocomotionSubState.Hang:
                PlayHang();
                break;
        }
    }
    private void PlayGround()
    {
        con.Movement.Move(con.Input.MoveInput);
    }
    private void PlayHang()
    {
        con.Hang.UpdateHang();
    }
    private void PlayAirborne()
    {
        if (!con.cc.isGrounded)
        {
            if (TryHang())
                return;

            con.Movement.Airborne();

            if (preSubState == LocomotionSubState.Walk ||
                preSubState == LocomotionSubState.Run)
            {
                con.Movement.Move(con.Input.MoveInput);
            }
        }

        CheckLanding();
    }
    private void CheckLanding()
    {
        if (!con.cc.isGrounded)
            return;

        if (con.Movement.hasLanded)
            return;

        con.Movement.ResetHasLand();
        con.Movement.CheckFallDistance();
    }
    private bool TryHang()
    {
        bool canHang =
        (preSubState == LocomotionSubState.SlowWalk ||
         preSubState == LocomotionSubState.Hang ||
         con.Input.IsLockOn ||
         con.ActionState.preType == ActionState.ActionType.Roll)
        && con.Airborn.startHeight >= 2.49f;

        if (!canHang)
            return false;

        if (!con.Hang.TryEnterHang())
            return false;

        ChangeState(LocomotionSubState.Hang);
        return true;
    }
    private void ChangeStateCode()
    {
        if (currentSubState == LocomotionSubState.Hang || currentSubState == LocomotionSubState.Mantle)
            return;

        if (con.Airborn.TryEnterAirborne())
            return;

        if (con.BowAttack.BowAimed || con.BowAttack.Standby)
        {
            ChangeState(LocomotionSubState.Walk);
            return;
        }

        TryGroundMoveState();
    }
    private void TryGroundMoveState()
    {
        if (!con.cc.isGrounded)
            return;

        if (con.Input.MoveInput == Vector3.zero)
        {
            ChangeState(LocomotionSubState.Idle);
            return;
        }

        if (currentSubState == LocomotionSubState.Airborne)
            return;

        if (con.Input.RunPressed)
        {
            ChangeState(LocomotionSubState.Run);
            return;
        }

        if (con.Input.InputAmount <= 0.5f)
        {
            if (currentSubState != LocomotionSubState.SlowWalk)
                ChangeState(LocomotionSubState.SlowWalk);

            return;
        }

        if (currentSubState != LocomotionSubState.Walk)
            ChangeState(LocomotionSubState.Walk);
    }

    // 절벽에서 떨어질때 사용하는 코드
    // Climb과 작동 방식은 비슷하지만
    // 에니메이션과 상태가 달라 따로 작성
    public void RequestOnCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);
        layerCoroutine = StartCoroutine(SetLendLayerOn());
    }
    IEnumerator SetLendLayerOn()
    {
        con.LayerController.RequestLayer1On(0.3f);

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("LandSoft"));
        // 에니메이션은 CheckFallDistance함수에서 설정
        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);

        con.Player.RequestWeaponRendererOn();
        con.LayerController.RequestLayer1Off(0.2f);
        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.LayerController.RequestLayer2On(0.3f);

        layerCoroutine = null;
        ChangeState(LocomotionSubState.Idle);
    }
}