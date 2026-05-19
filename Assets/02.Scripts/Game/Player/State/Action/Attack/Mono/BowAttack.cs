using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BowAttack : PlayerBehaviour
{
    [SerializeField] CrossHair crossHair;
    [SerializeField] ArrowPool arrowPool;
    [SerializeField] Renderer arrowRenderer;
    [SerializeField] Transform firePoint;
    public bool BowAimed { get; private set; }
    public bool BowShoot { get; private set; }
    public bool Standby { get; private set; }

    private Coroutine exitCoroutine;
    private Coroutine releaseCoroutine;

    public bool showCrosshair { get; private set; } = false;
    public BowAttack(PlayerController controller)
    {
        con = controller;
    }
    private void Start()
    {
        currentBowState = bowState.Idle;
        Standby = false;
        BowShoot = true;
        arrowRenderer.enabled = false;
    }
    // Bow
    public enum bowState
    {
        Idle,
        Enter,
        Released,
        Exiting
    }
    public bowState currentBowState { get; private set; }
    private bowState preBowState;
    public void RequestBowAttack() // 외부에서 호출하는 유일한 함수입니다.
    {
        if (currentBowState == bowState.Idle) // 외부에선 꼬임 방지를 위해 Idle에서만 상태를 변경할 수 있게 했어요
            ChangeBowState(bowState.Enter);
    }

    private void ChangeBowState(bowState state)
    {
        preBowState = currentBowState;
        currentBowState = state;

        switch (state)
        {
            case bowState.Idle:
                break;
            case bowState.Enter:
                StartCoroutine(BowEnter());
                break;
            case bowState.Released:
                if (releaseCoroutine != null)
                    StopCoroutine(releaseCoroutine);
                if (exitCoroutine != null)
                {
                    StopCoroutine(exitCoroutine);
                    exitCoroutine = null;
                    con.Animation.SetLayerWeight(3, 1);
                }
                releaseCoroutine = StartCoroutine(BowRelease());
                break;
            case bowState.Exiting:
                exitCoroutine = StartCoroutine(BowExit());
                break;
        }
    }

    IEnumerator BowEnter()
    {
        showCrosshair = true;
        BowAimed = true;
        BowShoot = false;
        arrowRenderer.enabled = true;
        con.Animation.PlayLoadBow();

        // 레이어 활성화
        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * 4;
            con.Animation.SetLayerWeight(3, t);

            yield return null;
        }
        con.Animation.SetLayerWeight(3, 1);

        ChangeBowState(bowState.Released);
    }
    IEnumerator BowRelease()
    {
        if (preBowState != bowState.Enter)
        {
            arrowRenderer.enabled = true;
            con.Animation.PlayLoadBow();
            showCrosshair = true;
            BowAimed = true;
            BowShoot = false;
        }

        float force = 0;
        while (con.Input.BowCharging)
        {
            force += Time.deltaTime * 0.7f;
            force = Mathf.Clamp01(force);
            if (force < 0.2f)
                force = 0.2f;
            yield return null;
        }

        // 활 생성
        Vector3 dir = GetShootDirection();
        arrowPool.GetArrow(dir, firePoint, force);
        arrowRenderer.enabled = false;
        showCrosshair = false;
        crossHair.RequestCorssHairFO();

        con.Animation.PlayUpperBody("Release");
        con.Animation.PlayLowerBody("BowIdle");
        float t = 0;

        // 활 쏘는 모션까지 좀 기다려야 rig가 쏘는 애니메이션에도 활성화된 상태일 수 있는 www
        while (t <= 0.3f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        BowShoot = true;

        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);

        Standby = true;
        BowAimed = false;

        // 연사 방지 대기시간
        yield return new WaitForSeconds(0.5f);

        t = 0;
        while (t <= 0.3f)
        {
            t += Time.deltaTime;

            // 한번더!
            if (con.Input.BowCharging)
                ChangeBowState(bowState.Released);
            yield return null;
        }
        releaseCoroutine = null;
        ChangeBowState(bowState.Exiting);
    }
    // 이건 왜 Idle로 안 뺐는가?
    // 그 이윤 Layer3번과 상호작용 하는게 없어서 그냥 여기서 0으로 가던 1으로 가던 상관 없음
    IEnumerator BowExit()
    {
        Standby = false;
        float t = 0;

        while (t <= 1)
        {
            if (con.Input.BowCharging)
            {
                ChangeBowState(bowState.Released);
                yield break;
            }
            t += Time.deltaTime * 2;
            con.Animation.SetLayerWeight(3, 1 - t);

            yield return null;
        }
        con.Animation.SetLayerWeight(3, 0);
        exitCoroutine = null;
        ChangeBowState(bowState.Idle);
    }
    Vector3 GetShootDirection()
    {
        Ray ray = con.Cam.MainCam.ScreenPointToRay(
       new Vector3(Screen.width / 2, Screen.height / 2)
   );

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint =
                ray.origin + ray.direction * 100f;
        }

        float distance =
            Vector3.Distance(
                firePoint.position,
                targetPoint
            );

        // 카메라 정면 방향
        Vector3 camDir =
            con.Cam.MainCam.transform.forward;

        // 실제 목표 방향
        Vector3 targetDir =
            (targetPoint - firePoint.position).normalized;

        // 가까우면 camDir 비중 높음
        float blend =
            Mathf.InverseLerp(2f, 10f, distance);

        Vector3 dir =
            Vector3.Lerp(
                camDir,
                targetDir,
                blend
            ).normalized;

        // 원거리 보정
        float t =
            Mathf.InverseLerp(5f, 20f, distance);

        dir +=
            con.Cam.MainCam.transform.right * (0.035f * t)
            - Vector3.up * 0.1f;

        dir.Normalize();

        return dir;
    }
}
