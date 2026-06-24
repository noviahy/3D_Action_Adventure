using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static PlayerStateMachine;

public class Climb : PlayerBehaviour
{
    // 벽에 어떻게 붙을건지 생각좀 해봐야 할듯
    // 움직임은 PlayerMovement 코드 에서 -> 나머지 Interaction이 중간에 Locomotion이 필요하기 때문에
    // 여기에 움직임 코드를 짜줄수도 있지만 통일성을 위해 PlayerMovement로 통일
    //[SerializeField] private Transform player;
    private Coroutine enterCoroutine;
    private Coroutine fallingCoroutine;
    private Coroutine arriveCoroutine;

    private Transform currentLadder;
    private Transform TopArrivePoint;
    private Vector3 arriveTargetPos;
    private Transform EnterPoint;

    public ClimbState currentState { get; private set; }
    public enum ClimbState
    {
        Idle,
        Enter,
        Climbing,
        Falling,
        ArriveTop,
        ArriveBottom
    }
    // 내부 호출
    private void ChangeClimbState(ClimbState state)
    {
        if (currentState == state)
            return;

        currentState = state;
        switch (currentState)
        {
            case ClimbState.Idle:
                break;
            case ClimbState.Enter:
                break;
            case ClimbState.Climbing:
                break;
            case ClimbState.Falling:
                fallingCoroutine = StartCoroutine(FallingCoroutine());
                break;
            case ClimbState.ArriveTop:
                arriveCoroutine = StartCoroutine(ArriveTopCoroutine());
                break;
            case ClimbState.ArriveBottom:
                arriveCoroutine = StartCoroutine(ArriveBottomCoroutine());
                break;
        }
    }
    public void Enter()
    {
        ChangeClimbState(ClimbState.Enter);
        if (enterCoroutine == null)
            enterCoroutine = StartCoroutine(EnterClimbing());
    }
    IEnumerator EnterClimbing()
    {
        if (con.Player.currentWeaponType != Player.WeaponType.Default)
        {
            con.Player.ChangeWeaponType(Player.WeaponType.Default);

            yield return new WaitForSeconds(0.3f);
        }
        con.Animation.PlayClimbing();

        Vector3 startPos = transform.position;
        Vector3 targetPos = EnterPoint.position;

        Vector3 dir = -currentLadder.forward;
        dir.y = 0;

        float time = 0;

        while (time <= 1)
        {
            time += Time.deltaTime * 10f;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, time);

            Vector3 move = nextPos - transform.position;
            con.Movement.FallMove(move);

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);

                con.Player.transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 8f * Time.deltaTime);
            }

            yield return null;
        }

        time = 0;
        while (time <= 1)
        {
            float delta = Time.deltaTime * 6.5f;
            time += delta;

            con.Animation.SetLayerWeight(1, time);

            // 총 0.15 이동하도록 분배
            Vector3 move = Vector3.up * 0.15f * delta;
            con.Movement.FallMove(move);

            yield return null;
        }
        enterCoroutine = null;

        // Enter 종료 후 Climb으로 상태 변경
        ChangeClimbState(ClimbState.Climbing);
    }

    private void Update()
    {
        // Debug.Log(currentState);
        // Debug.Log(con.GroundCheck.IsGrounded);
        if (con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
            return;
        if (con.Locomotion.currentSubState == LocomotionState.LocomotionSubState.Hang)
            return;

        if (currentState != ClimbState.Climbing)
            return;

        if (!con.StateMachine.isLadder)
        {
            arriveTargetPos = TopArrivePoint.position;
            ChangeClimbState(ClimbState.ArriveTop);
            return;
        }
        if (con.cc.isGrounded)
        {
            ChangeClimbState(ClimbState.ArriveBottom);
            return;
        }

        if (con.Input.BackBuffered && currentState == ClimbState.Climbing)
        {
            ChangeClimbState(ClimbState.Falling);
            return;
        }
    }
    IEnumerator FallingCoroutine()
    {
        con.Movement.ResetYVelocity();
        con.Animation.PlayFalling();

        Vector3 move = -con.Player.transform.forward * 0.15f;
        con.Movement.FallMove(move);

        while (!con.cc.isGrounded)
        {
            con.Movement.AirborneClimb();
            yield return null;
        }

        // 랜딩 애니메이션 대기
        yield return new WaitForSeconds(0.3f);

        fallingCoroutine = null;
        Finish();
    }
    IEnumerator ArriveTopCoroutine()
    {
        // 충돌 문제 해결을 위한 코드
        // 대각선으로 이동하면 앞으로 나갈수가 없어서 튐
        float originRadius = con.cc.radius;
        float originHeight = con.cc.height;
        con.cc.radius = 0.1f;
        con.cc.height = 0.1f;

        // RootMotion 사용 코드
        con.RootMotionController.RequestRootMotion(true);
        con.Animation.PlayArrive();
        yield return null;

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("Arrive"));

        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);

        con.RootMotionController.RequestRootMotion(false);


        Vector3 startPos = transform.position;
        Vector3 targetPos = arriveTargetPos - con.cc.center + Vector3.up * (originHeight * 0.7f);
        Vector3 prevPos = startPos;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 3f;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, time);
            con.cc.Move(nextPos - prevPos);

            prevPos = nextPos;
            yield return null;
        }

        con.cc.radius = originRadius;
        con.cc.height = originHeight;


        while (!con.cc.isGrounded)
        {
            con.Movement.Airborne();

            yield return null;
        }

        if (con.StateMachine.currentState == PlayerState.LocomotionState)
        {
            con.Player.RequestWeaponRendererOn();
        }

        time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 3f;

            con.Animation.SetLayerWeight(1, 1 - time);
            if (con.StateMachine.currentState == PlayerState.LocomotionState)
                con.Animation.SetLayerWeight(2, time);
            yield return null;
        }

        Finish();
        con.Animation.SetLayerWeight(1, 0);
        arriveCoroutine = null;
    }
    IEnumerator ArriveBottomCoroutine()
    {
        float time = 0;
        while (time <= 1)
        {
            float delta = Time.deltaTime * 2f;
            time += delta;

            Vector3 move = -con.Player.transform.forward * 0.2f * delta;
            con.Movement.FallMove(move);

            con.Animation.SetLayerWeight(1, 1 - time);

            yield return null;
        }

        while (con.cc.isGrounded)
        {
            con.Movement.AirborneClimb();
            yield return null;
        }

        Finish();
        con.Animation.SetLayerWeight(1, 0);
        arriveCoroutine = null;
    }
    public void SetLadder(Transform ladder)
    {
        currentLadder = ladder;
        TopArrivePoint = ladder.Find("TopArrivePoint");
        EnterPoint = ladder.transform.Find("EnterPoint");
    }
    public void SetTopPoint(RaycastHit hangHit)
    {
        // con.cc.Move(Vector3.up * 1f);
        arriveTargetPos = hangHit.point + Vector3.up * 0.6f - hangHit.normal * 0.7f;
        StartCoroutine(ArriveTopCoroutine());
    }
    // 도착 혹은 떨어져 착지 전까지 Climb State 유지
    public void Finish()
    {
        ChangeClimbState(ClimbState.Idle);
        con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Idle);
        con.StateMachine.TryChangeState(PlayerState.LocomotionState);
        con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Idle);
    }
    public void Exit()
    {

    }
}
