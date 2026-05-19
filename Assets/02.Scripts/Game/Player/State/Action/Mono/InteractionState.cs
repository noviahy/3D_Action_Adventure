using UnityEngine;

public class InteractionState : PlayerBehaviour
{
    
    public InteractionType CurrentInteractionType {  get; private set; }
    public enum InteractionType
    {
        Idle,
        UseItem,
        Select,
        Dialogue,
        Traversal
    }
    public void ChangeInteractionType(InteractionType type)
    {
        if (CurrentInteractionType != InteractionType.Idle)
            return;
        CurrentInteractionType = type;
    }

    private void Update()
    {
        if (con.ActionState.currentType != ActionState.ActionType.Interaction)
            return;

        if (con.Input.InteractionPressed)
        {
            con.Event.RequestInteraction();
        }
    }
}
