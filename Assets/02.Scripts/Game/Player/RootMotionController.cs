using UnityEngine;

public class RootMotionController : PlayerBehaviour
{
    public bool UseRootMotion { get; private set; } = false;
    public void OnAnimatorMove()
    {
        OnRootMotionMove();
    }
    public void OnRootMotionMove()
    {
        if (!UseRootMotion)
            return;

        Vector3 delta = con.Animator.deltaPosition * 0.8f;
        con.cc.Move(delta);
    }
    public void RequestRootMotion(bool value)
    {
        if (UseRootMotion == value)
            return;
        UseRootMotion = value;
        con.Animator.applyRootMotion = value;
    }
}
