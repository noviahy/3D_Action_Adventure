using System.Collections;
using UnityEngine;
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

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.Player.ChangeWeaponType(Player.WeaponType.Default);
    }
    IEnumerator EnterClimbing()
    {
        con.Animation.PlayClimbing();

        Vector3 startPos = transform.position;
        Vector3 targetPos = EnterPoint.position;

        Vector3 dir = -currentLadder.forward;
        dir.y = 0;

        float time = 0;

        while (time <= 1)
        {
            time += Time.deltaTime * 6f;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, time);

            Vector3 move = nextPos - transform.position;
            con.Movement.ClimbMove(move);

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
            con.Movement.ClimbMove(move);

            yield return null;
        }
        enterCoroutine = null;

        // Enter 종료 후 Climb으로 상태 변경
        ChangeClimbState(ClimbState.Climbing);
    }

    private void Update()
    {
        // Debug.Log(currentState);
        Debug.Log(con.GroundCheck.IsGrounded);
        if (con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
            return;

        if (currentState != ClimbState.Climbing)
            return;

        if (!con.StateMachine.isLadder)
        {
            ChangeClimbState(ClimbState.ArriveTop);
            return;
        }
        if (con.GroundCheck.IsGrounded)
        {
            ChangeClimbState(ClimbState.ArriveBottom);
            return;
        }

        if (con.Input.BackBuffered)
        {
            ChangeClimbState(ClimbState.Falling);
            return;
        }
    }
    IEnumerator FallingCoroutine()
    {
        con.Movement.ResetYVelocity();
        con.Animation.PlayFalling();

        Vector3 move = Vector3.back * 0.15f;
        con.Movement.ClimbMove(move);

        while (!con.GroundCheck.IsGrounded)
        {
            con.Movement.Falling();
            yield return null;
        }

        // 랜딩 애니메이션 대기
        yield return new WaitForSeconds(0.3f);

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 4f;
            con.Animation.SetLayerWeight(1, 1 - time);
            yield return null;
        }

        fallingCoroutine = null;
        Finish();
    }

    IEnumerator ArriveTopCoroutine()
    {
        con.Animation.PlayArrive();

        Vector3 startPos = transform.position;

        Vector3 targetPos = TopArrivePoint.position;

        float time = 0;
        while (time <= 1)
        {
            time += Time.deltaTime * 2f;

            float curve = Mathf.Sin(time * Mathf.PI * 0.5f);

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, curve);

            Vector3 move = nextPos - transform.position;

            con.Movement.ClimbMove(move);

            yield return null;
        }

        time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 4f;
            con.Animation.SetLayerWeight(1, 1 - time);
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

            Vector3 move = Vector3.back * 0.1f * delta;
            con.Movement.ClimbMove(move);

            con.Animation.SetLayerWeight(1, 1 - time);

            yield return null;
        }

        Finish();
        con.Animation.SetLayerWeight(1, 0);
        arriveCoroutine = null;
    }
    public void SetLadder(Transform ladder)
    {
        currentLadder = ladder;
        TopArrivePoint = ladder.transform.Find("TopArrivePoint");
        EnterPoint = ladder.transform.Find("EnterPoint");
    }

    // 도착 혹은 떨어져 착지 전까지 Climb State 유지
    public void Finish()
    {
        ChangeClimbState(ClimbState.Idle);
        con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Idle);
        con.StateMachine.TryChangeState(PlayerState.LocomotionState);
    }

    public void Exit()
    {

    }

}
