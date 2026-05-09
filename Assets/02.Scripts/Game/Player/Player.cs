using UnityEngine;

public class Player : MonoBehaviour
{
    // Player에서 사용하는 모든 MonoBehavior를 모아 PlayerController로 보내줌
    [Header("Init")]
    [SerializeField] InputManager input;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] InteractionState interaction;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] CameraFollow3D cam;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] Parrying parrying;
    [SerializeField] Dodge dodge;
    [SerializeField] Attack attack;
    [SerializeField] AttackState attackState;

    [Header("Weapon")]
    [SerializeField] Renderer sword;

    private PlayerController Controller;

    // 매 Attack마다 ChangeWeaponType를 해줘야
    // 무기 바꿀 때 바꾼지 확인 가능
    // 이거 옮겨줘야할듯
    public WeaponType currentWeaponType { get; private set; }
    public WeaponType previousWeaponType { get; private set; }
    public bool IsInvincible { get; private set; }
    private int weaponNum = 0;

    public enum WeaponType
    {
        Default,
        Sword,
        Bow
    }
    private void Start()
    {
        sword.enabled = false;

        var behaviours = GetComponentsInChildren<PlayerBehaviour>();

        foreach (var b in behaviours)
        {
            b.Init(Controller);
        }
        // ChangeWeaponType(WeaponType.Default);
    }

    private void Update()
    {
        // Debug.Log(currentWeaponType);
        if(attackState.isAttacking)
            return;

        int length = System.Enum.GetValues(typeof(WeaponType)).Length;
        int index = (int)weaponNum;
        if (Controller.Input.isPressed)
        {
            index = (index + Controller.Input.ChangeWeapon + length) % length;
            ChangeWeaponType((WeaponType)index);
        }
    }
    public void ChangeWeaponType(WeaponType type)
    {
        if (currentWeaponType == type)
            return;

        Debug.Log($"WaponType:{type}");

        previousWeaponType = currentWeaponType;
        currentWeaponType = type;

        switch (type)
        {
            case WeaponType.Default:
                sword.enabled = false;
                Controller.Animator.SetLayerWeight(2, 0);
                Controller.Animation.SetWeaponType(0);
                // 나중에 활 추가
                return;

            case WeaponType.Sword:
                sword.enabled = true; // 콜라이더 활성화는 다른 코드에서
                Controller.Animator.SetLayerWeight(2, 1);
                Controller.Animation.SetWeaponType(2);
                return;
            case WeaponType.Bow:
                Controller.Animator.SetLayerWeight(2, 1);
                Controller.Animation.SetWeaponType(1);
                // 활 렌더러 켜기
                return;
        }
    }

    public ItemType CurrentItemType { get; private set; }
    public enum ItemType
    {
        HPPosion,
        Bomb
    }
    // 무적
    public void ChangeInvincible(bool value) // 카운터 시간은 따로 만들어야함
    {
        IsInvincible = value;
    }

    private void Awake()
    {
        // Controller 생성
        Controller = new PlayerController(
            input,
            this,
            stateMachine,
            interaction,
            groundCheck,
            cam,
            characterController,
            dodge,
            animator,
            attack,
            attackState
        );
    }
}
