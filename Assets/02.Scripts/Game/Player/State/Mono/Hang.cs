using System.Collections;
using UnityEngine;
using static LocomotionState;
public class Hang : PlayerBehaviour
{
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform HangCheck;

    public RaycastHit Wall {  get; private set; }
    public Transform PreWall { get; private set; }
    public Coroutine HangCoroutine { get; private set; }
    public Coroutine WaitHangMove { get; private set; }
    public bool canClimb { get; private set; } = true;


    private float sphereRadius = 0.3f;
    private float sphereDistance = 0.3f;

    public void SetPreWall()
    {
        PreWall = Wall.transform;
    }

    public bool TryEnterHang()
    {
        if (!CheckHang(out RaycastHit hit))
            return false;

        if (!hit.transform.CompareTag("Hangable") &&
            !hit.transform.CompareTag("Climbable"))
            return false;

        if (hit.transform == PreWall)
            return false;

        Wall = hit;

        con.Movement.SetHanging(hit);

        return true;
    }

    private bool CheckHang(out RaycastHit hit)
    {
        bool backHit = Physics.SphereCast(
            HangCheck.position,
            sphereRadius,
            -con.Player.transform.forward,
            out RaycastHit back,
            sphereDistance,
            wallLayer);

        bool frontHit = Physics.SphereCast(
            HangCheck.position,
            sphereRadius,
            con.Player.transform.forward,
            out RaycastHit front,
            sphereDistance,
            wallLayer);

        if (backHit && frontHit)
        {
            hit = back.distance < front.distance ? back : front;
            return true;
        }

        if (backHit)
        {
            hit = back;
            return true;
        }

        if (frontHit)
        {
            hit = front;
            return true;
        }

        hit = default;
        return false;
    }

    public void Enter()
    {
        con.Player.RequestWeaponRendererOff();

        canClimb = true;

        if (!con.cc.isGrounded)
        {
            con.Animation.PlayHang();
            RequestWaitHangMove();
        }
        else
        {
            RequestJumpHang();
        }

        con.LayerController.RequestLayer1On(0.2f);
        con.LayerController.RequestLayer2Off(0.2f);
    }

    public void RequestWaitHangMove()
    {
        WaitHangMove = StartCoroutine(WaitHangMoveCoroutine());
    }
    public void RequestJumpHang()
    {
        WaitHangMove = StartCoroutine(EnterJumpHang());
    }
    IEnumerator WaitHangMoveCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        WaitHangMove = null;
    }
    IEnumerator EnterJumpHang()
    {
        con.cc.Move(con.Player.transform.up * 0.3f);
        con.RootMotionController.RequestRootMotion(true);
        con.Animation.PlayJumpHang();

        yield return new WaitUntil(() =>
    con.Animator.GetCurrentAnimatorStateInfo(1).IsName("JumpHang"));
        yield return new WaitUntil(() =>
            con.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.8f);
        con.RootMotionController.RequestRootMotion(false);

        WaitHangMove = null;
    }

    public void UpdateHang()
    {
        if (WaitHangMove != null)
            return;

        ExitHang();
        TryClimb();
        TryMoveSide();
    }
    private void ExitHang()
    {
        if (!con.Input.BackBuffered)
            return;

        con.cc.Move(-con.Player.transform.forward * 0.3f);

        con.Locomotion.ChangeState(LocomotionSubState.Airborne);
    }
    private void TryMoveSide()
    {
        if (con.Input.side >= 0.5f)
            RequestHangMove(1);

        if (con.Input.side <= -0.5f)
            RequestHangMove(-1);
    }
    private void TryClimb()
    {
        if (con.Input.forward < 0.8f)
            return;

        if (!canClimb)
            return;

        canClimb = false;

        con.cc.Move(-con.Player.transform.forward * 0.05f);

        CheckHang(out RaycastHit hit);

        Wall = hit;

        con.cc.Move(Vector3.up * 0.4f);

        con.Climb.SetTopPoint(Wall);
    }

    public void RequestHangMove(float value)
    {
        if (HangCoroutine != null)
            return;
        HangCoroutine = StartCoroutine(HangMove(value));
    }
    IEnumerator HangMove(float value)
    {
        if (value == 1)
            con.Animation.PlayHangRight();
        else
            con.Animation.PlayHangLeft();

        yield return new WaitForSeconds(0.3f);
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + con.Player.transform.right * value * 0.5f;

        float duration = 0.8f;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;

            float t = time / duration;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, t);

            Vector3 move = nextPos - transform.position;

            con.Movement.FallMove(move);

            yield return null;
        }
        HangCoroutine = null;
    }
    public void Exit()
    {
        con.LayerController.RequestLayer1Off(0.2f);

        if (con.Player.currentWeaponType != Player.WeaponType.Default)
            con.LayerController.RequestLayer2On(0.2f);

        con.Player.RequestWeaponRendererOn();
    }
}
