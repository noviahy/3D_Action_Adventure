using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] AttackState attackState;
    public void SwordAttack(AttackState.AttackStyle attackStyle)
    {
        switch (attackStyle)
        {
            case AttackState.AttackStyle.Light:
                {
                    break;

                }
            case AttackState.AttackStyle.Heavy:
                {

                    break;

                }
        }
    }
    private void Update()
    {
        
    }

    public void BowAttack()
    {

    }
    public void BombAttack()
    {

    }

}
