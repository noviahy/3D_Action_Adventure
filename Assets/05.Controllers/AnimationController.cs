using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMove(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetGrounded(bool isGrounded)
    {
        animator.SetBool("isGrounded", isGrounded);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayRoll()
    {
        animator.SetTrigger("Roll");
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void SetDead(bool isDead)
    {
        animator.SetBool("Dead", isDead);
    }
}
