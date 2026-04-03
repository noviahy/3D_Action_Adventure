using UnityEngine;

public class PlayerMovement
{
    [SerializeField] private CharacterController cc;
    [SerializeField] private LocomotionState state;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForwardPower;
    [SerializeField] private float jumpUpPower;

    private Vector3 jumpDir;
    private float yVelocity;
    private bool isJumping;

    public bool JustLanded { get; private set; } = false;
    public void ChangeJustLanded()
    {
        JustLanded = false;
    }
    public void Move(Vector3 inputDir, bool isRun)
    {
        Vector3 horizontal;

        if (isJumping)
        {
            // 점프 중이면 고정된 방향으로 이동
            horizontal = jumpDir * jumpForwardPower;
        }
        else
        {
            float speed = isRun ? runSpeed : walkSpeed;
            horizontal = inputDir.normalized * speed;
            controller.Animation.SetMove(speed);
        }

        // 중력
        if (cc.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
            isJumping = false;
            JustLanded = true;
        }

        yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = new Vector3(horizontal.x, yVelocity, horizontal.z);
        cc.Move(move * Time.deltaTime);
    }
    public void Jump(Vector3 dir)
    {
        if (cc.isGrounded)
        {
            isJumping = true;

            if (dir == Vector3.zero)
                jumpDir = state.transform.forward;
            else
                jumpDir = dir.normalized;

            yVelocity = jumpUpPower;
        }
    }
}