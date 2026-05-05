using System.Collections;
using UnityEngine;

public class Dodge : PlayerBehaviour
{
    private Coroutine coroutine;
    private float dodgeSpeed = 6f;
    private float timer = 0f;
    private float dodgeDuration = 0.7f;
    public void Enter() // ActionSate에서 호출
    {
        if (coroutine == null) // 코루틴 조건
        {
            timer = 0f; // 타이머 초기화
            con.Input.AckDodgeFinish(); // 버퍼 초기화
            coroutine = StartCoroutine(DoDodge()); // 실행
        }
    }//
    IEnumerator DoDodge()
    {
        con.Player.ChangeInvincible(true); // 무적

        // 에니메이션 weight 변경
        // 여기에 문제가 있는것 같음 회피 에니메이션이 씹힘
        //
        con.Animation.SetLayerWeight(1, 1); 
        con.Animation.SetLayerWeight(0, 0);
        con.Animation.PlayDodge();

        // 방향 정규화
        Vector3 inputDir = con.Input.MoveInput;
        inputDir.y = 0;
        inputDir = inputDir.normalized;

        // dodgeDir 초기화
        Vector3 dodgeDir = Vector3.zero;

        // inputDir 분리
        float forwardDot = Vector3.Dot(con.Cam.camForward, inputDir);
        float rightDot = Vector3.Dot(con.Cam.camRight, inputDir);

        // 왼쪽 오른쪽 앞 뒤 고정
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            dodgeDir = forwardDot > 0 ? con.Cam.camForward : -con.Cam.camForward;
        }
        else
        {
            dodgeDir = rightDot > 0 ? con.Cam.camRight : -con.Cam.camRight;
        }
        // 기본 전방 회피
        if (inputDir.sqrMagnitude < 0.01f)
        {
            dodgeDir = con.Cam.camForward;
        }


        // dodge 완료까지 반복
        while (true)
        {
            timer += Time.deltaTime;

            float t = timer / dodgeDuration; // t가 duration을 넘어가지 않음
            t = Mathf.Clamp01(t);

            // sin형으로 속도 조절
            float speedMultiplier = Mathf.Sin(t * Mathf.PI);

            // 이동 방향 설정
            Vector3 move = dodgeDir * dodgeSpeed * speedMultiplier;

            // 이동
            con.cc.Move(move * Time.deltaTime);

            // 애니메이션, timer 조건 달성시 while문 나감
            if (timer >= dodgeDuration)
                break;

            yield return null;
        }

        // Layer weight 다시 바꿔줌
        float w = 0;
        while (w < 1f)
        {
            w += Time.deltaTime * 3f;
            con.Animation.SetLayerWeight(0, w);
            con.Animation.SetLayerWeight(1, 1 - w);
            yield return null;
        }

        // 상태 변경
        con.ActionState.ChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);

        coroutine = null;
    }
    public void Exit()
    {
        // idle로 상태 변경시 자동으로 호출
        con.Player.ChangeInvincible(false);
    }
}
