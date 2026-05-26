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
    [SerializeField] Animator animator;
    [SerializeField] Parrying parrying;
    [SerializeField] Dodge dodge;
    [SerializeField] Attack attack;
    [SerializeField] AttackState attackState;
    [SerializeField] AnimationEventController animatorEventController;
    [SerializeField] ActionIdle actionIdle;
    [SerializeField] BowAttack bowAttack;
    [SerializeField] Climb climb;

    [Header("Weapon")]
    [SerializeField] Renderer sword;
    [SerializeField] Renderer bow;

    private PlayerController Controller;

    // 매 Attack마다 ChangeWeaponType를 해줘야
    // 무기 바꿀 때 바꾼지 확인 가능
    // 이거 옮겨줘야할듯
    public WeaponType currentWeaponType { get; private set; }

    public ItemType currentItemType { get; private set; }

    public bool Invincibility { get; private set; }
    public bool Guard { get; private set; }

    private int weaponNum = 0;
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
        // Debug.Log(currentWeaponType);
        // Debug.Log(Controller.Input.IsLockOn);
        if (attackState.isAttacking || Controller.ActionIdle.IdleBlending)
            return;

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

        // Debug.Log($"WaponType:{type}");

        currentWeaponType = type;

        switch (type)
        {
            case WeaponType.Default:
                Controller.Animation.SetWeaponType(0);
                RequestCoroutine();
                return;

            case WeaponType.Sword:
                isEquip = true;
                Controller.Animation.SetLayerWeight(2, 1);
                Controller.Animation.SetWeaponType(2);
                Controller.Animation.PlayUpperBody("Sword");
                sword.enabled = true; // 콜라이더 활성화는 다른 코드에서
                bow.enabled = false;
                return;
            case WeaponType.Bow:
                isEquip = true;
                Controller.Input.RequestLockOn(false);
                Controller.Animation.SetLayerWeight(2, 1);
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

        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * 3;

            Controller.Animation.SetLayerWeight(2, 1 - t);

            yield return null;
        }
        Controller.Animation.SetLayerWeight(2, 0);

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
    public void NormalGuard(bool value)
    {
        Guard = value;
    }

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
            dodge,
            animator,
            attack,
            attackState,
            animatorEventController,
            actionIdle,
            parrying,
            bowAttack,
            climb
        );
        Controller.Animation.SetWeaponType(0);
    }
}
