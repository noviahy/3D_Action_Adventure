using UnityEngine;


public class InputManager : PlayerBehaviour
{
    public Vector3 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool LightAttack { get; private set; }
    public bool HeavyAttack { get; private set; }

    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool DodgePressed { get; private set; }

    public bool StartBowCharging { get; private set; }
    public bool BowCharging { get; private set; }
    public bool BowShoot { get; private set; }

    public bool IsLockOn { get; private set; } = false;

    public bool InteractionPressed { get; private set; }
    public bool ChangeItemNext { get; private set; }
    public bool ChangeItemPrev { get; private set; }

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    public bool LocomotionPressed { get; private set; }
    public bool ActionPressed { get; private set; }
    public float forward {  get; private set; }
    public float side { get; private set; }

    private float prevRT;
    public InputMode CurrentInput { get; private set; }

    public enum InputMode
    {
        PlayerInput,
        UIInput,
        InputLock
    }
    public void ChangeInputMode(InputMode mode)
    {
        if (CurrentInput == mode) return;

        CurrentInput = mode;
    }

    void Update() // 동시에 누를때 생기는 문제 처리해야함
    {
        MouseX = Input.GetAxis("LookX");
        MouseY = Input.GetAxis("LookY");

        forward = Input.GetAxisRaw("Vertical");
        side = Input.GetAxisRaw("Horizontal");
        MoveInput = con.Cam.camForward * forward + con.Cam.camRight * side;

        AttackPressed = Input.GetButtonDown("Attack");
        LightAttack = Input.GetButtonDown("Light");
        float rt = Input.GetAxis("Heavy");

        bool isPressed = rt > 0.5f;
        bool wasPressed = prevRT > 0.5f;

        HeavyAttack = isPressed && !wasPressed;

        prevRT = rt;

        ChangeItemNext = Input.GetButtonDown("ChangeItemNext");
        ChangeItemPrev = Input.GetButtonDown("ChangeItemPre");

        InteractionPressed = Input.GetButtonDown("Interaction");

        RunPressed = Input.GetButton("Run");

        ParryingPressed = Input.GetButton("Parrying");

        DodgePressed = Input.GetButtonDown("Dodge");

        float bow = Input.GetAxis("Bow");

        bool wasBowCharging = BowCharging;

        BowCharging = bow > 0.5f;
        StartBowCharging = BowCharging && !wasBowCharging;
        BowShoot = !BowCharging && wasBowCharging;

        ActionPressed = AttackPressed || ParryingPressed || DodgePressed || BowCharging || InteractionPressed;

        if (Input.GetButtonDown("LockOn"))
            IsLockOn = !IsLockOn;
        con.Animation.SetLockOn(IsLockOn);
    }
    public void RequestLockOn(bool value)
    {
        IsLockOn = value;
    }
}
