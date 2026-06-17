using UnityEngine;

public class PlayerMovement
{
    private PlayerController con;

    private float walkSpeed = 3f;
    private float runSpeed = 7f;

    private float lockOnSpeed = 1.5f;

    public bool isJumping { get; private set; } = false;
    private float jumpForwardPower = 3f;
    private float jumpUpPower = 5f;

    private float rotSpeed = 10f;

    private float ladderSpeed = 2f;
    private float fastLadderSpeed = 4f;

    private Vector3 jumpDir;
    private float yVelocity;

    public bool JustLanded { get; private set; } = false;

    public PlayerMovement(PlayerController controller)
    {
        con = controller;
    }
    // 보통 여기에 진짜 천천히 움직이는걸 하나 더 만들어야 하지만 그건 다음 게임에 추가
    public void Move(Vector3 inputDir, bool isRun)
    {
        if (con.cc.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        Vector3 horizontal;

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

        Vector3 move = new Vector3(horizontal.x, yVelocity, horizontal.z);
        con.cc.Move(move * Time.deltaTime);
    }
    // 트리거시 호출하는 코드
    public void StartJump()
    {
        if (con.Input.MoveInput.sqrMagnitude > 0.01f)
            jumpDir = con.Input.MoveInput.normalized;
        else
            jumpDir = con.Player.transform.forward.normalized;
        yVelocity = jumpUpPower;

        con.Animation.PlayJump();
    }
    public void Airborne()
    {
        Vector3 horizontal = jumpDir * jumpForwardPower;

        yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = new Vector3(
            horizontal.x,
            yVelocity,
            horizontal.z);

        con.cc.Move(move * Time.deltaTime);

        if (con.cc.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
            JustLanded = true;
        }
    }
    public void ChangeJustLanded()
    {
        JustLanded = false;
    }
    public void ResetYVelocity()
    {
        yVelocity = 0;
    }

    public void FallMove(Vector3 move)
    {
        con.cc.Move(move);
    }

    public void Climb(float dir, bool isRun)
    {
        float speed = isRun ? fastLadderSpeed : ladderSpeed;


        Vector3 move = Vector3.up * dir * speed;
        float climbSpeed = Mathf.Abs(dir * speed);
        con.Animation.SetMove(climbSpeed);

        con.cc.Move(move * Time.deltaTime);
    }


}