using UnityEngine;

public class ActionState : MonoBehaviour, IPlayerState
{
    public ActionType currentType;
    public enum ActionType
    {
        Idle,
        Parrying,
        Attack,
        Interaction,
        Roll
    }
    
    public void TryChangeType(ActionType type)
    {
        if (currentType == ActionType.Idle || currentType == ActionType.Parrying)
            return;

        ChangeType(type);
    }

    public void ChangeType(ActionType type)
    {
        currentType = type;
    }
}
