using System.Collections.Generic;
using UnityEngine;

public class LockOnSystem
{
    private readonly InputManager input;
    private readonly Transform player;
    private readonly Transform cam;
    private readonly Camera mainCam;
    private readonly LayerMask obstacleMask;
    private readonly float switchThreshold;
    private readonly float switchCooldown;
    public LockOnSystem(
        InputManager input,
        Transform player,
        Transform cam,
        Camera mainCam,
        LayerMask obstacleMask,
        float switchThreshold,
        float switchCooldown)
    {
        this.input = input;
        this.player = player;
        this.cam = cam;
        this.mainCam = mainCam;
        this.obstacleMask = obstacleMask;
        this.switchThreshold = switchThreshold;
        this.switchCooldown = switchCooldown;
    }
    public Vector3 CamForward { get; set; }
    private float switchTimer;
    private float scanTimer = 0.2f;
    private List<Transform> enemies = new List<Transform>();
    private Transform target;
    private float invisibleTimer = 0f;
    private const float invisibleDelay = 0.4f;
    public Transform Target => target;

    public void UpdateLockOn()
    {
        switchTimer -= Time.deltaTime;
        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            ScanEnemies();
            scanTimer = 0.2f;
        }

        HandleLockOn();
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
            float forwardDot = Vector3.Dot(CamForward, dirToEnemy); // 두 방향이 얼마나 같은지
            if (forwardDot < 0.3f) continue;

            // 적이 화면 중앙에서 얼마나 떨어져 있는지 계산
            Vector3 screenPos = mainCam.WorldToViewportPoint(enemy.position); // 3D 위치를 화면 좌표로 변경

            float screenX = Mathf.Abs(screenPos.x - 0.5f); // 중앙이 0이어야하기 때문에 -0.5
            float screenY = Mathf.Abs(screenPos.y - 0.5f);

            // 거리
            float dist = Vector3.Distance(player.position, enemy.position);

            // 점수 계산
            float score = dist * 0.5f + screenX * 2f + screenY * 1.5f;

            // 가장 낮은 점수 찾기
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
            float forwardDot = Vector3.Dot(CamForward, dirToEnemy);
            if (forwardDot < 0.3f) continue;

            // 좌우 판별
            float sideDot = Vector3.Dot(cam.right, dirToEnemy);

            // 방향 필터링
            if (toRight && sideDot < 0) continue;
            if (!toRight && sideDot > 0) continue;

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
        if (target == null)
            return;

        // 비활성화 체크
        if (!target.gameObject.activeInHierarchy)
        {
            input.RequestLockOn(false);
            target = null;
            invisibleTimer = 0f;
            return;
        }

        // 거리 체크
        float dist = Vector3.Distance(player.position, target.position);

        if (dist > 15f)
        {
            input.RequestLockOn(false);
            target = null;
            invisibleTimer = 0f;
            return;
        }

        // 화면 + 가림 체크
        if (!IsTargetVisible())
        {
            invisibleTimer += Time.deltaTime;

            if (invisibleTimer >= invisibleDelay)
            {
                input.RequestLockOn(false);
                target = null;
                invisibleTimer = 0f;
            }
        }
        else
        {
            invisibleTimer = 0f;
        }
    }
    private bool IsTargetVisible()
    {
        if (target == null)
            return false;

        // 화면 좌표
        Vector3 screenPos = mainCam.WorldToViewportPoint(target.position);

        // 카메라 뒤
        if (screenPos.z <= 0f)
            return false;

        // 화면 밖
        if (screenPos.x < -0.1f || screenPos.x > 1.1f ||
    screenPos.y < -0.1f || screenPos.y > 1.1f)
            return false;

        // 지형에 가려졌는지 검사
        Vector3 origin = mainCam.transform.position;

        // 적의 중심 대신 가슴 높이를 향해 검사
        Vector3 targetPoint = target.position + Vector3.up * 1.2f;

        Vector3 dir = targetPoint - origin;
        float dist = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstacleMask))
        {
            Debug.Log(hit.transform.name);
            return false;
        }

        return true;
    }
}
