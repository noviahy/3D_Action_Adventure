using System.Collections;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class LocomotionState : PlayerBehaviour, IPlayerState
{
    public LocomotionSubState currentSubState { get; private set; }
    public LocomotionSubState preSubState { get; private set; }
    private Coroutine layerCoroutine;
    private Coroutine hangCoroutine;
    private Coroutine waitHangMove;
    private Coroutine mantleCoroutine;
    private RaycastHit wall;
    private Transform preWall;
    private float originRadius;
    private float originHeight;
    private float startHeight;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform HangCheck;

    private float sphereRadius = 0.3f;
    private float sphereDistance = 0.3f;
    private bool canClimb = true;

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
        // Debug.Log(state);
        // if(state == LocomotionSubState.Hang || currentSubState == LocomotionSubState.Hang)
        // Debug.Log($"{currentSubState} -> {state}");

        if (currentSubState == state)
            return;

        if (currentSubState == LocomotionSubState.Hang)
            preWall = wall.transform;

        if ((preSubState == LocomotionSubState.Airborne && state != LocomotionSubState.Hang)
            || preSubState == LocomotionSubState.Hang)
        {
            con.cc.radius = originRadius;
        }   

        if (state == LocomotionSubState.Airborne && con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
        {
            con.Movement.ResetYVelocity();
            startHeight = con.EdgeCheck.EdgeValue;
            con.Movement.SetStartY();

            if (currentSubState == LocomotionSubState.Run && startHeight >= 2.5)
            {
                con.Movement.StartJump();
            }
            if (currentSubState == LocomotionSubState.Walk || (currentSubState == LocomotionSubState.Run && startHeight <= 2.5))
            {
                if (startHeight > 3f)
                {
                    con.LayerController.RequestLayer1On(0.3f);

                    con.Animation.PlayFalling();
                }
            }
            if (currentSubState == LocomotionSubState.Hang)
                con.Animation.PlayFalling();

            originRadius = con.cc.radius;
            con.cc.radius = 0.05f;
        }

        if (state == LocomotionSubState.Hang)
        {
            if (layerCoroutine != null)
                StopCoroutine(layerCoroutine);

            con.Player.RequestWeaponRendererOff();

            if (!con.cc.isGrounded)
            {
                con.Animation.PlayHang();
                canClimb = true;

                waitHangMove = StartCoroutine(WaitHangMove());
            }
            if (con.cc.isGrounded)
            {
                canClimb = true;
                originRadius = con.cc.radius;
                con.cc.radius = 0.05f;
                waitHangMove = StartCoroutine(EnterJumpHang());
            }
            con.LayerController.RequestLayer1On(0.2f);
            con.LayerController.RequestLayer2Off(0.2f);
        }
        if (state == LocomotionSubState.Mantle && mantleCoroutine == null)
        {
            originRadius = con.cc.radius;
            con.cc.radius = 0.05f;
            con.cc.height = 0.05f;
            con.Animation.PlayMantle();
            mantleCoroutine = StartCoroutine(EnterMantle());
        }

        preSubState = currentSubState;
        currentSubState = state;
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


        if (currentSubState == LocomotionSubState.Idle || currentSubState == LocomotionSubState.Hang)
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
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Walk:
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Run:
                con.Movement.Move(con.Input.MoveInput);
                break;
            case LocomotionSubState.Airborne:
                if (!con.cc.isGrounded)
                {
                    if ((preSubState == LocomotionSubState.SlowWalk || preSubState == LocomotionSubState.Hang ||
                        con.Input.IsLockOn || con.ActionState.preType == ActionState.ActionType.Roll) &&
                        startHeight >= 2.49f)
                    {
                        if (CheckHang(out RaycastHit hit))
                        {
                            if (hit.transform.CompareTag("Hangable") || hit.transform.CompareTag("Climbable"))
                            {
                                if (hit.transform != preWall)
                                {
                                    ChangeState(LocomotionSubState.Hang);
                                    con.Movement.SetHanging(hit);
                                    wall = hit;
                                    return;
                                }
                            }
                        }
                    }
                    con.Movement.Airborne();
                    if (preSubState == LocomotionSubState.Walk || preSubState == LocomotionSubState.Run)
                        con.Movement.Move(con.Input.MoveInput);
                }
                if (con.cc.isGrounded && !con.Movement.hasLanded)
                {
                    con.Movement.ResetHasLand();
                    con.Movement.CheckFallDistance();
                }
                break;
            case LocomotionSubState.Hang:
                if (waitHangMove == null)
                {
                    if (con.Input.BackBuffered)
                    {
                        con.cc.Move(-con.Player.transform.forward * 0.3f);
                        ChangeState(LocomotionSubState.Airborne);
                    }
                    if (con.Input.forward >= 0.8f && canClimb)
                    {
                        canClimb = false;
                        con.cc.Move(-con.Player.transform.forward * 0.05f);

                        CheckHang(out RaycastHit hit);
                        wall = hit;

                        con.cc.Move(con.Player.transform.up * 0.4f);

                        con.Climb.SetTopPoint(wall);
                    }
                    if (con.Input.side >= 0.5)
                        RequestHangMove(1);
                    if (con.Input.side <= -0.5)
                        RequestHangMove(-1);
                }
                break;
        }

    }
    private void ChangeStateCode()
    {
        if (!con.cc.isGrounded && currentSubState != LocomotionSubState.Hang &&
            currentSubState != LocomotionSubState.Mantle &&
            con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
        {
            ChangeState(LocomotionSubState.Airborne);
            return;
        }
        if (con.BowAttack.BowAimed || con.BowAttack.Standby)
        {
            ChangeState(LocomotionSubState.Walk);
            return;
        }

        if (con.cc.isGrounded && currentSubState != LocomotionSubState.Hang)
        {
            if (con.Input.MoveInput != Vector3.zero)
            {
                if (currentSubState == LocomotionSubState.Airborne)
                    return;
                if (con.Input.RunPressed)
                {
                    ChangeState(LocomotionSubState.Run);
                    return;
                }
                if (currentSubState != LocomotionSubState.SlowWalk && con.Input.InputAmount <= 0.5f)
                {
                    ChangeState(LocomotionSubState.SlowWalk);
                    return;
                }
                if (currentSubState != LocomotionSubState.Walk && con.Input.InputAmount > 0.6f)
                {
                    ChangeState(LocomotionSubState.Walk);
                    return;
                }
            }
            if (con.Input.MoveInput == Vector3.zero && con.cc.isGrounded)
                ChangeState(LocomotionSubState.Idle);
        }

    }
    private bool CheckHang(out RaycastHit hit)
    {
        bool backHit = Physics.SphereCast(
            HangCheck.position,
            sphereRadius,
            -con.Player.transform.forward,
            out RaycastHit back,
            sphereDistance,
            wallLayer);

        bool frontHit = Physics.SphereCast(
            HangCheck.position,
            sphereRadius,
            con.Player.transform.forward,
            out RaycastHit front,
            sphereDistance,
            wallLayer);

        if (backHit && frontHit)
        {
            hit = back.distance < front.distance ? back : front;
            return true;
        }

        if (backHit)
        {
            hit = back;
            return true;
        }

        if (frontHit)
        {
            hit = front;
            return true;
        }

        hit = default;
        return false;
    }
    public void RequestOnCoroutine()
    {
        if (layerCoroutine != null)
            StopCoroutine(layerCoroutine);
        layerCoroutine = StartCoroutine(SetLendLayerOn());
    }
    public void RequestHangMove(float value)
    {
        if (hangCoroutine != null)
            return;
        hangCoroutine = StartCoroutine(HangMove(value));
    }
    // 절벽에서 떨어질때 사용하는 코드
    // Climb과 작동 방식은 비슷하지만
    // 에니메이션과 상태가 달라 따로 작성
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
    IEnumerator HangMove(float value)
    {
        if (value == 1)
            con.Animation.PlayHangRight();
        else
            con.Animation.PlayHangLeft();

        yield return new WaitForSeconds(0.3f);
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + con.Player.transform.right * value * 0.5f;

        float duration = 0.8f;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;

            float t = time / duration;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, t);

            Vector3 move = nextPos - transform.position;

            con.Movement.FallMove(move);

            yield return null;
        }
        hangCoroutine = null;
    }
    IEnumerator WaitHangMove()
    {
        yield return new WaitForSeconds(0.2f);
        waitHangMove = null;
    }
    IEnumerator EnterJumpHang()
    {
        con.cc.Move(con.Player.transform.up * 0.3f);
        con.RootMotionController.RequestRootMotion(true);
        con.Animation.PlayJumpHang();

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("JumpHang"));
        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);
        con.RootMotionController.RequestRootMotion(false);

        waitHangMove = null;
    }
    IEnumerator EnterMantle()
    {
        con.LayerController.RequestLayer1On(0.2f);
        con.LayerController.RequestLayer2Off(0.2f);
        con.RootMotionController.RequestRootMotion(true);
        con.Animation.PlayMantle();

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("PlayMantle"));
        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);
        con.RootMotionController.RequestRootMotion(false);

        con.LayerController.RequestLayer1Off(0.2f);
        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.LayerController.RequestLayer2On(0.2f);

        con.cc.radius = originRadius;
        con.cc.height = originHeight;
        ChangeState(LocomotionSubState.Idle);

        mantleCoroutine = null;
    }
}