using UnityEngine;

public class BoxInteractionState : PlayerBehaviour
{
    private Vector3 targetPos;
    private RaycastHit box;
    private Box currentBox;
    private float playerDistance;

    public BoxState currentState;
    public enum BoxState
    {
        Idle,
        Pull,
        Push
    }
    public void ChangeState(BoxState state)
    {
        if (currentState == state)
            return;
        if (currentState != BoxState.Idle || state != BoxState.Idle)
            return;

        currentState = state;
    }
    private void Start()
    {
        currentState = BoxState.Idle;
    }
    public void Enter()
    {
        box = con.StateMachine.Box;

        targetPos = box.collider.bounds.center + box.normal * (box.collider.bounds.extents.z + playerDistance);
    }
    private void Update()
    {
        if (con.InteractionState.CurrentType != InteractionState.InteractionType.Box)
            return;

        if (con.Input.forward > 0.3f)
            ChangeState(BoxState.Push);
        if(con.Input.forward < -0.3f)
            ChangeState(BoxState.Pull);

        switch (currentState)
        {
            case BoxState.Idle:
                break;
            case BoxState.Pull:
                break;
            case BoxState.Push:
                break;
        }

    }
    public void Exit()
    {

    }
}
