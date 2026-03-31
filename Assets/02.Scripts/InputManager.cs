using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private CameraFollow3D cam;
    public Vector3 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool LightAttack {  get; private set; }
    public bool HeavyAttack {  get; private set; }
    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool StartBowCharging { get; private set; }
    public bool BowCharging { get; private set; }
    public bool BowShoot { get; private set; }
    public bool isLockOn { get; private set; }
    public bool InteractionPressed {  get; private set; }
    public bool ChangeItemNext {  get; private set; }
    public bool ChangeItemPrev { get; private set; }
    public float mouseX { get; private set; }
    public float mouseY { get; private set; }
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
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        float forward = Input.GetAxisRaw("Vertical");
        float side = Input.GetAxisRaw("Horizontal");
        MoveInput = cam.camForward * forward + cam.camRight * side;

        AttackPressed = Input.GetButtonDown("Attack");
        LightAttack = Input.GetButtonDown("Light");
        HeavyAttack = Input.GetButtonDown("Heavy");

        ChangeItemNext = Input.GetButtonDown("ChangeItemNext");
        ChangeItemPrev = Input.GetButtonDown("ChangeItemPre");

        InteractionPressed = Input.GetButtonDown("Interaction");

        RunPressed = Input.GetButton("Run");

        ParryingPressed = Input.GetButton("Parrying");

        RollPressed = Input.GetButtonDown("Roll");

        StartBowCharging = Input.GetButtonDown("Bow");
        BowCharging = Input.GetButton("Bow");
        BowShoot = Input.GetButtonUp("Bow");
    }
}
