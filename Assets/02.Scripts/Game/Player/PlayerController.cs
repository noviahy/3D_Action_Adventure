using UnityEngine;

public class PlayerController
{
    public InputManager Input { get; }
    public Player Player { get; }
    public PlayerStateMachine StateMachine { get; }
    public PlayerMovement Movement { get; }
    public PlayerCombat Combat { get; }
    public Health Health { get; }
    public KnockbackState KnockbackState { get; }
    public ActionState Action { get; }
    public GroundCheck GroundCheck { get; }
}
