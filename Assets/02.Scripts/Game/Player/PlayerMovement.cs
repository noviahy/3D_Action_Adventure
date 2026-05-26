using UnityEngine;

public class PlayerMovement
{
    private PlayerController con;

    private float walkSpeed = 3f;
    private float runSpeed = 7f;
    private float lockOnSpeed = 1.5f;
    private float jumpForwardPower = 2f;
    private float jumpUpPower = 2f;
    private float rotSpeed = 10f;

    private Vector3 jumpDir;
    private float yVelocity;
    private bool isJumping;

    public bool JustLanded { get; private set; } = false;

    public PlayerMovement(PlayerController controller)
    {
        con = controller;
    }
    // 보통 여기에 진짜 천천히 움직이는걸 하나 더 만들어야 하지만 그건 다음 게임에 추가
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

            // 록온 시 speed를 덮어씌움
            if (con.Input.IsLockOn || con.BowAttack.BowAimed)
                speed = lockOnSpeed;

            horizontal = inputDir.normalized * speed;
            con.Animation.SetMove(speed);

            // Player 회전 코드
            if (!con.Input.IsLockOn && !con.BowAttack.BowAimed && !con.BowAttack.Standby)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDir);

                con.Player.transform.rotation = Quaternion.Lerp(con.Player.transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            }
        }

        // 중력
        if (con.cc.isGrounded && yVelocity < 0 && isJumping)
        {
            yVelocity = -2f;
            isJumping = false;
            JustLanded = true;
        }

        yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = new Vector3(horizontal.x, yVelocity, horizontal.z);
        con.cc.Move(move * Time.deltaTime);
    }
    public void Jump(Vector3 dir)
    {
        if (con.cc.isGrounded)
        {
            isJumping = true;
            // Jump 애니메이션 삽입
            if (dir == Vector3.zero)
                jumpDir = con.Locomotion.transform.forward;
            else
                jumpDir = dir.normalized;

            yVelocity = jumpUpPower;
        }
    }
    public void ChangeJustLanded()
    {
        JustLanded = false;
    }

    public void Climb(float dir, bool isRun)
    {
        float speed = isRun ? runSpeed : walkSpeed;

        con.Animation.SetMove(speed);

        Vector3 move = Vector3.up * dir * speed;
        con.cc.Move(move * Time.deltaTime);
    }
}