using System.Collections;
using UnityEngine;
using static PlayerStateMachine;

public class Climb : PlayerBehaviour
{
    // 벽에 어떻게 붙을건지 생각좀 해봐야 할듯
    // 움직임은 PlayerMovement 코드 에서 -> 나머지 Interaction이 중간에 Locomotion이 필요하기 때문에
    // 여기에 움직임 코드를 짜줄수도 있지만 통일성을 위해 PlayerMovement로 통일
    private Coroutine enterCoroutine;
    private Coroutine fallingCoroutine;
    private Coroutine arriveCoroutine;
    public bool isClimbing { get; private set; } = false;

    public void Enter()
    {
        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.Player.ChangeWeaponType(Player.WeaponType.Default);
        if (enterCoroutine == null)
            enterCoroutine = StartCoroutine(EnterClimbing());
    }
    private void Update()
    {
        if (con.InteractionState.CurrentType != InteractionState.InteractionType.Climb)
            return;

        if (!con.StateMachine.isLadder && arriveCoroutine == null && fallingCoroutine == null)
            arriveCoroutine = StartCoroutine(ArriveCoroutine());
            
        if (con.Input.BackBuffered && arriveCoroutine == null && fallingCoroutine == null)
            fallingCoroutine = StartCoroutine(FallingCoroutine());
    }
    // 도착 혹은 떨어져 착지 전까지 Climb State 유지
    public void Finish()
    {
        con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Idle);
        con.StateMachine.TryChangeState(PlayerState.LocomotionState);
    }
    public void Exit()
    {

    }
    IEnumerator FallingCoroutine()
    {
        con.Animation.PlayFalling();
        while (!con.GroundCheck.IsGrounded) 
            yield return null;

        // 랜딩 애니메이션 대기
        yield return new WaitForSeconds(1f);
        Finish();
        fallingCoroutine = null;
    }

    IEnumerator EnterClimbing()
    {
        //여기에 무기 0으로 바꾸는 코루틴 대기 시간을 넣어줘야 할듯
        float time = 0;
        while (time <= 1)
        {
            time += Time.deltaTime * 3;
            con.Animation.SetLayerWeight(1, time);
            yield return null;
        }
        con.Animation.SetLayerWeight(1, 1);
        isClimbing = true;
        enterCoroutine = null;
    }
    IEnumerator ArriveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Finish();
        arriveCoroutine = null;
    }
}
