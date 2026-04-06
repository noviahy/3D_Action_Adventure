using UnityEngine;

public class AnimationController
{
    private PlayerController con;

    public AnimationController(PlayerController con)
    {
        this.con = con;
    }
    public void SetMove(float speed) //
    {
        con.Animator.SetFloat("Speed", speed);
    }
    public void SetMoveX(float x)
    {
        con.Animator.SetFloat("MoveX", x);
    }
    public void SetMoveY(float y)
    {
        con.Animator.SetFloat("MoveY", y);
    }
    public void SetGrounded(bool isGrounded) // 왜 안되는지 찾아야함
    {
        // con.Animator.SetBool("isGrounded", isGrounded);
    }
    public void SetLockOn(bool lockOn) //
    {
        // con.Animator.SetBool("LockOn", lockOn);
    }
    public void SetParry(bool parry) // 
    {
        con.Animator.SetBool("Parry", parry);
    }
    public void SetDead(bool isDead) //
    {
        con.Animator.SetBool("Dead", isDead);
    }
    public void SetBowAim(bool bowAim)
    {
        con.Animator.SetBool("BowAim", true);
    }
    public void PlayJump() //
    {
        con.Animator.SetTrigger("Jump");
    }
    public void PlayAttack() 
    {
        con.Animator.SetTrigger("Attack");
    }
    public void PlayDodge() //
    {
        con.Animator.SetTrigger("Dodge");
    }
    public void SetWeaponType(int type) //
    {
        con.Animator.SetInteger("WeaponType", type);
    }
    public void SetAttackType(int type) //
    {
        con.Animator.SetInteger("AttackType", type);
    }
    public void PlayHit() //
    {
        con.Animator.SetTrigger("Hit");
    }
}
