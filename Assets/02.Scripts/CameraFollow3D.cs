using UnityEngine;

public class CameraFollow3D : MonoBehaviour

{
    [SerializeField] Transform cameraPivot; // 카메라의 중심
    [SerializeField] private Transform player; // 위아래 회전, 거리 조절

    // 마우스 입력으로 yaw/pitch 조절
    [SerializeField] InputManager input;
    [SerializeField] private float sensitivity;

    [Header("Distance")]
    [SerializeField] private float defaultDistance;
    [SerializeField] private float hideDistance;
    private float currentDistance;

    [Header("LockOn")]
    [SerializeField] float switchThreshold = 0.5f; // 마우스 얼마나 움직이면 타겟 전환할지
    [SerializeField] float switchCooldown = 0.3f; // 연속 전환 방지
    private Transform target;
    private float switchTimer;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Etc")]
    [SerializeField] private Renderer playerRenderer;

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }

    private Transform cameraRoot;
    private float yaw;
    private float pitch;

    public Transform Target => target;

    private void Start()
    {
        cameraRoot = transform; // 코드 가독성? 카메라의 중심 역할
        cameraRoot.position = player.position;
        currentDistance = defaultDistance; 
    }
    void LateUpdate()
    {
        // 락온 꺼지면 타겟 삭제
        if (!input.IsLockOn)
            target = null;

        // 켜져 있으면 유효성 체크
        if (input.IsLockOn)
            ValidateTarget();

        // LockOn할 타겟 찾기
        if (input.IsLockOn && target == null)
            FindTarget();

        if (input.IsLockOn && target != null)
        {
            switchTimer -= Time.deltaTime;

            // 마우스 좌우로 움직이면
            // 생각해보니까 조이콘이랑 키보드랑 나눠야할듯
            // 키보드 사용시 코드 사용 안 해야겠음
            if (switchTimer <= 0f)
            {
                if (input.MouseX > switchThreshold)
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
        }

        // Player 따라가기
        cameraRoot.position = Vector3.Lerp(cameraRoot.position, player.position, 10f * Time.deltaTime);

        if (input.IsLockOn && target != null)
        {
            // Player -> 타겟 방향으로 회전
            Vector3 dir = target.position - player.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);

                cameraRoot.rotation = Quaternion.Lerp(
                    cameraRoot.rotation,
                    targetRot,
                    10f * Time.deltaTime
                );

                yaw = cameraRoot.eulerAngles.y;
            }
            else
            {
                // 수동 회전으로 fallback
                yaw += input.MouseX * sensitivity * Time.deltaTime;
                cameraRoot.rotation = Quaternion.Euler(0, yaw, 0);
            }

            // 상하 회전(위아래 회전 제한 포함)
            pitch -= input.MouseY * sensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -40f, 70f);

            cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);

            // 카메라 방향 계산(Player 플레이어 이동 방향 기준)
            camForward = new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z).normalized;
            camRight = new Vector3(cameraRoot.right.x, 0, cameraRoot.right.z).normalized;

            // 벽 충돌 처리
            HandleCollision();

            // Player 숨김
            playerRenderer.enabled = currentDistance > hideDistance;
        }
    }
    // 카메라 벽 충돌
    private void HandleCollision()
    {
        Vector3 dir = (cameraPivot.position - cameraRoot.position).normalized;

        // 카메라 뒤쪽으로 Ray 쏨
        Ray ray = new Ray(cameraRoot.position, dir);
        RaycastHit hit;

        float targetDistance = defaultDistance;

        // 벽 바로 앞까지 당김
        if (Physics.Raycast(ray, out hit, targetDistance, wallLayer))
            targetDistance = hit.distance - 0.2f;

        // 최소 거리 제한 (카메라가 너무 붙지 않게)
        targetDistance = Mathf.Max(0.5f, targetDistance);

        // 부드럽게 거리 변경
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, 10f * Time.deltaTime);

        // 실제 카메라 위치
        cameraPivot.localPosition = new Vector3(0, 0, -currentDistance);
    }

    // 타겟 찾기
    private void FindTarget()
    {
        // 범위 안 적 찾기
        Collider[] hits = Physics.OverlapSphere(player.position, 10f);

        float minDist = Mathf.Infinity;
        Transform bestTarget = null;

        // 적만 필터링
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 dir = (hit.transform.position - player.position).normalized;

            // 카메라 앞쪽만
            float dot = Vector3.Dot(cameraRoot.forward, dir);
            if (dot < 0.5f) continue;

            float dist = Vector3.Distance(player.position, hit.transform.position);

            // 가장 가까운 적 선택
            if (dist < minDist)
            {
                minDist = dist;
                bestTarget = hit.transform;
            }
        }
        target = bestTarget;
    }

    // 타겟 변경
    private void SwitchTarget(bool toRight)
    {
        Collider[] hits = Physics.OverlapSphere(player.position, 10f);

        Transform bestTarget = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            if (hit.transform == target) continue;

            Vector3 dir = (hit.transform.position - player.position).normalized;

            // 앞쪽 필터
            float forwardDot = Vector3.Dot(cameraRoot.forward, dir);
            if (forwardDot < 0.3f) continue;

            // 좌우 판별
            float side = Vector3.Dot(cameraRoot.right, dir);

            // 방향 필터링
            if (toRight && side < 0) continue;
            if (!toRight && side > 0) continue;

            // 거리 기준 선택
            float dist = Vector3.Distance(player.position, hit.transform.position);

            // 가장 가까운 적 선택
            if (dist < bestScore)
            {
                bestScore = dist;
                bestTarget = hit.transform;
            }
        }

        if (bestTarget != null)
        {
            target = bestTarget;
        }
    }

    // 유효성 체크
    private void ValidateTarget()
    {
        // 타겟 없음 or 비활성화
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            input.RequestLockOn(false);
            return;
        }

        // 거리 체크
        float dist = Vector3.Distance(player.position, target.position);

        // 너무 멀어지면제
        if (dist > 15f)
            input.RequestLockOn(false);
    }
}