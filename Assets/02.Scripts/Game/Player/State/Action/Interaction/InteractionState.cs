using UnityEngine;
using static ActionState;

public class InteractionState
{
    PlayerController con;
    public InteractionType CurrentType { get; private set; }
    private InteractionType preType;
    public enum InteractionType
    {
        Idle,
        UseItem, // Layer4
        Box, // Layer1
        Mantle, // Layer1
        Climb // Layer1
    }
    public InteractionState(PlayerController controller)
    {
        con = controller;
        CurrentType = InteractionType.Idle;
    }
    // 상태 바꿔주는 코드 짜야함
    public void TryChangeInteractionType(InteractionType type)
    {
        if (CurrentType == type)
            return;

        if (con.InteractionIdle.IdleBlending)
        {
            if(type == InteractionType.UseItem)
                    con.InteractionIdle.RequestStopLayer4();
            else
                con.InteractionIdle.RequestStopLayer1();
        }

        if (CurrentType != InteractionType.Idle && type != InteractionType.Idle)
            return;

        ChangeType(type);
    }
    private void ChangeType(InteractionType type)
    {
        exitInteraction(CurrentType);

        preType = CurrentType;
        CurrentType = type;

        enterInteraction(CurrentType);
    }

    private void enterInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Idle:
                con.InteractionIdle.Enter(preType);
                break;
            case InteractionType.UseItem:
                break;
            case InteractionType.Box:
                break;
            case InteractionType.Mantle:
                break;
            case InteractionType.Climb:
                con.Dodge.Enter();
                break;
        }
    }
    // 솔찍히 Exit에 따로 뭐가 없는데 걍 여기서 Idle 실행해도 될 것 같기도
    // 라는 생각을 했는데 서순이 안 맞음
    // 절때 exit에 Idle로 바꾸는 코드를 넣어선 안됨
    // 그럼 eixt은 왜있는거지
    private void exitInteraction(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.UseItem:
                break;
            case InteractionType.Box:
                break;
            case InteractionType.Mantle:
                break;
            case InteractionType.Climb:
                con.Climb.Exit();
                break;
        }
    }
}
