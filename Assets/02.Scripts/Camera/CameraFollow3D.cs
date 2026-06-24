using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// 미래의 내가 나중에 NPC 대화 카메라 짤때 리팩토링 할거라 믿고있어 ㅇㅇ
public class CameraFollow3D : MonoBehaviour
{
    [Header("Code")]
    [SerializeField] private InputManager input;
    [SerializeField] private BowAttack bowAttack;
    [SerializeField] private Climb climb;
    [SerializeField] private LocomotionState locomotionState;

    [Header("C# Code")]
    private PlayerTransparency transparency;
    private CameraDistance camDistance;
    private BowCamera bowCam;
    private CameraRotation rotation;
    private LockOnSystem lockOnSystem;

    [Header("Transform")]
    [SerializeField] private Transform player; // 위아래 회전, 거리 조절
    [SerializeField] private Transform cam;
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform shoulder;

    [Header("BowAimed")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Rig upperBodyRig;
    [SerializeField] float bowOffset = -0.2f;
    float pitchDistanceOffset = -1f;
    private CinemachineThirdPersonFollow follow;
    private float normalDistance;

    [Header("Camera")]
    [SerializeField] float rotationSpeed = 120f;
    [SerializeField] float BowRotationSpeed = 60f;
    [SerializeField] float minPitch = -40f;
    [SerializeField] float maxPitch = 30f;
    private float currentRotationSpeed;

    [Header("LockOn")]
    [SerializeField] float switchThreshold = 0.2f; // 마우스 얼마나 움직이면 타겟 전환할지
    [SerializeField] float switchCooldown = 0.3f; // 연속 전환 방지
    [SerializeField] private LayerMask obstacleMask;
    private Coroutine coroutine;

    [Header("Renderer")]
    private float currentAlpha = 1f;

    public Camera MainCam { get; private set; }

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }

    private void Start()
    {
        MainCam = Camera.main;
        follow = vcam.GetComponent<CinemachineThirdPersonFollow>();
        normalDistance = follow.CameraDistance;

        // 카메라 코드 생성
        transparency = new PlayerTransparency();
        bowCam = new BowCamera();
        rotation = new CameraRotation(minPitch, maxPitch);
        lockOnSystem = new LockOnSystem(
    input,
    player,
    cam,
    MainCam,
    obstacleMask,
    switchThreshold,
    switchCooldown);

        follow = vcam.GetComponent<CinemachineThirdPersonFollow>();

        camDistance = new CameraDistance(
            follow,
            normalDistance,
            bowOffset,
            pitchDistanceOffset);


        transparency.RefreshRenderers(player);
    }
    private void Update()
    {
        camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        lockOnSystem.CamForward = camForward;
        /*
         * 카메라의 중심을 Player의 Shoulder로 옮겨줌
         * Player에있는걸 직접 사용하면 Player 회전 시 카메라도 같이 회전하기 때문에
         * 외부에 있는 피봇에 위치만 넣어주는걸로 변경
         */
        pivot.position = shoulder.position;

        float dist = Vector3.Distance(cam.position, player.position);

        // 최소/최대 거리 설정
        float minDist = 1.2f;   // 완전히 투명
        float maxDist = 2.0f;   // 완전히 보임

        // 0~1 사이 값으로 정규화
        float t = Mathf.InverseLerp(minDist, maxDist, dist);

        // 알파 적용
        currentAlpha = Mathf.Lerp(currentAlpha, t, Time.deltaTime * 10f);
        transparency.SetAlpha(currentAlpha);

        currentRotationSpeed = rotationSpeed;

        bool isBowAim = bowAttack.BowAimed || bowAttack.Standby;

        if (isBowAim)
        {
            currentRotationSpeed = BowRotationSpeed;
            bowCam.RotatePlayer(player, camForward);
        }

        // 카메라 거리 계산
        camDistance.UpdateDistance(
            rotation.Pitch,
            minPitch,
            isBowAim);

        // 카메라 수동 회전: 타켓 없음 기본
        if (lockOnSystem.Target == null)
        {
            rotation.RotateFree(
                pivot,
                input.MouseX,
                input.MouseY,
                currentRotationSpeed);
        }
        // 록온 상태: 타겟 있음
        if (lockOnSystem.Target != null && coroutine == null && !bowAttack.BowAimed)
        {
            rotation.RotateLockOn(
                pivot,
                lockOnSystem.Target);
        }

        if (climb.currentState == Climb.ClimbState.ArriveTop)
            follow.Damping.y = 4f;
        else
            follow.Damping.y = 1f;

        // 플레이어의 Object의 회전 코드 (여기 있으면 안되는거 아닌가?) Update에 있어야할 것 같은데 
        if (lockOnSystem.Target != null)
        {
            Vector3 dir = camForward; // 카메라 기준

            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f &&
                locomotionState.currentSubState != LocomotionState.LocomotionSubState.Airborne &&
                locomotionState.currentSubState != LocomotionState.LocomotionSubState.Hang)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);

                player.rotation = Quaternion.Slerp(
                    player.rotation,
                    lookRot,
                    8f * Time.deltaTime
                );
            }
        }

    }
    private void LateUpdate()
    {
        lockOnSystem.UpdateLockOn();

        // 활 과녁
        bowCam.UpdateAimTarget(cam, aimTarget);

        bowCam.UpdateRig(upperBodyRig, bowAttack.BowShoot);

    }
}
