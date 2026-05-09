using UnityEngine;

public class AnimationEventController
{
    private PlayerController con;

    public AnimationEventController(PlayerController con)
    {
        this.con = con;
    }
    public void RequestFinishAttacking()
    {
        con.AttackState.FinishAttacking();
    }
}
