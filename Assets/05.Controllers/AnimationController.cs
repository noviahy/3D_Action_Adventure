using UnityEngine;

public class AnimationController
{
    private PlayerController con;
    private float idleTimer;
    private float idleDelay = 0.05f;

    public AnimationController(PlayerController con)
    {
        this.con = con;
    }
    public void SetMove(float speed) //
    {
        if (speed <= 0.1f)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleDelay)
                con.Animator.SetFloat("Speed", 0f);
        }
        else
        {
            idleTimer = 0;
            con.Animator.SetFloat("Speed", speed);
        }
    }
    public void SetMoveX(float x) // 앞뒤
    {
        float damp = (x < 0.1f) ? 0.15f : 0.05f;
        con.Animator.SetFloat("MoveX", x, damp, Time.deltaTime);
    }
    public void SetMoveY(float y) // 양옆
    {
        float damp = (y < 0.1f) ? 0.08f : 0.05f;
        con.Animator.SetFloat("MoveY", y, damp, Time.deltaTime);
    }
    public void SetMouseX(float x)
    {
        con.Animator.SetFloat("MouseX", x);
    }
    public void SetGrounded(bool isGrounded) // 왜 안되는지 찾아야함
    {
        con.Animator.SetBool("isGrounded", isGrounded);
    }
    public void SetLockOn(bool lockOn) //
    {
        con.Animator.SetBool("LockOn", lockOn);
    }
    public void SetParry(bool parry) // 
    {
        con.Animator.SetBool("Parry", parry);
    }
    public void SetDead(bool isDead) //
    {
        con.Animator.SetBool("Dead", isDead);
    }
    public void PlayLoadBow()
    {
        con.Animator.CrossFadeInFixedTime("Load", 0.05f);
    }
    public void PlayDodge(string dir) //
    {
        con.Animator.CrossFade($"{dir}JumpMove", 0.05f);
    }
    public void PlayRoll()
    {
        con.Animator.CrossFade("Roll", 0.05f);
    }
    public void PlayUseingItem(Player.ItemType type)
    {
        switch (type)
        {
            case Player.ItemType.HPPosion:
                con.Animator.CrossFadeInFixedTime("Drink", 0.05f);
                break;
            case Player.ItemType.Bomb:
                con.Animator.CrossFadeInFixedTime("Throw", 0.05f);
                break;
        }

    }
    public void SetWeaponType(int type) //
    {
        con.Animator.SetInteger("WeaponType", type);
    }
    public void PlayUpperBody(string type)
    {
        con.Animator.CrossFadeInFixedTime($"Upper Body.{type}", 0.25f);
    }
    public void PlayLowerBody(string type)
    {
        con.Animator.CrossFadeInFixedTime($"Lower Body.{type}", 0.25f);
    }
    public void PlayLightAttack(int combo)
    {
        con.Animator.CrossFade($"LightAttack{combo}", 0.05f);
    }
    public void PlayHeavyAttack()
    {
        con.Animator.CrossFadeInFixedTime("HeavyAttack", 0.05f);
    }
    public void PlayJump()
    {
        con.Animator.CrossFadeInFixedTime("Jump", 0.05f);
    }
    public void PlayLand()
    {
        con.Animator.CrossFadeInFixedTime("LandSoft", 0.1f);
    }
    public void PlayHit() //
    {
        con.Animator.SetTrigger("Hit");
    }
    public void PlayPull()
    {

    }
    public void PlayPush()
    {

    }
    // 터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다터진다
    public void SetLayerWeight(int index, float weight)
    {
        con.Animator.SetLayerWeight(index, weight);
    }
    public void PlayMantle()
    {
        con.Animator.CrossFadeInFixedTime("Mantle", 0.05f);
    }
    public void PlayClimbing()
    {
        con.Animator.CrossFadeInFixedTime("ClimbIdle", 0.05f);
    }
    public void PlayFalling()
    {
        con.Animator.CrossFadeInFixedTime("Falling", 0.05f);
    }
    public void PlayArrive()
    {
        con.Animator.CrossFadeInFixedTime("Arrive", 0.1f);
    }
    public void PlayHang()
    {
        con.Animator.CrossFadeInFixedTime("Hanging", 0.05f);
    }
    public void PlayJumpHang()
    {
        con.Animator.CrossFadeInFixedTime("JumpHang", 0.05f);
    }
    public void PlayHangRight()
    {
        con.Animator.CrossFadeInFixedTime("Right", 0.05f);
    }
    public void PlayHangLeft()
    {
        con.Animator.CrossFadeInFixedTime("Left", 0.05f);
    }
}
