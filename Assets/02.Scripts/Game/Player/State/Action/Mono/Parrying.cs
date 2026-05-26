using System.Collections;
using UnityEngine;

public class Parrying : PlayerBehaviour
{
    private float InvincibleTime = 0.3f;
    public void Enter()
    {
        StopCoroutine(InvincibleTimer());
        con.Animation.SetParry(true);
        con.Player.ChangeInvincibility(true);
        con.Player.NormalGuard(true);
        StartCoroutine(InvincibleTimer());
    }
    private void Update()
    {
        if (con.ActionState.currentType != ActionState.ActionType.Parrying)
            return;

        if(!con.Input.ParryingPressed)
            con.ActionState.TryChangeType(ActionState.ActionType.Idle);

        if (con.ActionState.currentType != ActionState.ActionType.Parrying)
            return;

        // if(con.ActionState.currentType == ActionState.ActionType.Parrying) -> 나중에 코드 추가
    }
    public void Exit()
    {
        con.Player.ChangeInvincibility(false);
        con.Player.NormalGuard(false);
        con.Animation.SetParry(false);
    }

    IEnumerator InvincibleTimer()
    {
        yield return new WaitForSeconds(InvincibleTime);
        con.Player.ChangeInvincibility(false);
    }
}
