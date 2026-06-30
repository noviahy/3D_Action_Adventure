using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour
{
    // Player에서 사용하는 모든 MonoBehavior를 모아 PlayerController로 보내줌
    [Header("Init")]
    [SerializeField] InputManager input;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] CameraFollow3D cam;
    [SerializeField] CharacterController characterController;
    [SerializeField] LocomotionState locomotionState;
    [SerializeField] Animator animator;
    [SerializeField] Parrying parrying;
    [SerializeField] Dodge dodge;
    [SerializeField] Attack attack;
    [SerializeField] AttackState attackState;
    [SerializeField] AnimationEventController animatorEventController;
    [SerializeField] ActionIdle actionIdle;
    [SerializeField] BowAttack bowAttack;
    [SerializeField] Climb climb;
    [SerializeField] InteractionIdle interactionIdle;
    [SerializeField] Roll roll;
    [SerializeField] BoxInteractionState boxInteractionState;
    [SerializeField] RootMotionController rootMotionController;
    [SerializeField] EdgeCheck edgeCheck;
    [SerializeField] LayerController layerController;
    [SerializeField] Hang hang;
    [SerializeField] Mantle mantle;
    [SerializeField] Airborn airborn;

    [Header("Weapon")]
    [SerializeField] Renderer sword;
    [SerializeField] Renderer bow;

    private PlayerController Controller;

    // 2차 상태 확인
    // Interaction시 이것만 default로 바꾸고 
    public WeaponType currentWeaponType { get; private set; }

    public ItemType currentItemType { get; private set; }

    public bool Invincibility { get; private set; }
    public bool Guard { get; private set; }

    // 1차 상태 확인
    public int weaponNum { get; private set; } = 0;
    private int itemNum = 0;
    private bool isEquip = false;

    public Coroutine defaultCoroutine;
    private Coroutine changeCoroutine;

    public enum WeaponType
    {
        Default,
        Sword,
        Bow
    }
    private void Start()
    {
        sword.enabled = false;
        bow.enabled = false;

        var behaviours = GetComponentsInChildren<PlayerBehaviour>();

        foreach (var b in behaviours)
        {
            b.Init(Controller);
        }
    }
    private void Update()
    {
        // Debug.Log(stateMachine.currentState);
        if (stateMachine.currentState != PlayerStateMachine.PlayerState.LocomotionState)
        {
            Controller.Input.AckWeaponInput();
            return;
        }

        if (Controller.Input.BowCharging)
        {
            weaponNum = 2;
            RequestChangeCoroutine();
        }

        int weaponLength = System.Enum.GetValues(typeof(WeaponType)).Length;

        if (Controller.Input.ChangeWeapon != 0 && changeCoroutine == null && defaultCoroutine == null)
        {
            weaponNum = (weaponNum + Controller.Input.ChangeWeapon + weaponLength) % weaponLength;
            Controller.Input.AckWeaponInput();
            RequestChangeCoroutine();
        }

        // Locomotionstate로 돌아오면 여기서 1차 변수 기반으로 변경
        ChangeWeaponType((WeaponType)weaponNum);

        int itemLength = System.Enum.GetValues(typeof(ItemType)).Length;

        if (Controller.Input.ChangeItem != 0 && changeCoroutine == null)
        {
            itemNum = (itemNum + Controller.Input.ChangeWeapon + itemLength) % itemLength;
            Controller.Input.AckItemInput();
            RequestChangeCoroutine();
        }

        ChangeItemType((ItemType)itemNum);
    }
    public void ChangeWeaponType(WeaponType type)
    {
        if (currentWeaponType == type)
            return;

        //Debug.Log($"WaponType:{type}");

        currentWeaponType = type;

        switch (type)
        {
            case WeaponType.Default:
                RequestCoroutine();
                Controller.Animation.SetWeaponType(0);
                return;

            case WeaponType.Sword:
                isEquip = true;
                Controller.LayerController.RequestLayer2On(0.2f);
                Controller.Animation.SetWeaponType(2);
                Controller.Animation.PlayUpperBody("Sword");
                sword.enabled = true; // 콜라이더 활성화는 다른 코드에서
                bow.enabled = false;
                return;
            case WeaponType.Bow:
                isEquip = true;
                Controller.Input.RequestLockOn(false);
                Controller.LayerController.RequestLayer2On(0.2f);
                Controller.Animation.SetWeaponType(1);
                Controller.Animation.PlayUpperBody("Bow");
                sword.enabled = false;
                bow.enabled = true;
                return;
        }
    }
    public ItemType CurrentItemType { get; private set; }
    public enum ItemType
    {
        HPPosion,
        Bomb
    }

    public void ChangeItemType(ItemType type)
    {
        if (currentItemType == type)
            return;

        Debug.Log($"ItemType:{type}");

        currentItemType = type;

        switch (type)
        {
            case ItemType.HPPosion:
                RequestCoroutine();
                return;

            case ItemType.Bomb:
                return;
        }
    }
    private void RequestCoroutine()
    {
        if (defaultCoroutine == null)
            defaultCoroutine = StartCoroutine(DefaultWeapon());
    }
    IEnumerator DefaultWeapon()
    {
        yield return new WaitUntil(() => !isEquip);

        sword.enabled = false;
        bow.enabled = false;

        layerController.RequestLayer2Off(0.3f);

        defaultCoroutine = null;
    }
    private void RequestChangeCoroutine()
    {
        if (changeCoroutine == null)
            changeCoroutine = StartCoroutine(WaitForChangeInput());
    }
    IEnumerator WaitForChangeInput()
    {
        yield return new WaitForSeconds(0.7f);
        changeCoroutine = null;
    }
    public void Unequip()
    {
        isEquip = false;
    }
    // 무적
    public void ChangeInvincibility(bool value)
    {
        Invincibility = value;
    }
    public void RequestWeaponRendererOff()
    {
        sword.enabled = false;

        bow.enabled = false;
    }
    public void RequestWeaponRendererOn()
    {
        if (currentWeaponType == WeaponType.Sword)
            sword.enabled = true;
        if (currentWeaponType == WeaponType.Bow)
            bow.enabled = true;
    }
    public void NormalGuard(bool value)
    {
        Guard = value;
    }

    // 나중에 Item 사용시 무기를 끄고 키는거 만들어야함
    private void Awake()
    {
        // Controller 생성
        Controller = new PlayerController(
            input,
            this,
            stateMachine,
            groundCheck,
            cam,
            characterController,
            locomotionState,
            dodge,
            animator,
            attack,
            attackState,
            animatorEventController,
            actionIdle,
            parrying,
            bowAttack,
            climb,
            interactionIdle,
            roll,
            boxInteractionState,
            rootMotionController,
            edgeCheck,
            layerController,
            hang,
            mantle,
            airborn
        );
        Controller.Animation.SetWeaponType(0);
    }
}
