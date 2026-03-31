using UnityEngine;

public class Health
{
    public float HP;

    public bool isDead { get; private set; }

    public void GetHPPosion(int value)
    {
        HP += value;
    }
}
