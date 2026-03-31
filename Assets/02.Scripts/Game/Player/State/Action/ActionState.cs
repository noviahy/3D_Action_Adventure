using NUnit.Framework;
using UnityEngine;

public class ActionState : IPlayerState
{
    public ActionType currentType;
    public enum ActionType
    {
        Parrying,
        Attack,
        Interaction,
        Roll
    }
    public void ChangeType(ActionType type)
    {
        currentType = type;
    }
}
