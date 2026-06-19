using UnityEngine;
public class ActionState : IPlayerState
{
    private PlayerController con;
    public bool fromBow = false;
    public ActionType currentType { get; private set; }
    public ActionType preType { get; private set; }
    public enum ActionType
    {
        Idle,
        Parrying, // Layer 2: 나머지 자유
        Attack, // Sword-Layer 1(0,1,2고려) Bow-Layer 2 사용(고려하지 않음-우선순위)
        Activity,
        Dodge, // Layer 1 사용 : 나머지 자유
        Roll // Layer 1 사용 : 나머지 다 막아야 함
    }

    public ActionState(PlayerController controller)
    {
        con = controller;
        currentType = ActionType.Idle;
    }

    public void TryChangeType(ActionType type)
    {
        if (currentType == type)
            return;

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
                case ActionType.Roll:
                    if (con.BowAttack.Standby)
                    {
                        fromBow = true;
                        con.BowAttack.RollOnRelease();
                    }
                    break;
            }
        }
        // 이거 왜 이렇게 하나하나 다 써놨는지 모르겠음
        // 그래도 나중에 혹시 쓸일이 있을까 일단 남겨둠
        // 이거 지웠더니 안 돌아감 진짜 왜 그러는거야
        if (type != ActionType.Idle)
        {
            if (currentType == ActionType.Parrying)
                return;
            if (currentType == ActionType.Attack)
                return;
            if (currentType == ActionType.Dodge)
                return;
            if (currentType == ActionType.Activity)
                return;
            if (currentType == ActionType.Roll)
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
            case ActionType.Roll:
                con.Roll.Enter();
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
            case ActionType.Roll:
                con.Roll.Exit();
                break;
        }
    }
    public void ResetFromBow()
    {
        fromBow = false;
    }
}
