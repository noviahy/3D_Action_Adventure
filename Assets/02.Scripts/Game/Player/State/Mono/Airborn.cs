using UnityEngine;
using static LocomotionState;

public class Airborn : PlayerBehaviour
{
    public float startHeight {  get; private set; }
    public bool TryEnterAirborne()
    {
        if (con.cc.isGrounded)
            return false;

        if (con.InteractionState.CurrentType ==
            InteractionState.InteractionType.Climb)
            return false;

        con.Locomotion.ChangeState(LocomotionSubState.Airborne);
        return true;
    }

    public void Enter(LocomotionState.LocomotionSubState from)
    {
        if (con.InteractionState.CurrentType ==
            InteractionState.InteractionType.Climb)
            return;

        con.Movement.ResetYVelocity();

        startHeight = con.EdgeCheck.EdgeValue;

        con.Movement.SetStartY();

        if (from == LocomotionSubState.Run && startHeight >= 2.5)
        {
            con.Movement.StartJump();
            Debug.Log(startHeight);
        }
        if (from == LocomotionSubState.Walk || (from == LocomotionSubState.Run && startHeight <= 2.5))
        {
            if (startHeight > 3f)
            {
                con.LayerController.RequestLayer1On(0.3f);

                con.Animation.PlayFalling();
            }
        }
        if (from == LocomotionSubState.Hang)
            con.Animation.PlayFalling();
    }
    public void Exit()
    {
    }
}
