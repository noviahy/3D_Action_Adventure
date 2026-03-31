using UnityEngine;

public class InteractionState : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private ActionState action;
    [SerializeField] private EventManager eventManager;

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
        if (action.currentType != ActionState.ActionType.Interaction)
            return;

        if (controller.Input.InteractionPressed)
        {
            eventManager.RequestInteraction();
        }
    }
}
