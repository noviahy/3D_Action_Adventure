using System.Collections;
using Unity.VisualScripting;
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

    public ClimbState currentState { get; private set; }
    public enum ClimbState
    {
        Idle,
        Enter,
        Climbing,
        Falling,
        Arrive,
        Exit // 바닥까지 천천히 내려간 상태?
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
            case ClimbState.Arrive:
                arriveCoroutine = StartCoroutine(ArriveCoroutine());
                break;
            case ClimbState.Exit:
                break;
        }
    }
    public void Enter()
    {
        ChangeClimbState (ClimbState.Enter);
        if (enterCoroutine == null)
            enterCoroutine = StartCoroutine(EnterClimbing());

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.Player.ChangeWeaponType(Player.WeaponType.Default);
    }
    IEnumerator EnterClimbing()
    {
        // 무기 끄는 코루틴 완료까지 기다림
        if (con.Player.currentItemType != 0)
            yield return new WaitForSeconds(0.5f);

        con.Movement.EnterClimb();

        con.Animation.PlayClimbing();
        float time = 0;
        while (time <= 1)
        {
            time += Time.deltaTime * 3;
            con.Animation.SetLayerWeight(1, time);
            yield return null;
        }
        con.Animation.SetLayerWeight(1, 1);
        enterCoroutine = null;

        // Enter 종료 후 Climb으로 상태 변경
        ChangeClimbState(ClimbState.Climbing);
    }

    private void Update()
    {
        Debug.Log(currentState);
        if (con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
            return;

        if (currentState != ClimbState.Climbing)
            return;

        if (!con.StateMachine.isLadder)
        {
            ChangeClimbState(ClimbState.Arrive);
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
        while (!con.GroundCheck.IsGrounded)
            yield return null;

        // 랜딩 애니메이션 대기
        yield return new WaitForSeconds(1f);
        fallingCoroutine = null;
        Finish();
    }

    IEnumerator ArriveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        arriveCoroutine = null;
        Finish();
    }
    public void SetLadder(Transform ladder)
    {
        currentLadder = ladder;
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
