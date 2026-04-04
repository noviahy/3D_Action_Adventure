using UnityEngine;

public class DeadState : IPlayerState
{
    private PlayerController con;

    public DeadState(PlayerController controller)
    {
        this.con = controller;
    }
    public void Dead()
    {
        con.Event.RequestGameOver();
        con.Animation.SetDead(true);
    }
}
