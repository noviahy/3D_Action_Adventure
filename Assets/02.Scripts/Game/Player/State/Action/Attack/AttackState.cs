using UnityEngine;

public class AttackState : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private ActionState action;
    [SerializeField] private Attack attack;
    [SerializeField] private Parrying parrying;
    public AttackStyle currentAttackStyle { get; private set; }
    public enum AttackStyle 
    {
        Light,
        Heavy
    }
    public void ChangeAttackStyle(AttackStyle style)
    {
        if (currentAttackStyle == style) return;
        currentAttackStyle = style;
    }
    private void Update()
    {
        if (action.currentType != ActionState.ActionType.Attack)
            return;
        if(controller.Input.LightAttack)
            ChangeAttackStyle(AttackStyle.Light);
        if(controller.Input.HeavyAttack)
            ChangeAttackStyle(AttackStyle.Heavy);
    }
    private void FixedUpdate()
    {
        if (action.currentType != ActionState.ActionType.Attack)
            return;

        switch (controller.Player.currentAttackType)
        {
            case Player.AttackType.Sword:
                
                attack.SwordAttack(currentAttackStyle);
                break;
            case Player.AttackType.Bow:
                attack.BowAttack();
                break;
            case Player.AttackType.Bomb:
                attack.BombAttack();
                break;
        }
    }
}
