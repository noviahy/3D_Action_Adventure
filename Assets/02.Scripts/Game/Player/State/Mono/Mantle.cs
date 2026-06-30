using UnityEngine;
using System.Collections;
using static LocomotionState;

public class Mantle : PlayerBehaviour
{
    public void Enter()
    {
        StartCoroutine(EnterMantle());
    }
    IEnumerator EnterMantle()
    {
        con.LayerController.RequestLayer1On(0.2f);
        con.LayerController.RequestLayer2Off(0.2f);

        // RootMotion 사용 코드
        con.RootMotionController.RequestRootMotion(true);
        con.Animation.PlayMantle();

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("Mantle"));

        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.7f);

        con.RootMotionController.RequestRootMotion(false);

        Vector3 arriveTargetPos = con.StateMachine.Mantle.point + Vector3.up * 0.8f - con.StateMachine.Mantle.normal * 0.3f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = arriveTargetPos - con.cc.center + Vector3.up * (con.Locomotion.originHeight * 0.5f);
        Vector3 prevPos = startPos;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 2f;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, time);
            con.cc.Move(nextPos - prevPos);

            prevPos = nextPos;
            yield return null;
        }

        con.cc.radius = con.Locomotion.originRadius;
        con.cc.height = con.Locomotion.originHeight;

        while (!con.cc.isGrounded)
        {
            con.Movement.Airborne();
            yield return null;
        }

        con.LayerController.RequestLayer1Off(0.2f);
        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.LayerController.RequestLayer2On(0.2f);

        con.Locomotion.ChangeState(LocomotionSubState.Idle);
    }
}
