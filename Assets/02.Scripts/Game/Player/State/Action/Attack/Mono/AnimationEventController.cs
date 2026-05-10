public class AnimationEventController: PlayerBehaviour
{
    public void RequestFinishAttacking()
    {
        con.AttackState.FinishAttacking();
    }
    public void RequestUnequip()
    {
        con.Player.Unequip();
    }
}
