using System;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public Vector3 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool LightAttack { get; private set; }
    public bool HeavyAttack { get; private set; }

    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool DodgeBuffered { get; private set; }

    public bool StartBowCharging { get; private set; }
    public bool BowCharging { get; private set; }
    public bool BowShoot { get; private set; }

    public bool IsLockOn { get; private set; } = false;

    public bool InteractionPressed { get; private set; }
    public int ChangeItem { get; private set; }
    public int ChangeWeapon { get; private set; }
    public bool inputItem { get; private set; }
    public bool inputWeapon { get; private set; }

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float Direction { get; private set; }
    public float PreDirection { get; private set; }
    public bool LocomotionPressed { get; private set; }
    public bool ActionPressed { get; private set; }
    public float forward { get; private set; }
    public float side { get; private set; }

    private PlayerController con;
    private float prevRT;
    private float deadZone = 0.2f;
    private float bufferTime = 0.3f;
    private float bufferTimer;
    private bool isPressed = false;

    public InputMode CurrentInput { get; private set; }

    public enum InputMode
    {
        PlayerInput,
        UIInput,
        InputLock
    }
    public void Init(PlayerController controller)
    {
        con = controller;
    }
    public void ChangeInputMode(InputMode mode)
    {
        if (CurrentInput == mode) return;

        CurrentInput = mode;
    }
    void Update()
    {
        MouseX = Input.GetAxis("LookX");
        MouseY = Input.GetAxis("LookY");

        forward = Input.GetAxisRaw("Vertical");
        side = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(forward) < deadZone)
            forward = 0;
        if (Mathf.Abs(side) < deadZone)
            side = 0;

        MoveInput = con.Cam.camForward * forward + con.Cam.camRight * side;
        if (MoveInput.sqrMagnitude < deadZone * deadZone)
            MoveInput = Vector3.zero;

        // 공격
        AttackInput();

        // 아이템 변경
        ItemInput();

        // 무기 변경
        WeaponInput();

        InteractionPressed = Input.GetButtonDown("Interaction");

        // 달리기와 회피
        runDodgeInput();

        // 패링 상태
        ParryingPressed = Input.GetButton("Parrying");

        // 활
        BowInput();

        // Action State 설정
        ActionPressed = AttackPressed || ParryingPressed || DodgeBuffered || BowCharging || InteractionPressed;

        // LockOn키
        if (Input.GetButtonDown("LockOn"))
        {
            IsLockOn = !IsLockOn;
        }
        con.Animation.SetLockOn(IsLockOn);
    }
    private void runDodgeInput()
    {
        RunPressed = Input.GetButton("Run");

        bufferTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Run") && IsLockOn)
        {
            DodgeBuffered = true;
            bufferTimer = bufferTime;
        }
        if (bufferTimer < 0)
        {
            DodgeBuffered = false;
        }
    }
    private void ItemInput()
    {
        float dir = Input.GetAxisRaw("ChangeItem");

        if (dir > 0.5f && !isPressed)
        {
            ChangeItem = 1;
            isPressed = true;
        }
        else if (dir < -0.5f)
        {
            ChangeItem = -1;
            isPressed = true;
        }
        else if (Mathf.Abs(dir) < 0.1f)
        {
            ChangeItem = 0;
            isPressed = false;
        }
    }
    private void WeaponInput()
    {

        float dir = Input.GetAxisRaw("ChangeWeapon");

        if (dir > 0.5f && !isPressed)
        {
            ChangeWeapon = 1;
            isPressed = true;
        }
        else if (dir < -0.5f)
        {
            ChangeWeapon = -1;
            isPressed = true;
        }
        else if (Mathf.Abs(dir) < 0.1f)
        {
            ChangeWeapon = 0;
            isPressed = false;
        }

    }
    private void BowInput()
    {
        float bow = Input.GetAxis("Bow");
        bool wasBowCharging = BowCharging;

        BowCharging = bow > 0.5f;
        StartBowCharging = BowCharging && !wasBowCharging;
        BowShoot = !BowCharging && wasBowCharging;
    }
    private void AttackInput()
    {
        LightAttack = Input.GetButtonDown("Light");
        float rt = Input.GetAxis("Heavy");

        bool isPressed = rt > 0.5f;
        bool wasPressed = prevRT > 0.5f;

        HeavyAttack = isPressed && !wasPressed;

        prevRT = rt;
        if (LightAttack || HeavyAttack)
            AttackPressed = true;
    }
    public void RequestLockOn(bool value)
    {
        IsLockOn = value;
    }
    public void AckDodgeFinish()
    {
        DodgeBuffered = false;
    }
}
