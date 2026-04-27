using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow3D : MonoBehaviour

{
    [SerializeField] private Transform player; // 위아래 회전, 거리 조절
    [SerializeField] private InputManager input;
    [SerializeField] private Transform cam;
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform shoulder;

    [Header("LockOn")]
    [SerializeField] float switchThreshold = 0.2f; // 마우스 얼마나 움직이면 타겟 전환할지
    [SerializeField] float switchCooldown = 0.3f; // 연속 전환 방지
    private Transform target;
    private float switchTimer;
    private float rotSpeed = 720f;
    private float scanTimer = 0.2f;
    private Camera mainCam;
    private List<Transform> enemies = new List<Transform>();
    public Transform Target => target;

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }

    private void Start()
    {
        mainCam = Camera.main;
    }
    private void LateUpdate()
    {
        camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        pivot.position = shoulder.position;

        // 타켓 변경 딜레이 
        switchTimer -= Time.deltaTime;
        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            ScanEnemies();
            scanTimer = 0.2f;
        }

        HandleLockOn();
    }
    private void ScanEnemies()
    {
        enemies.Clear();

        Collider[] hits = Physics.OverlapSphere(player.position, 10f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.transform);
        }
    }

    private void HandleLockOn()
    {
        // 유효성 체크
        ValidateTarget();

        // 락온 꺼지면 타겟 삭제
        if (!input.IsLockOn)
        {
            target = null;
            return;
        }

        // LockOn할 타겟 찾기
        if (target == null)
            FindTarget();

        if (target == null)
            return;

        // 타겟 변경 코드
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
        if (vcam != null && vcam.LookAt != target)
        {
            vcam.LookAt = target;
        }

        Vector3 dir = target.position - player.position;
        dir.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        /*
        player.rotation = Quaternion.RotateTowards(
            player.rotation,
            lookRot,
            rotSpeed * Time.deltaTime
        );
        */
    }

    // 타겟 찾기
    private void FindTarget()
    {
        Transform bestTarget = null;
        float bestScore = Mathf.Infinity;

        // 적만 필터링
        foreach (Transform enemy in enemies)
        {

            Vector3 dirToEnemy = (enemy.position - player.position).normalized;

            // 카메라 앞쪽만
            float forwardDot = Vector3.Dot(camForward, dirToEnemy);
            if (forwardDot < 0.3f) continue;

            Vector3 screenPos = mainCam.WorldToViewportPoint(enemy.position);

            float screenX = Mathf.Abs(screenPos.x - 0.5f);
            float screenY = Mathf.Abs(screenPos.y - 0.5f);

            float dist = Vector3.Distance(player.position, enemy.position);

            float score = dist * 0.5f + screenX * 2f + screenY * 1.5f;

            // 가장 가까운 적 선택
            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }
        target = bestTarget;
    }

    // 타겟 변경
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
            if(!toRight && sideDot > 0) continue;

            Vector3 screenPos = mainCam.WorldToViewportPoint(enemy.position);

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