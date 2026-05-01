using System.Collections;
using UnityEngine;

public class Dodge : PlayerBehaviour
{
    private Coroutine coroutine;
    private float dodgeSpeed = 5f;
    private float timer = 0f;
    private float duration = 0.5f;
    public void Enter()
    {
        if (coroutine == null)
        {
            timer = 0f;
            con.Input.AckDodgeFinish();
            coroutine = StartCoroutine(DoDodge());

        }
    }
    IEnumerator DoDodge()
    {
        con.Player.ChangeInvincible(true);

        con.Animation.SetLayerWeight(1, 1);
        con.Animation.SetLayerWeight(0, 0);
        con.Animation.PlayDodge();

        while (true)
        {
            AnimatorStateInfo state = con.Animator.GetCurrentAnimatorStateInfo(1);

            timer += Time.deltaTime;
            float t = timer / duration;

            float speedMultiplier = Mathf.Sin(t * Mathf.PI * 0.8f);

            Vector3 inputDir = con.Input.MoveInput;
            inputDir.y = 0;
            inputDir = inputDir.normalized;

            Vector3 dodgeDir = Vector3.zero;

            float forwardDot = Vector3.Dot(con.Cam.camForward, inputDir);
            float rightDot = Vector3.Dot(con.Cam.camRight, inputDir);

            if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
            {
                dodgeDir = forwardDot> 0 ? con.Cam.camForward : -con.Cam.camForward;
            }
            else
            {
                dodgeDir = rightDot > 0 ? con.Cam.camRight : -con.Cam.camRight;
            }
            if (inputDir.sqrMagnitude < 0.01f)
            {
                dodgeDir = con.Cam.camForward; // 기본 전방 회피
            }

            Vector3 move = dodgeDir * dodgeSpeed * speedMultiplier;

            con.cc.Move(move * Time.deltaTime);

            if (t >= 1f || state.normalizedTime >= 0.95f)
                break;

            yield return null;
        }


        con.Animation.SetLayerWeight(1, 0);
        con.Animation.SetLayerWeight(0, 1);

        con.ActionState.ChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);

        coroutine = null;
    }
    public void Exit()
    {
        con.Player.ChangeInvincible(false);
    }
}
