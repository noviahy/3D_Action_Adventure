using UnityEngine;

public class InputManager : MonoBehaviour
{
    // 이동 방향
    public Vector3 MoveInput { get; private set; }

    // 공격
    public bool AttackPressed { get; private set; }
    public bool LightAttack { get; private set; }
    public bool HeavyAttack { get; private set; }

    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool DodgeBuffered { get; private set; }

    // 활
    public bool BowCharging { get; private set; }

    // 아이템
    public bool ItemBuffered { get; private set; }

    // 록온
    public bool IsLockOn { get; private set; } = false;

    // 상호작용 키
    public bool InteractionPressed { get; private set; }
    public int ChangeItem { get; private set; }
    public int ChangeWeapon { get; private set; }
    public bool inputItem { get; private set; }
    public bool inputWeapon { get; private set; }
    public bool BackBuffered {  get; private set; }

    // 카메라 회전
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float Direction { get; private set; }
    public float PreDirection { get; private set; }

    // 상태
    public bool LocomotionPressed { get; private set; }
    public bool ActionPressed { get; private set; }
    public float forward { get; private set; }
    public float side { get; private set; }

    private PlayerController con;
    private PlayerInputAction inputAction;

    private float prevRT;
    private float deadZone = 0.2f;

    private float dodgeTime = 0.2f;
    private float dodgeTimer;

    private float ItemTime = 0.12f;
    private float ItemTimer;

    private float attackTime = 0.15f;
    private float attackTImer;
    public InputMode CurrentInputMode { get; private set; }

    public enum InputMode
    {
        Gameplay,
        UI,
        Dialogue,
        Traversal
    }
    public void Init(PlayerController controller)
    {
        con = controller;
    }
    public void ChangeInputMode(InputMode mode)
    {
        if (CurrentInputMode == mode) 
            return;

        CurrentInputMode = mode;
    }

    private void Awake()
    {
        inputAction = new PlayerInputAction();
        BowCharging = false;
        // 
        CurrentInputMode = InputMode.Gameplay;
    }
    private void OnEnable()
    {
        inputAction.Enable();
    }
    private void OnDisable()
    {
        inputAction.Disable();
    }
    void Update()
    {
        // 카메라 회전용
        Vector2 look = inputAction.Player.Look.ReadValue<Vector2>();
        MouseX = look.x;
        MouseY = -look.y;
        
        // Player 이동용
        Vector2 move = inputAction.Player.Move.ReadValue<Vector2>();
        forward = move.y;
        side = move.x;

        // 떨림 방지
        if (Mathf.Abs(forward) < deadZone)
            forward = 0;
        if (Mathf.Abs(side) < deadZone)
            side = 0;

        // 방향 계산
        MoveInput = con.Cam.camForward * forward + con.Cam.camRight * side;
        // 떨림 방지
        if (MoveInput.sqrMagnitude < deadZone * deadZone)
            MoveInput = Vector3.zero;

        // 공격
        AttackInput();

        // 아이템 사용
        ItemInput();
        // 아이템 변경
        ItemChangeInput();

        // 무기 변경
        WeaponChangeInput();

        // 상호작용키
        InteractionPressed = inputAction.Player.Interaction.WasPressedThisFrame();

        // 달리기와 회피
        runDodgeInput();

        // 패링 상태
        ParryingPressed = inputAction.Player.Parry.IsPressed();

        // 활
        BowCharging = inputAction.Player.Bow.IsPressed();

        // Action State 설정
        ActionPressed = DodgeBuffered || InteractionPressed || ItemBuffered;

        // LockOn키
        if (inputAction.Player.LockOn.WasPressedThisFrame() && con.Player.currentWeaponType != Player.WeaponType.Bow)
        {
            IsLockOn = !IsLockOn;
        }
        con.Animation.SetLockOn(IsLockOn);
    }
    private void runDodgeInput()
    {
        RunPressed = inputAction.Player.Run.IsPressed();

        dodgeTimer -= Time.deltaTime;
        if (inputAction.Player.Run.WasPressedThisFrame() && IsLockOn)
        {
            DodgeBuffered = true;
            BackBuffered = true;
            dodgeTimer = dodgeTime;

        }
        if (dodgeTimer < 0)
        {
            DodgeBuffered = false;
            BackBuffered = false;
        }
    }
    private void AttackInput()
    {
        bool lightBuffered = inputAction.Player.Light.WasPressedThisFrame();
        float rt = inputAction.Player.Heavy.ReadValue<float>();

        bool isPressed = rt > 0.5f;
        bool wasPressed = prevRT > 0.5f;

        bool HeavyBuffered = isPressed && !wasPressed;

        prevRT = rt;

        attackTImer -= Time.deltaTime;
        if (lightBuffered || HeavyBuffered)
        {
            AttackPressed = true;
            attackTImer = attackTime;

            // 초기화
            LightAttack = false;
            HeavyAttack = false;

            // 마지막 값만 유지
            if (lightBuffered)
                LightAttack = true;
            if (HeavyBuffered)
                HeavyAttack = true;
        }
        if (attackTImer < 0)
        {
            AttackPressed = false;
        }
    }
    private void WeaponChangeInput()
    {
        if (inputAction.Player.NextWeapon.WasPressedThisFrame())
            ChangeWeapon = 1;
        else if (inputAction.Player.PrevWeapon.WasPressedThisFrame())
            ChangeWeapon = -1;
    }
    private void ItemChangeInput()
    {
        if (inputAction.Player.NextItem.WasPressedThisFrame())
            ChangeItem = 1;
        else if (inputAction.Player.PrevItem.WasPressedThisFrame())
            ChangeItem = -1;
    }
    private void ItemInput()
    {
        bool ItemPressed = inputAction.Player.Item.WasPressedThisFrame();


        ItemTimer -= Time.deltaTime;
        if(ItemPressed)
        {
            ItemBuffered = true;
            ItemTimer = ItemTime;
        }
        if(ItemTimer < -0)
        {
            ItemBuffered = false;
        }
    }
    public void AckAttack()
    {
        LightAttack = false;
        HeavyAttack = false;
    }
    public void RequestLockOn(bool value)
    {
        IsLockOn = value;
    }
    public void AckDodgeFinish()
    {
        DodgeBuffered = false;
    }
    public void AckWeaponInput()
    {
        ChangeWeapon = 0;
    }
    public void AckItemInput()
    {
        ChangeItem = 0;
    }
}
