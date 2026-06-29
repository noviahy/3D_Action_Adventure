using System.Collections;
using UnityEngine;

// Dodge에 넣어주려고 했는데 사용하는 에니메이션 레이어가 다름
public class Roll : PlayerBehaviour
{
    private Coroutine coroutine;
    private float timer = 0f;
    private float rollDuration = 0.7f;
    private float rollSpeed = 3.5f;
    public bool isRollCoolTime = false;
    public void Enter() // ActionSate에서 호출
    {
        con.Input.AckRollFinish(); // 버퍼 초기화

        if (coroutine == null && !con.Input.IsLockOn) // 코루틴 조건
        {
            timer = 0f; // 타이머 초기화
            coroutine = StartCoroutine(DoRoll()); // 실행
            con.LayerController.RequestLayer1On(0.3f);
        }
    }
    IEnumerator DoRoll()
    {
        isRollCoolTime = true;
        con.Player.ChangeInvincibility(true); // 무적

        // dodgeDir 초기화
        Vector3 rollDir = con.Player.transform.forward;
        rollDir.y = 0f;

        if (con.Input.MoveInput.sqrMagnitude > 0.01f)
            rollDir = con.Input.MoveInput;

        if (con.ActionState.fromBow)
        {
            Quaternion targetRot = Quaternion.LookRotation(con.Input.MoveInput);
            con.Player.transform.rotation = targetRot;
        }

        rollDir = rollDir.normalized;
        con.ActionState.ResetFromBow();

        con.Animation.PlayRoll();

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.LayerController.RequestLayer2Off(0.2f);

        timer = 0f;
        // dodge 완료까지 반복
        // 이동 코드
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

        // 무기 드는 포즈 없음
        if (con.Player.currentWeaponType == Player.WeaponType.Default)
        {
            con.LayerController.RequestLayer1Off(0.2f);
        }
        // 무기 드는 포즈 있음
        else
        {
            con.LayerController.RequestLayer1Off(0.2f);
            con.LayerController.RequestLayer2On(0.2f);
        }

        con.Input.AckRollFinish();
        coroutine = null;
        StartCoroutine(startTimer());

        // 상태 변경
        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);
        con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Idle);
    }
    // 공중 상태에서만 호출
    public void RequestStopRoll()
    {
        // 일단 코루틴을 멈춤
        StopCoroutine(coroutine);
        coroutine = null;

        // 모든 레이어를 꺼줌
        con.LayerController.RequestLayer1Off(0.2f);
        con.LayerController.RequestLayer2Off(0.2f);

        // 변수 초기화
        isRollCoolTime = false;

        // 상태 변경
        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);
        con.Locomotion.ChangeState(LocomotionState.LocomotionSubState.Airborne);
    }
    public void Exit()
    {
        // 무적
        con.Player.ChangeInvincibility(false);
    }
    IEnumerator startTimer()
    {
        yield return new WaitForSeconds(0.3f);
        isRollCoolTime = false;
    }
}
