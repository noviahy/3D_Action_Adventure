using UnityEngine;

public class KnockbackState
{
    public bool isKnockback { get; private set; } = false;
    
    private PlayerController con;

    public KnockbackState(PlayerController PlayerController)
    {
        con = PlayerController;
    }
}
