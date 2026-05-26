using UnityEngine;

public class InteractionState
{
    PlayerController con;
    public InteractionType CurrentInteractionType { get; private set; }
    public enum InteractionType
    {
        Idle,
        UseItem,
        Box,
        Climb
    }
    public InteractionState(PlayerController controller)
    {
        con = controller;
        CurrentInteractionType = InteractionType.Idle;
    }
    // 상태 바꿔주는 코드 짜야함
    public void TryChangeInteractionType(InteractionType type)
    {
        if (CurrentInteractionType != InteractionType.Idle && type != InteractionType.Idle)
            return;

        CurrentInteractionType = type;
        switch (CurrentInteractionType)
        {
            case InteractionType.Idle:
                
                break;
            case InteractionType.UseItem:
                break;
            case InteractionType.Box:
                break;
            case InteractionType.Climb:
                con.Climb.Enter();
                break;
        }
    }    
}
