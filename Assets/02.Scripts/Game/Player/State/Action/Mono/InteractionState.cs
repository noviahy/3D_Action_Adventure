using UnityEngine;

public class InteractionState : PlayerBehaviour
{
    /*
    public InteractionType interactionType {  get; private set; }
    public enum InteractionType
    {
        None,
        UseItem,
        Select,
        Dialogue
    }
    public void ChangeInteractionType(InteractionType type)
    {
        interactionType = type;
    }
    */

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
