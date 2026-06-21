using Unity.VisualScripting;
using UnityEngine;
using static LocomotionState;

public class PlayerMovement
{
    private PlayerController con;

    private float slowWalkSpeed = 1.5f;
    private float walkSpeed = 3.5f;
    private float runSpeed = 7f;
    private float fallingSpeed = 1f;

    private float lockOnSpeed = 1.5f;
    private float fallStartY;
    private float totalFallY;

    public bool isJumping { get; private set; } = false;
    private float jumpForwardPower = 3f;
    private float jumpUpPower = 6.5f;
    public bool hasLanded { get; private set; } = true;

    private float rotSpeed = 10f;

    private float ladderSpeed = 2f;
    private float fastLadderSpeed = 4f;

    private Vector3 jumpDir;
    private float yVelocity = 0f;

    private float hangOffset = 0.1f;

    public AirbornState currentAirbornState {  get; private set; }
    public enum AirbornState
    {
        Idle,
        Jump,
        Walk,
        SlowWalk,

    }

    public PlayerMovement(PlayerController controller)
    {
        con = controller;
    }
    // 보통 여기에 진짜 천천히 움직이는걸 하나 더 만들어야 하지만 그건 다음 게임에 추가
    public void Move(Vector3 inputDir)
    {
        if (con.cc.isGrounded && yVelocity < 0)
            yVelocity = -2.5f;

        Vector3 horizontal;
        float speed = 0f;

        switch (con.Locomotion.currentSubState)
        {
            case LocomotionState.LocomotionSubState.SlowWalk:
                speed = slowWalkSpeed;
                break;
            case LocomotionState.LocomotionSubState.Walk: 
                speed = walkSpeed;
                break;
            case LocomotionState.LocomotionSubState.Run:
                speed = runSpeed;
                break;
        }
        // 록온 시 speed를 덮어씌움
        if (con.Input.IsLockOn || con.BowAttack.BowAimed)
            speed = lockOnSpeed;
        if (con.Locomotion.currentSubState == LocomotionState.LocomotionSubState.Airborne)
            speed = fallingSpeed;
        
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
        isJumping = true;
        if (con.Input.MoveInput.sqrMagnitude > 0.01f)
            jumpDir = con.Input.MoveInput.normalized;
        else
            jumpDir = con.Player.transform.forward.normalized;
        yVelocity = jumpUpPower;

        con.Animation.PlayJump();
        con.Locomotion.RequestJumpCoroutine();
    }
    // Climb에서만 사용하는 코드
    // 떨어지기만 함
    // Locomotion의 Ariborn에서 사용하는 코드가 아님
    public void AirborneClimb()
    {
        yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = new Vector3(0, yVelocity, 0);

        con.cc.Move(move * Time.deltaTime);

        if (con.cc.isGrounded && yVelocity < 0)
        {
            yVelocity = -2.5f;
            con.Locomotion.RequestOffCoroutine();
        }
    }
    // Locomotion Airborne에서 사용하는 코드
    // 점프, 착지 혹은 그냥 떨어지기 담당
    // 다양한 조건이 있기 때문에 여기서 애니메이션 설정을 해주면 안됨
    // 순전히 떨어지기만 하는 코드
    public void SetStartY()
    {
        // 현재 높이 저장
        fallStartY = con.Player.transform.position.y;
        hasLanded = false;
        jumpDir = Vector3.zero;
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
    }
    public void CheckFallDistance()
    {

        totalFallY = fallStartY - con.Player.transform.position.y;
        yVelocity = -2f;
        isJumping = false;

        if (totalFallY < 2f && con.Locomotion.preSubState != LocomotionSubState.Hang)
        {
            con.Locomotion.ChangeState(LocomotionSubState.Idle);
        }
        else if (totalFallY < 3f || con.Locomotion.preSubState != LocomotionSubState.Run)
        {
            con.Animation.PlayLand();

            con.Locomotion.RequestOnCoroutine();
        }
        else if(totalFallY >=3f && con.Locomotion.preSubState == LocomotionSubState.Run)
        {
            con.StateMachine.RequestRoll();
        }
    }
    public void SetHanging(RaycastHit hit)
    {
        Vector3 pos = con.Player.transform.position;

        pos.x = hit.point.x + hit.normal.x * hangOffset;
        pos.z = hit.point.z + hit.normal.z * hangOffset;

        con.Player.transform.position = pos;

        Vector3 lookDir = -hit.normal;
        lookDir.y = 0f;

        con.Player.transform.rotation =
            Quaternion.LookRotation(lookDir);
    }
    public void ResetHasLand()
    {
        hasLanded = true;
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

    public void MoveBox()
    {

    }
}