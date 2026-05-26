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
        Activity,
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

        if (con.ActionIdle.IdleBlending) 
        {
            switch (type)
            {
                case ActionType.Attack:
                    con.ActionIdle.RequestStopAllCoroutine();
                    break;
                case ActionType.Dodge:
                    con.ActionIdle.RequestStopDodgeLayer();
                    con.ActionIdle.RequestStopLayer1();
                    break;
                case ActionType.Parrying:
                    con.ActionIdle.RequestStopDodgeLayer();
                    con.ActionIdle.RequestStopLayer1();
                    break;
                case ActionType.Activity:
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
            if(currentType == ActionType.Activity)
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
                con.ActionIdle.Enter(preType);
                break;
            case ActionType.Parrying:
                con.Parrying.Enter();
                break;
            case ActionType.Attack:
                con.AttackState.Enter();
                break;
            case ActionType.Activity:
                // 따로 코드 생성해야함
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
            case ActionType.Activity:
                break;
            case ActionType.Dodge:
                con.Dodge.Exit();
                break;
        }
    }
}
