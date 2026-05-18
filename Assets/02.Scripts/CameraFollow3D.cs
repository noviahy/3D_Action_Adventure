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
    // [SerializeField] private Animator animator;

    [Header("Transform")]
    [SerializeField] private Transform player; // 위아래 회전, 거리 조절
    [SerializeField] private Transform cam;
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform shoulder;

    [Header("BowAimed")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Rig upperBodyRig;
    [SerializeField] float bowOffset = -0.2f;
    float pitchDistanceOffset = -1f;
    private CinemachineThirdPersonFollow follow;
    private float normalDistance;
    // private float turnAccum;

    [Header("Camera")]
    [SerializeField] float rotationSpeed = 120f;
    [SerializeField] float BowRotationSpeed = 60f;
    [SerializeField] float minPitch = -40f;
    [SerializeField] float maxPitch = 30f;
    private float yaw;
    private float pitch;
    private float currentRotationSpeed;

    [Header("LockOn")]
    [SerializeField] float switchThreshold = 0.2f; // 마우스 얼마나 움직이면 타겟 전환할지
    [SerializeField] float switchCooldown = 0.3f; // 연속 전환 방지
    private float switchTimer;
    private float scanTimer = 0.2f;
    private List<Transform> enemies = new List<Transform>();
    private Transform target;

    [Header("Renderer")]
    private List<Material> mats = new List<Material>();
    private float currentAlpha = 1f;

    public Camera MainCam { get; private set; }

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }

    private void Start()
    {
        MainCam = Camera.main;
        target = null;
        follow = vcam.GetComponent<CinemachineThirdPersonFollow>();
        normalDistance = follow.CameraDistance;
        RefreshRenderers();
    }
    private void Update()
    {
        camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

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
        SetAlpha(currentAlpha);

        float targetDistance = normalDistance;
        float speed = 6f;

        // pitch를 0~1로 변환
        float pitchT = Mathf.InverseLerp(0f, minPitch, pitch);
        // 위를 볼수록 카메라 가까워짐
        targetDistance += pitchDistanceOffset * pitchT;

        currentRotationSpeed = rotationSpeed;
        // Bow Charging 상태 : 카메라 정면 보기
        if (bowAttack.BowAimed || bowAttack.Standby)
        {
            currentRotationSpeed = BowRotationSpeed;
            BowChargingCam();
            targetDistance += bowOffset;
        }

        follow.CameraDistance = Mathf.Lerp(follow.CameraDistance, targetDistance, Time.deltaTime * speed);

        // 카메라 수동 회전: 타켓 없음 기본
        if (target == null)
        {
            yaw += input.MouseX * currentRotationSpeed * Time.deltaTime;
            pitch += input.MouseY * currentRotationSpeed * Time.deltaTime;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        // 록온 상태: 타겟 있음
        if (target != null && !bowAttack.BowAimed) // Bow 들고 있을땐 록온이 안되긴 하는데 혹시 모르니까
        {
            Vector3 dir = (target.position - pivot.position).normalized;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            // 부드럽게 카메라 회전
            pivot.rotation = Quaternion.Slerp(
                pivot.rotation,
                targetRot,
                10f * Time.deltaTime
            );

            // pitch 값도 같이 동기화 (중요)
            Vector3 angles = pivot.rotation.eulerAngles;
            pitch = angles.x;
            yaw = angles.y;
        }
    }
    private void LateUpdate()
    {
        // 타켓 변경 딜레이 
        switchTimer -= Time.deltaTime;
        // 타켓 스켄 딜레이
        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            ScanEnemies();
            scanTimer = 0.2f;
        }

        if (target != null)
            lockOnTarget.position = (player.position + target.position) * 0.5f + Vector3.up * 0.5f;

        // 록온 코드
        HandleLockOn();

        // 활 과녁
        Vector3 aimPos =
            cam.position +
            cam.forward * 10f;

        aimTarget.position = aimPos;

        float targetWeight = 0f;

        if (!bowAttack.BowShoot)
            targetWeight = 0.9f;

        upperBodyRig.weight = Mathf.Lerp(upperBodyRig.weight, targetWeight, Time.deltaTime * 10);

    }
    private void BowChargingCam()
    {
        Vector3 dir = camForward;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        player.rotation = Quaternion.Slerp(player.rotation, targetRot, 15f * Time.deltaTime);
    }

    private void SetAlpha(float alpha)
    {
        foreach (var mat in mats)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.SetColor("_BaseColor", c);
        }
    }
    private void ScanEnemies() // HandlLockOn과 FindTarget에서의 중복 코드
    {
        enemies.Clear();// 전 프레임에 찾았던 List 삭제

        Collider[] hits = Physics.OverlapSphere(player.position, 8f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.transform);
        }
    }

    private void HandleLockOn()
    {
        // 락온 꺼지면 타겟 삭제
        if (!input.IsLockOn)
        {
            target = null;
            return;
        }

        // LockOn할 타겟 찾기
        if (target == null)
        {
            FindTarget();

            if (target == null)
            {
                input.RequestLockOn(false);
                return;
            }
        }

        // 유효성 체크
        ValidateTarget();

        // 타겟 검색 후에도 없다면 return
        // 그러면 계속 IsLockOn인 상태가 아닌가? 풀어줘야할 것 같은데 실패하면...
        if (target == null)
            return;

        // 타겟 변경 코드
        if (switchTimer <= 0f)
        {
            if (input.MouseX > switchThreshold) // 방향 검색
            {
                SwitchTarget(true); // 오른쪽
                switchTimer = switchCooldown;
            }

            if (input.MouseX < -switchThreshold)
            {
                SwitchTarget(false); // 왼쪽
                switchTimer = switchCooldown;
            }
        }
        vcam.LookAt = lockOnTarget;


        // 플레이어의 Object의 회전 코드 (여기 있으면 안되는거 아닌가?) Update에 있어야할 것 같은데 
        Vector3 dir = camForward; // 카메라 기준

        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);

            player.rotation = Quaternion.Slerp(
                player.rotation,
                lookRot,
                8f * Time.deltaTime
            );
        }
    }

    // 타겟 찾기
    private void FindTarget()
    {
        // 전 코드는 거리만 계산했기 떄문에 점수로 변경
        Transform bestTarget = null; // 타겟 위치
        float bestScore = Mathf.Infinity; // BestScore

        // 적만 필터링
        foreach (Transform enemy in enemies)
        {
            Vector3 dirToEnemy = (enemy.position - player.position).normalized; // 적 방향, 위에 같은 코드가 있음

            // 카메라 앞쪽만
            float forwardDot = Vector3.Dot(camForward, dirToEnemy); // 두 방향이 얼마나 같은지
            if (forwardDot < 0.3f) continue;

            // 적이 화면 중앙에서 얼마나 떨어져 있는지 계산
            Vector3 screenPos = MainCam.WorldToViewportPoint(enemy.position); // 3D 위치를 화면 좌표로 변경

            float screenX = Mathf.Abs(screenPos.x - 0.5f); // 중앙이 0이어야하기 때문에 -0.5
            float screenY = Mathf.Abs(screenPos.y - 0.5f);

            // 거리
            float dist = Vector3.Distance(player.position, enemy.position);

            // 점수 계산
            float score = dist * 0.5f + screenX * 2f + screenY * 1.5f;

            // 가장 찾은 점수 찾기
            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }
        target = bestTarget;
    }

    // 타겟 변경
    // FindTarget과 비슷함
    private void SwitchTarget(bool toRight)
    {
        Transform bestTarget = null;
        float bestScore = Mathf.Infinity;

        foreach (Transform enemy in enemies)
        {
            if (enemy == target) continue;

            Vector3 dirToEnemy = (enemy.position - player.position).normalized;

            // 앞쪽 필터
            float forwardDot = Vector3.Dot(camForward, dirToEnemy);
            if (forwardDot < 0.3f) continue;

            // 좌우 판별
            float sideDot = Vector3.Dot(cam.right, dirToEnemy);

            // 방향 필터링
            if (toRight && sideDot < 0) continue;
            if (!toRight && sideDot > 0) continue;

            Vector3 screenPos = MainCam.WorldToViewportPoint(enemy.position);

            float screenX = Mathf.Abs(screenPos.x - 0.5f);
            float screenY = Mathf.Abs(screenPos.y - 0.5f);

            // 거리 기준
            float dist = Vector3.Distance(player.position, enemy.position);


            float score =
                dist * 0.5f + // 거리
                screenX * 2f + // 좌우
                screenY * 1.5f; // 높이

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }

        if (bestTarget != null)
            target = bestTarget;
    }

    // 유효성 체크
    private void ValidateTarget()
    {
        // 타겟 없음 or 비활성화
        if (target != null)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                input.RequestLockOn(false);
                target = null;
                return;
            }

            float dist = Vector3.Distance(player.position, target.position);
            if (dist > 15f)
            {
                input.RequestLockOn(false);
                target = null;
                return;
            }
        }
    }
    private void RefreshRenderers()
    {
        mats.Clear();

        Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);

        foreach (var ren in renderers)
        {
            foreach (var mat in ren.materials)
            {
                mats.Add(mat);
            }
        }
    }
}

// 제자리 회전 코드
/*
if (!attack.BowShoot)
{
    turnAccum += input.MouseX * rotationSpeed * Time.deltaTime;
    if(turnAccum > 60f)
    {
        animator.CrossFadeInFixedTime($"Lower Body.TurnRight", 0.25f);
        turnAccum = 0f;
    }
    if (turnAccum < -60f)
    {
        animator.CrossFadeInFixedTime($"Lower Body.TurnLeft", 0.25f);
        turnAccum = 0f;
    }
}
귀찮아서 유기함
뭐랄까 이걸 만지려면 적당한 애니메이션과 60 전까지는 몸통만 돌아가게 또 조건을 만저야하는데
그렇게 하다간 끝도 안 날 것 같음
애초에 IK를 안 하는 이유도 이건건데
애니메이션을 넣으려면 Rig 때문에 또 레이어를 나눠서 발만 작동하게 해줘야하는데 
그러면 회전 전에 몸통 회전이랑 그 다음 발 회전까지 해줘야하는게 너무너무 큰 일이 되고
잘 보이지도 않아서 안 하기로 함
*/
