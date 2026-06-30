using UnityEngine;
public class AttackState : PlayerBehaviour
{
    public AttackStyle currentAttackStyle { get; private set; }
    public bool IgnoreBowInput { get; private set; } = false;


    private void Start()
    {
        currentAttackStyle = AttackStyle.Default;
    }
    public enum AttackStyle
    {
        Default,
        Light,
        Heavy
    }
    public void ChangeAttackStyle(AttackStyle style)
    {
        if (currentAttackStyle == style) return;
        currentAttackStyle = style;
    }
    public AttackState(PlayerController controller)
    {
        con = controller;
    }
    public void Enter()
    {
        if (con.Player.currentWeaponType == Player.WeaponType.Sword)
        {
            con.LayerController.RequestLayer1On(0.2f);
            con.LayerController.RequestLayer2Off(0.2f);
        }
    }
    private void Update()
    {
        Debug.Log(IgnoreBowInput);
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;
        if (con.BowAttack.BowAimed && con.Input.BackBuffered)
        {
            IgnoreBowInput = true;
            con.BowAttack.CancelBow();
        }

        if(IgnoreBowInput && !con.Input.BowCharging)
            IgnoreBowInput = false;

        //Debug.Log(con.Player.currentWeaponType);
        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                if (con.Input.LightAttack)
                    ChangeAttackStyle(AttackStyle.Light);
                if (con.Input.HeavyAttack)
                    ChangeAttackStyle(AttackStyle.Heavy);
                con.Attack.RequestSwordAttack();
                break;
            case Player.WeaponType.Bow:
                con.BowAttack.RequestBowAttack();
                break;
        }
    }
    public void Exit()
    {
    }
}
