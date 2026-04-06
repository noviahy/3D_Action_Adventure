using System.Collections;
using UnityEngine;

public class Dodge : PlayerBehaviour
{
    private Coroutine coroutine;
    public void Enter()
    {
        if (coroutine != null)
        {
            con.Animation.PlayDodge();
            con.ActionState.ChangeInvincible(true);
            coroutine = StartCoroutine(DoDodge());
        }
    }
    IEnumerator DoDodge()
    {
        yield return new WaitForSeconds(1.2f);
        coroutine = null;
        con.ActionState.ChangeType(ActionState.ActionType.Idle);
    }
    public void Exit()
    {
        con.ActionState.ChangeInvincible(false);
    }
}
