public class ActionState : IPlayerState
{
    private PlayerController con;
    public ActionType currentType {  get; private set; }
    public ActionType preType { get; private set; }
    public enum ActionType
    {
        Idle,
        Parrying,
        Attack,
        Interaction,
        Dodge
    }
    
    public ActionState(PlayerController controller)
    {
        con = controller;
        currentType = ActionType.Idle;
    }

    public void TryChangeType(ActionType type)
    {
        if(currentType == type) return;

        if (con.Idle.IdleBlending) 
        {
            switch (type)
            {
                case ActionType.Attack:
                    con.Idle.RequestStopAllCoroutine();
                    break;
                case ActionType.Dodge:
                    con.Idle.RequestStopDodgeLayer();
                    con.Idle.RequestStopLayer1();
                    break;
                case ActionType.Parrying:
                    con.Idle.RequestStopDodgeLayer();
                    con.Idle.RequestStopLayer1();
                    break;
            }
        }
        if (type != ActionType.Idle)
        {
            if (currentType == ActionType.Parrying)
                return;
            if (currentType == ActionType.Attack)
                return;
            if (currentType == ActionType.Dodge)
                return;
            if(currentType == ActionType.Interaction)
                return;
        }

        ChangeType(type);
    }

    private void ChangeType(ActionType type)
    {
        exitAction(currentType);

        preType = currentType;
        currentType = type;

        enterAction(currentType);
    }
    private void enterAction(ActionType type)
    {
        switch (type)
        {
            case ActionType.Idle:
                con.Idle.Enter(preType);
                break;
            case ActionType.Parrying:
                con.Parrying.Enter();
                break;
            case ActionType.Attack:
                con.AttackState.Enter();
                break;
            case ActionType.Interaction:
                break;
            case ActionType.Dodge:
                con.Dodge.Enter();
                break;
        }
    }

    private void exitAction(ActionType type)
    {
        switch (type)
        {
            case ActionType.Parrying:
                con.Parrying.Exit();
                break;
            case ActionType.Attack:
                con.AttackState.Exit();
                break;
            case ActionType.Interaction:
                break;
            case ActionType.Dodge:
                con.Dodge.Exit();
                break;
        }
    }
}
