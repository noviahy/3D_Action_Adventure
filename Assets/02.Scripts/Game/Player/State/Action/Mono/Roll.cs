using System.Collections;
using UnityEngine;

// Dodge에 넣어주려고 했는데 사용하는 에니메이션 레이어가 다름
public class Roll : PlayerBehaviour
{
    private Coroutine coroutine;
    private float timer = 0f;
    private float rollDuration = 0.7f;
    private float rollSpeed = 6f;
    public void Enter() // ActionSate에서 호출
    {
        con.Input.AckRollFinish(); // 버퍼 초기화

        if (coroutine == null && !con.Input.IsLockOn) // 코루틴 조건
        {
            timer = 0f; // 타이머 초기화
            coroutine = StartCoroutine(DoRoll()); // 실행
            StartCoroutine(setLayer1());
        }
    }
    IEnumerator DoRoll()
    {
        con.Player.ChangeInvincibility(true); // 무적

        // dodgeDir 초기화
        Vector3 rollDir = con.Player.transform.forward;
        rollDir.y = 0f;

        if (con.Input.MoveInput.sqrMagnitude > 0.01f)
            rollDir = con.Input.MoveInput;

        rollDir = rollDir.normalized;

        con.Animation.PlayRoll();

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.Animation.SetLayerWeight(2, 0f);

        timer = 0f;
        // dodge 완료까지 반복
        while (true)
        {
            timer += Time.deltaTime;

            float t = timer / rollDuration; // t가 duration을 넘어가지 않음
            t = Mathf.Clamp01(t);

            // 이동 방향 설정
            Vector3 move = rollDir * rollSpeed;

            // 이동
            con.cc.Move(move * Time.deltaTime);

            // 애니메이션, timer 조건 달성시 while문 나감
            if (timer >= rollDuration)
                break;

            yield return null;
        }

        timer = 0f;
        while (timer <= 1f)
        {
            timer += Time.deltaTime * 2f;
            con.Animation.SetLayerWeight(1, 1 - timer);
            yield return null;
        }
        con.Animation.SetLayerWeight(1, 0f);

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
        {
            timer = 0f;
            while (timer <= 1f)
            {
                timer += Time.deltaTime * 5f;
                con.Animation.SetLayerWeight(2, timer);
                yield return null;
            }
            con.Animation.SetLayerWeight(2, 1f);
        }
        con.Input.AckRollFinish();
        // 상태 변경
        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);

        coroutine = null;
    }
    IEnumerator setLayer1()
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(1, t);
            yield return null;
        }
        con.Animation.SetLayerWeight(1, 1f);
    }
    public void Exit()
    {
        // 무적
        con.Player.ChangeInvincibility(false);
    }
}
