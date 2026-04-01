using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow3D : MonoBehaviour
{
    [SerializeField] Transform cameraPivot;
    [SerializeField] InputManager input;
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform player;

    [Header("Distance")]
    [SerializeField] private float defaultDistance;
    [SerializeField] private float hideDistance;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Etc")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] float switchThreshold = 0.5f;
    [SerializeField] float switchCooldown = 0.3f;

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }

    private Transform cameraRoot;
    private Transform target;
    private float switchTimer;
    private float yaw;
    private float pitch;
    private float currentDistance;

    public Transform Target => target;

    private void Start()
    {
        cameraRoot = transform;
        cameraRoot.position = player.position;
        currentDistance = defaultDistance;
    }
    void LateUpdate()
    {
        if (!input.isLockOn)
            target = null;

        if (input.isLockOn)
            ValidateTarget();

        // LockOn할 타겟 찾기
        if (input.isLockOn && target == null)
            FindTarget();

        if (input.isLockOn && target != null)
        {
            switchTimer -= Time.deltaTime;

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

        if (input.isLockOn && target != null)
        {
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
                yaw += input.MouseX * sensitivity * Time.deltaTime;
                cameraRoot.rotation = Quaternion.Euler(0, yaw, 0);
            }

            pitch -= input.MouseY * sensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -40f, 70f);

            cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);

            // 카메라 방향
            camForward = new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z).normalized;
            camRight = new Vector3(cameraRoot.right.x, 0, cameraRoot.right.z).normalized;

            // 벽 충돌 처리
            HandleCollision();

            // Player 숨김
            playerRenderer.enabled = currentDistance > hideDistance;
        }
    }
    private void HandleCollision()
    {
        Vector3 dir = (cameraPivot.position - cameraRoot.position).normalized;

        Ray ray = new Ray(cameraRoot.position, dir);
        RaycastHit hit;

        float targetDistance = defaultDistance;

        if (Physics.Raycast(ray, out hit, targetDistance, wallLayer))
            targetDistance = hit.distance - 0.2f;

        // 최소 거리 제한 (카메라가 너무 붙지 않게)
        targetDistance = Mathf.Max(0.5f, targetDistance);

        // 부드럽게 거리 변경
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, 10f * Time.deltaTime);

        // 실제 카메라 위치
        cameraPivot.localPosition = new Vector3(0, 0, -currentDistance);
    }
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

            float dot = Vector3.Dot(cameraRoot.forward, dir);
            if (dot < 0.5f) continue;

            float dist = Vector3.Distance(player.position, hit.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                bestTarget = hit.transform;
            }
        }
        target = bestTarget;
    }
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

            if (toRight && side < 0) continue;
            if (!toRight && side > 0) continue;

            // 거리 기준 선택
            float dist = Vector3.Distance(player.position, hit.transform.position);

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

        if (dist > 15f)
            input.RequestLockOn(false);
    }
}
